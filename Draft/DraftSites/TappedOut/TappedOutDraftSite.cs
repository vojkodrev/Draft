using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Draft.Pictures;
using System.Drawing;

namespace Draft.DraftSites.TappedOut
{
    public class TappedOutDraftSite : IDraftSite
    {
        private readonly CookieContainer cookieContainer = new CookieContainer();
        private string username = "";

        public event EventHandler<CardEventArgs> CurrentPickReceived;
        public event EventHandler GetCurrentPicksStarted;
        public event EventHandler GetCurrentPicksFinished;
        public event EventHandler<CardEventArgs> PickedCardReceived;
        public event EventHandler GetPickedCardsStarted;
        public event EventHandler GetPickedCardsFinished;
        public event EventHandler<ErrorEventArgs> GetCurrentPicksError;
        public event EventHandler<ErrorEventArgs> GetPickedCardsError;
        public event EventHandler<TimeLeftEventArgs> TimeLeftReceived;

        public virtual void OnTimeLeftReceived(object sender, TimeLeftEventArgs e)
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
            string request = Http.HttpHelper.Get("http://tappedout.net/accounts/login", cookieContainer);
            MatchCollection matches = Regex.RegexHelper.Match("<input type='hidden' name='csrfmiddlewaretoken' value='(.*?)' />", request);

            string loginResponse = Http.HttpHelper.Post("http://tappedout.net/accounts/login/", new Dictionary<string, string> 
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
            string newVariable = Http.HttpHelper.Get("http://tappedout.net" + id, cookieContainer);
        }

        public void GetCurrentPicks()
        {
            OnGetCurrentPicksStarted(this, new EventArgs());

            try
            {
                string draftPage = Http.HttpHelper.Get("http://tappedout.net/mtg-draft-simulator/", cookieContainer);

                if (!draftPage.Contains("Players will auto-pick after holding pack"))
                    throw new Exception("Not in a draft!");

                try
                {
                    string countDownPattern = String.Format("<span style=\"text-decoration:none;\"><a href='/users/.*?/'>{0}</a></span>.*?<td class=\"player-countdown\">(.*?)</td>", username);
                    MatchCollection timeLeftMatches = Regex.RegexHelper.Match(countDownPattern, draftPage.Replace("\n", " ").Replace("\r", " ").Replace("\t", " "));
                    int timeLeft = Convert.ToInt32(timeLeftMatches[0].Groups[1].Value);
                    OnTimeLeftReceived(this, new TimeLeftEventArgs { TimeLeft = timeLeft });
                }
                catch (Exception)
                {
                    OnGetCurrentPicksError(this, new ErrorEventArgs { Error = "Unable to get time left!" });
                }

                string pattern = "<a target=\"_new\" class=\"card-hover pick\" href=\"(.*?)\">.*?<span class=\"image-box hide\"><img src=\"(.*?)\" alt=\"MTG Card: (.*?)\" /><br />";
                MatchCollection matches = Regex.RegexHelper.Match(pattern, draftPage);
                Match[] ma = new Match[matches.Count];
                matches.CopyTo(ma, 0);

                Parallel.ForEach<Match>(ma, new ParallelOptions { MaxDegreeOfParallelism = 20 }, match =>
                {
                    //System.Drawing.Bitmap dwnPic = Http.HttpHelper.DownloadPicture(match.Groups[2].Value);
                    string name = match.Groups[3].Value;
                    var dwnPic = PictureCache.GetPicture(name, match.Groups[2].Value);
                    Card card = new Card { Id = match.Groups[1].Value, Name = name, Picture = dwnPic };
                    OnCurrentPickReceived(this, new CardEventArgs { Card = card });
                });
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
        public void GetPickedCards()
        {
            OnGetPickedCardsStarted(this, new EventArgs());

            try
            {
                string draftPage = Http.HttpHelper.Get("http://tappedout.net/mtg-draft-simulator/", cookieContainer);

                if (!draftPage.Contains("Players will auto-pick after holding pack"))
                    throw new Exception("Not in draft!");

                string pattern = "<input type=\"checkbox\" id=\"exclude-\\d*\" checked=\"checked\" class=\"exclude\" /> <span class=\".*?\"><a  class=\"card-hover\" href=\".*?\">(.*?)</a><span.*?<img class=\"screen\" src=\"(.*?)\" alt=\"MTG Card: .*?\" /></span>";
                MatchCollection matches = Regex.RegexHelper.Match(pattern, draftPage);
                Match[] ma = new Match[matches.Count];
                matches.CopyTo(ma, 0);
                Parallel.ForEach(ma, new ParallelOptions { MaxDegreeOfParallelism = 20 }, matche =>
                {
                    string name = matche.Groups[1].Value;
                    Bitmap pic = PictureCache.GetPicture(name, matche.Groups[2].Value);
                    Card card = new Card { Name = name, Picture = pic };
                    OnPickedCardReceived(this, new CardEventArgs { Card = card });
                });
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
