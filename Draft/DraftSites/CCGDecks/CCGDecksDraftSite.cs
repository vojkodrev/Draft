using System;
using System.Collections.Generic;
using System.Linq;
using Helpers.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using Helpers.Pictures;
using Helpers.Regex;
using System.Web;

namespace Draft.DraftSites.CCGDecks
{
    public class CCGDecksDraftSite : IDraftSite
    {
        private readonly CookieContainer cookies;        

        public CCGDecksDraftSite()
        {
            cookies = new CookieContainer();            
        }

        public event EventHandler<CardEventArgs> CurrentPickReceived;
        public event EventHandler GetCurrentPicksStarted;
        public event EventHandler GetCurrentPicksFinished;
        public event EventHandler<ErrorEventArgs> GetCurrentPicksError;
        public event EventHandler<TimeLeftEventArgs> TimeLeftReceived;
        public event EventHandler<CardEventArgs> PickedCardReceived;
        public event EventHandler GetPickedCardsStarted;
        public event EventHandler GetPickedCardsFinished;
        public event EventHandler<ErrorEventArgs> GetPickedCardsError;

        protected virtual void OnGetPickedCardsError(object sender, ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> handler = GetPickedCardsError;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnGetPickedCardsFinished(object sender, EventArgs e)
        {
            EventHandler handler = GetPickedCardsFinished;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnGetPickedCardsStarted(object sender, EventArgs e)
        {
            EventHandler handler = GetPickedCardsStarted;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnPickedCardReceived(object sender, CardEventArgs e)
        {
            EventHandler<CardEventArgs> handler = PickedCardReceived;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnTimeLeftReceived(object sender, TimeLeftEventArgs e)
        {
            EventHandler<TimeLeftEventArgs> handler = TimeLeftReceived;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnGetCurrentPicksError(object sender, ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> handler = GetCurrentPicksError;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnGetCurrentPicksFinished(object sender, EventArgs e)
        {
            EventHandler handler = GetCurrentPicksFinished;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnGetCurrentPicksStarted(object sender, EventArgs e)
        {
            EventHandler handler = GetCurrentPicksStarted;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnCurrentPickReceived(object sender, CardEventArgs e)
        {
            EventHandler<CardEventArgs> handler = CurrentPickReceived;
            if (handler != null)
                handler(sender, e);
        }

        public void Login(string username, string password)
        {
            string loginPage = HttpHelper.Post("http://ccgdecks.com/board/login.php", new Dictionary<string, string> { 
                { "username", username }, 
                { "password", password },
                { "login", "Log in" },
            }, cookies);

            if (!loginPage.Contains("Logout</a></li>"))
                throw new Exception("Login failed!");
        }
        public void PickCard(string id)
        {
            string response = HttpHelper.Post("http://ccgdecks.com/drafting.php", new Dictionary<string, string> { { "card_picked", id }, { "viewtype", "picture" } }, cookies);
        }
        private static void CheckIfInADraft(string draftPage)
        {
            if (!draftPage.Contains("View Draft Table") || !draftPage.Contains("Current picks:"))
                throw new Exception("You are not in a draft!");
        }
        public void GetCurrentPicks()
        {
            OnGetCurrentPicksStarted(this, new EventArgs());

            try
            {
                string draftPage = HttpHelper.Get("http://ccgdecks.com/draft_gen.php", cookies);

                CheckIfInADraft(draftPage);

                try
                {
                    string timeLeftPattern = "<h3>Please select a card from below, you have <font color=\"red\">(\\d*)</font> seconds left to choose</h3>";
                    MatchCollection match = RegexHelper.Match(timeLeftPattern, draftPage);
                    int timeLeft = Convert.ToInt32(match[0].Groups[1].Value);
                    OnTimeLeftReceived(this, new TimeLeftEventArgs { TimeLeft = timeLeft });
                }
                catch (Exception)
                {
                    OnGetCurrentPicksError(this, new ErrorEventArgs { Error = "Unable to get time left!" });
                }

                string pattern = "<a href=\"javascript: PickCard\\((.*?)\\);\".*?><img.*?src=\"(.*?)\".*?alt=\"(.*?)\"></a>";

                MatchCollection matches = RegexHelper.Match(pattern, draftPage.Replace("\n", " ").Replace("\r", " "));
                Match[] ma = new Match[matches.Count];
                matches.CopyTo(ma, 0);

                Parallel.ForEach<Match>(ma, new ParallelOptions { MaxDegreeOfParallelism = 20 }, match =>
                {
                    string pictureUrl = match.Groups[2].Value;
                    string id = match.Groups[1].Value;
                    string name = HttpUtility.HtmlDecode(match.Groups[3].Value).Trim();

                    Bitmap picture = PictureCache.GetPicture(name, pictureUrl);

                    OnCurrentPickReceived(this, new CardEventArgs { Card = new Card { Id = id, Name = name, Picture = picture } });
                });
            }
            catch (Exception ex)
            {
                OnGetCurrentPicksError(this, new ErrorEventArgs { Error = ex.Message });
            }
            finally
            {
                OnGetCurrentPicksFinished(this, new EventArgs());
            }
        }
        public void GetPickedCards()
        {
            OnGetPickedCardsStarted(this, new EventArgs());

            try
            {
                string picksPage = HttpHelper.Get("http://ccgdecks.com/drafting.php", cookies);

                CheckIfInADraft(picksPage);

                // 2 x <a href="javascript: viewCard('4428');" title="Ray of Revelation">Ray of Revelation</a> (1<img src="images/sm_white.gif">)<br>
                string pattern = "(\\d*) x <a href=\"javascript: viewCard\\('(\\d*)'.*?title=.*?>(.*?)</a>.*?<img.*?>.*?<br>";
                MatchCollection matches = RegexHelper.Match(pattern, picksPage.Replace("\n", " ").Replace("\r", " "));
                Match[] ma = new Match[matches.Count];
                matches.CopyTo(ma, 0);

                Parallel.ForEach<Match>(ma, new ParallelOptions { MaxDegreeOfParallelism = 20 }, match =>
                {
                    int numberOfPicks = Convert.ToInt32(match.Groups[1].Value);
                    string id = match.Groups[2].Value;
                    string name = HttpUtility.HtmlDecode(match.Groups[3].Value).Trim();

                    Bitmap picture;

                    if (PictureCache.ContainsPicture(name))
                        picture = PictureCache.GetPictureDirectlyFromCache(name);
                    else
                    {
                        string cardViewPage = HttpHelper.Get("http://ccgdecks.com/card_view.php?id=" + id, cookies);
                        // <tr><td valign="top"><img src="http://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=262859&type=card"></td>
                        string cardViewPagePattern = "<tr>.*?<td valign=\"top\">.*?<img src=\"(.*?)\">.*?</td>";
                        MatchCollection cardUrlMatch = RegexHelper.Match(cardViewPagePattern, cardViewPage.Replace("\n", " ").Replace("\r", " ").Replace("\t", " "));
                        picture = PictureCache.GetPicture(name, cardUrlMatch[0].Groups[1].Value);
                    }

                    for (int i = 0; i < numberOfPicks; i++)
                        OnPickedCardReceived(this, new CardEventArgs { Card = new Card { Id = id, Name = name, Picture = picture } });
                });
            }
            catch (Exception ex)
            {
                OnGetPickedCardsError(this, new ErrorEventArgs { Error = ex.Message });
            }
            finally
            {
                OnGetPickedCardsFinished(this, new EventArgs());
            }
        }
        public void TerminateActionGetCurrentPicks()
        {

        }
        public void TerminateActionGetPickedCards()
        {

        }
    }
}