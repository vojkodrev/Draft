using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Helpers.Xml;
using Helpers.Diagnostics;

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
                    try
                    {
                        result.Add(cardRatingsItem.CardName, cardRatingsItem);
                    }
                    catch (Exception)
                    {
                        TH.Warning("Ratings Reader: Card added twice: " + cardRatingsItem.CardName);
                    }
            }

            return result;
        }
    }
}
