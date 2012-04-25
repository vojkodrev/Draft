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
using Helpers.Diagnostics;
using System.Threading;

namespace Draft.DraftSites.CCGDecks
{
    public class CCGDecksDraftSite : IDraftSite
    {
        private readonly CookieContainer cookies;
        private const int REFRESH_TIMEOUT = 2000;
        private const int DOWNLOAD_THREAD_COUNT = 20;

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
        private static bool InADraft(string draftPage)
        {
            //if (!draftPage.Contains("View Draft Table") || !draftPage.Contains("Current picks:"))
            //    throw new Exception("You are not in a draft!");

            return draftPage.Contains("View Draft Table") && draftPage.Contains("Current picks:");
        }
        private static bool DraftFinished(string draftPage)
        {

            return draftPage.Contains("<title>Draft Deck Builder");
        }
        public void GetCurrentPicks()
        {
            OnGetCurrentPicksStarted(this, new EventArgs());

            try
            {
                bool first = true;

                while (true)
                {
                    if (!first)
                        Thread.Sleep(REFRESH_TIMEOUT);

                    first = false;                                        

                    string draftPage = HttpHelper.Get("http://ccgdecks.com/draft_gen.php", cookies);

                    if (DraftFinished(draftPage))
                    {
                        OnGetCurrentPicksError(this, new ErrorEventArgs { Error = "Draft finished!" });
                        break;
                    }

                    if (!InADraft(draftPage))
                        continue;

                    try
                    {
                        string timeLeftPattern = "<h3>Please select a card from below, you have <font color=\"red\">(\\d*)</font> seconds left to choose</h3>";
                        MatchCollection match = RegexHelper.Match(timeLeftPattern, draftPage);
                        int timeLeft = Convert.ToInt32(match[0].Groups[1].Value);
                        OnTimeLeftReceived(this, new TimeLeftEventArgs { TimeLeft = timeLeft });
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    string pattern = "<a href=\"javascript: PickCard\\((.*?)\\);\".*?><img.*?src=\"(.*?)\".*?alt=\"(.*?)\"></a>";

                    MatchCollection matches = RegexHelper.Match(pattern, RegexHelper.ReplaceWS(draftPage));

                    if (matches.Count == 0)
                        continue;

                    Match[] ma = new Match[matches.Count];
                    matches.CopyTo(ma, 0);

                    Parallel.ForEach<Match>(ma, new ParallelOptions { MaxDegreeOfParallelism = DOWNLOAD_THREAD_COUNT }, match =>
                    {
                        string pictureUrl = match.Groups[2].Value;
                        string id = match.Groups[1].Value;
                        string name = HttpUtility.HtmlDecode(match.Groups[3].Value).Trim();

                        Bitmap picture = PictureCache.GetPicture(name, pictureUrl);

                        OnCurrentPickReceived(this, new CardEventArgs { Card = new Card { Id = id, Name = name, Picture = picture } });
                    });

                    break;
                }
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
                bool first = true;

                while (true)
                {
                    if (!first)
                        Thread.Sleep(REFRESH_TIMEOUT);

                    first = false;

                    string picksPage = HttpHelper.Get("http://ccgdecks.com/drafting.php", cookies);

                    if (DraftFinished(picksPage))
                    {
                        OnGetPickedCardsError(this, new ErrorEventArgs { Error = "Draft finished!" });
                        break;
                    }

                    if (!InADraft(picksPage))
                        continue;

                    string pattern = "(\\d*) x <a href=\"javascript: viewCard\\('(\\d*)'.*?title=.*?>(.*?)</a>"; 
                    MatchCollection matches = RegexHelper.Match(pattern, RegexHelper.ReplaceWS(picksPage));

                    if (matches.Count == 0)
                        continue;

                    Match[] ma = new Match[matches.Count];
                    matches.CopyTo(ma, 0);

                    Parallel.ForEach<Match>(ma, new ParallelOptions { MaxDegreeOfParallelism = DOWNLOAD_THREAD_COUNT }, match =>
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
                            string cardViewPagePattern = "<tr>.*?<td valign=\"top\">.*?<img src=\"(.*?)\">.*?</td>";
                            MatchCollection cardUrlMatch = RegexHelper.Match(cardViewPagePattern, RegexHelper.ReplaceWS(cardViewPage));
                            picture = PictureCache.GetPicture(name, cardUrlMatch[0].Groups[1].Value);
                        }

                        for (int i = 0; i < numberOfPicks; i++)
                            OnPickedCardReceived(this, new CardEventArgs { Card = new Card { Id = id, Name = name, Picture = picture } });
                    });

                    break;
                }
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