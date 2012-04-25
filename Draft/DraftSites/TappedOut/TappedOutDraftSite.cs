using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Helpers.Pictures;
using System.Drawing;
using Helpers.Http;
using Helpers.Regex;
using System.Web;
using System.Threading;

namespace Draft.DraftSites.TappedOut
{
    public class TappedOutDraftSite : IDraftSite
    {
        private readonly CookieContainer cookieContainer = new CookieContainer();
        private string username = "";
        private const int REFRESH_TIMEOUT = 2000;
        private const int DOWNLOAD_THREAD_COUNT = 20;

        public event EventHandler<CardEventArgs> CurrentPickReceived;
        public event EventHandler GetCurrentPicksStarted;
        public event EventHandler GetCurrentPicksFinished;
        public event EventHandler<CardEventArgs> PickedCardReceived;
        public event EventHandler GetPickedCardsStarted;
        public event EventHandler GetPickedCardsFinished;
        public event EventHandler<ErrorEventArgs> GetCurrentPicksError;
        public event EventHandler<ErrorEventArgs> GetPickedCardsError;
        public event EventHandler<TimeLeftEventArgs> TimeLeftReceived;

        protected virtual void OnTimeLeftReceived(object sender, TimeLeftEventArgs e)
        {
            EventHandler<TimeLeftEventArgs> handler = TimeLeftReceived;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnGetPickedCardsError(object sender, ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> handler = GetPickedCardsError;
            if (handler != null)
                handler(sender, e);
        }
        protected virtual void OnGetCurrentPicksError(object sender, ErrorEventArgs e)
        {
            EventHandler<ErrorEventArgs> handler = GetCurrentPicksError;
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
            this.username = username;
            string request = HttpHelper.Get("http://tappedout.net/accounts/login", cookieContainer);
            MatchCollection matches = RegexHelper.Match("<input type='hidden' name='csrfmiddlewaretoken' value='(.*?)' />", request);

            string loginResponse = HttpHelper.Post("http://tappedout.net/accounts/login/", new Dictionary<string, string> 
            { 
                { "csrfmiddlewaretoken", matches[0].Groups[1].Value } ,                
                { "username", username }, 
                { "password", password },                               
            }, cookieContainer);

            if (!loginResponse.Contains(username) || !loginResponse.Contains("logout"))
                throw new Exception("Login failed!");
        }
        public void PickCard(string id)
        {
            string newVariable = HttpHelper.Get("http://tappedout.net" + id, cookieContainer);
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

                    string draftPage = HttpHelper.Get("http://tappedout.net/mtg-draft-simulator/", cookieContainer);

                    if (DraftCompleted(draftPage))
                    {
                        OnGetCurrentPicksError(this, new ErrorEventArgs { Error = "Draft has completed!" });
                        break;
                    }

                    if (!InADraft(draftPage))
                        continue;

                    try
                    {
                        string countDownPattern = String.Format("<span style=\"text-decoration:none;\"><a href='/users/.*?/'>{0}</a></span>.*?<td class=\"player-countdown\">(.*?)</td>", username);
                        MatchCollection timeLeftMatches = RegexHelper.Match(countDownPattern, RegexHelper.ReplaceWS(draftPage));
                        int timeLeft = Convert.ToInt32(timeLeftMatches[0].Groups[1].Value);
                        OnTimeLeftReceived(this, new TimeLeftEventArgs { TimeLeft = timeLeft });
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    string pattern = "<a target=\"_new\" class=\"card-hover pick\" href=\"(.*?)\">.*?<span class=\"image-box hide\"><img src=\"(.*?)\" alt=\"MTG Card: (.*?)\" /><br />";
                    MatchCollection matches = RegexHelper.Match(pattern, draftPage);

                    if (matches.Count == 0)
                        continue;

                    Match[] ma = new Match[matches.Count];
                    matches.CopyTo(ma, 0);

                    Parallel.ForEach<Match>(ma, new ParallelOptions { MaxDegreeOfParallelism = DOWNLOAD_THREAD_COUNT }, match =>
                    {
                        string name = HttpUtility.HtmlDecode(match.Groups[3].Value).Trim();
                        var dwnPic = PictureCache.GetPicture(name, match.Groups[2].Value);
                        Card card = new Card { Id = match.Groups[1].Value, Name = name, Picture = dwnPic };
                        OnCurrentPickReceived(this, new CardEventArgs { Card = card });
                    });

                    break;
                }
            }
            catch (Exception e)
            {
                OnGetCurrentPicksError(this, new ErrorEventArgs { Error = e.Message });
            }
            finally
            {
                OnGetCurrentPicksFinished(this, new EventArgs());
            }
        }
        private static bool InADraft(string draftPage)
        {
            //if (!draftPage.Contains("Players will auto-pick after holding pack"))
            //    throw new Exception("Not in draft!");

            return draftPage.Contains("Players will auto-pick after holding pack");
        }
        private static bool DraftCompleted(string draftPage)
        {
            return draftPage.Contains("Draft has completed");
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

                    string draftPage = HttpHelper.Get("http://tappedout.net/mtg-draft-simulator/", cookieContainer);

                    if (DraftCompleted(draftPage))
                    {
                        OnGetPickedCardsError(this, new ErrorEventArgs { Error = "Draft has completed!" });
                        break;
                    }

                    if (!InADraft(draftPage))
                        continue;

                    string pattern = "<input type=\"checkbox\" id=\"exclude-\\d*\" checked=\"checked\" class=\"exclude\" /> <span class=\".*?\"><a  class=\"card-hover\" href=\".*?\">(.*?)</a><span.*?<img class=\"screen\" src=\"(.*?)\" alt=\"MTG Card: .*?\" /></span>";
                    MatchCollection matches = RegexHelper.Match(pattern, draftPage);

                    if (matches.Count == 0)
                        continue;

                    Match[] ma = new Match[matches.Count];
                    matches.CopyTo(ma, 0);
                    Parallel.ForEach(ma, new ParallelOptions { MaxDegreeOfParallelism = DOWNLOAD_THREAD_COUNT }, matche =>
                    {
                        string name = HttpUtility.HtmlDecode(matche.Groups[1].Value).Trim();
                        Bitmap pic = PictureCache.GetPicture(name, matche.Groups[2].Value);
                        Card card = new Card { Name = name, Picture = pic };
                        OnPickedCardReceived(this, new CardEventArgs { Card = card });
                    });

                    break;
                }
            }
            catch (Exception e)
            {
                OnGetCurrentPicksError(this, new ErrorEventArgs { Error = e.Message });
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
