using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Helpers.Xml;

namespace CardRatings
{
    public class CardRatingsReader
    {
        public static Dictionary<string, CardRatingsItem> Read(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*.xml");
            Dictionary<string, CardRatingsItem> result = new Dictionary<string, CardRatingsItem>();

            foreach (string file in files)
            {
                List<CardRatingsItem> fileContent = XmlHelper.FromFile<List<CardRatingsItem>>(file);
                foreach (CardRatingsItem cardRatingsItem in fileContent)
                    result.Add(cardRatingsItem.CardName, cardRatingsItem);
            }

            return result;
        }
    }
}
