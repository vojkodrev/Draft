using System;
using System.Collections.Generic;
using System.Linq;
using Draft.Http;
using System.Net;
using System.Text.RegularExpressions;
using Drafting.Xml;

namespace Draft.DraftSites.CCGDecks
{
    public class CCGDecksDraftSite : IDraftSite
    {
        private readonly CookieContainer cookies;

        public CCGDecksDraftSite()
        {
            cookies = new CookieContainer();
        }

        public void Login(string username, string password)
        {
            string loginPage = HttpHelper.Post("http://ccgdecks.com/board/login.php", new Dictionary<string, string> { 
                { "username", username }, 
                { "password", password },
                { "login", "Log in" },
            }, cookies);

            CheckIfLoginSuccessful(loginPage);
        }
        public List<PickPicture> GetPickPictures()
        {
            string draftPage = HttpHelper.Get("http://ccgdecks.com/draft_gen.php", cookies);
            CheckIfInADraft(draftPage);

            List<PictureUrlIdAndName> extractedPickPicsUrls = ExtractPickPictureUrls(draftPage);

            return DownloadPictures(extractedPickPicsUrls);
        }
        public void PickCard(string id)
        {
            string response = HttpHelper.Post("http://ccgdecks.com/drafting.php", new Dictionary<string, string> { { "card_picked", id }, { "viewtype", "picture" } }, cookies);            
        }
        public List<string> GetPicks()
        {
            string picksPage = HttpHelper.Get("http://ccgdecks.com/drafting.php", cookies);

            CheckIfInADraft(picksPage);

            return ExtractPicks(picksPage);
        }

        private static List<string> ExtractPicks(string picksPage)
        {
            // 2 x <a href="javascript: viewCard('4428');" title="Ray of Revelation">Ray of Revelation</a> (1<img src="images/sm_white.gif">)<br>
            string pattern = "(\\d*) x <a href=.*?title=.*?>(.*?)</a>.*?<img.*?>.*?<br>";
            MatchCollection matches = Regex.RegexHelper.Match(pattern, picksPage.Replace("\n", " ").Replace("\r", " "));
            List<string> result = new List<string>();
            foreach (Match match in matches)
            {
                int numberOfPicks = Convert.ToInt32(match.Groups[1].Value);

                for (int i = 0; i < numberOfPicks; i++)
                    result.Add(match.Groups[2].Value);
            }

            return result;
        }
        private static void CheckIfLoginSuccessful(string loginPage)
        {
            if (!loginPage.Contains("Logout</a></li>"))
                throw new LoginFailedException("Login failed!");
        }
        private static List<PickPicture> DownloadPictures(List<PictureUrlIdAndName> extractedPickPicsUrls)
        {
            List<PickPicture> result = new List<PickPicture>();

            foreach (PictureUrlIdAndName extractedPickPicsUrl in extractedPickPicsUrls)
            {
                System.Drawing.Bitmap pic = HttpHelper.DownloadPicture(extractedPickPicsUrl.Url);
                result.Add(new PickPicture { Id = extractedPickPicsUrl.Id, Picture = pic, Name = extractedPickPicsUrl.Name });
            }

            return result;
        }
        private static void CheckIfInADraft(string draftPage)
        {
            if (!draftPage.Contains("View Draft Table") || !draftPage.Contains("Current picks:"))
                throw new NotInADraftException("You are not in a draft!");
        }
        private static List<PictureUrlIdAndName> ExtractPickPictureUrls(string draftPage)
        {
            string pattern = "<a href=\"javascript: PickCard\\((.*?)\\);\".*?><img.*?src=\"(.*?)\".*?alt=\"(.*?)\"></a>";

            MatchCollection matches = Regex.RegexHelper.Match(pattern, draftPage.Replace("\n", " ").Replace("\r", " "));
            List<PictureUrlIdAndName> result = new List<PictureUrlIdAndName>();
            foreach (Match match in matches)
                result.Add(new PictureUrlIdAndName { Url = match.Groups[2].Value, Id = match.Groups[1].Value, Name = match.Groups[3].Value });

            return result;
        }
    }
}