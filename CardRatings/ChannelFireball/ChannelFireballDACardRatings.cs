using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers.Http;
using System.Net;
using Helpers.Regex;
using System.Text.RegularExpressions;
using System.Globalization;

namespace CardRatings.ChannelFireball
{
    public class ChannelFireballDACardRatings : CardRatingsWriter
    {
        private readonly string ratingsSite;

        public ChannelFireballDACardRatings(string ratingsSite)
        {
            this.ratingsSite = ratingsSite;            
        }

        public override Dictionary<string, CardRatingsItem> GetRatings()
        {
            string ratingsPageContent = HttpHelper.Get(ratingsSite, new CookieContainer());

            string pattern = "style='display:none;width:1px;height:1px;' /></p><p>(.*?)</p><p>Constructed: \\d*?\\.\\d*?</p><p>.*?</p><p>Limited: (\\d*?\\.\\d*?)</p><p>(.*?)</p>";
            MatchCollection matches = RegexHelper.Match(pattern, RegexHelper.ReplaceWS( ratingsPageContent));
            Dictionary<string, CardRatingsItem> result = new Dictionary<string, CardRatingsItem>();
            foreach (Match match in matches)
            {
                string ratingDesc = match.Groups[3].Value;
                double rating = Convert.ToDouble(match.Groups[2].Value, new NumberFormatInfo { NumberDecimalSeparator = "." });
                string cardName = match.Groups[1].Value;

                // fix if there is a link in name
                string namePattern = "<a href=\".*?\".*?>(.*?)</a>.*?";
                MatchCollection nameMatches = RegexHelper.Match(namePattern, cardName);
                if (nameMatches.Count > 0)
                    cardName = nameMatches[0].Groups[1].Value;                

                // remove links from desc
                string linkPattern = "<a href=\".*?\".*?>(.*?)</a>";
                ratingDesc = Regex.Replace(ratingDesc, linkPattern, "$1");

                CardRatingsItem cardRatingsItem = new CardRatingsItem { CardName = cardName, Rating = rating, RatingDescription = ratingDesc };
                result.Add(cardName, cardRatingsItem);
            }

            return result;
        }
    }
}
