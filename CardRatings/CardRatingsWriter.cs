using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers.Xml;

namespace CardRatings
{
    public abstract class CardRatingsWriter
    {
        public abstract Dictionary<string, CardRatingsItem> GetRatings();

        public void Save(string file)
        {
            XmlHelper.ToFile(file, GetRatings().ToList().Select(i => i.Value).ToList());
        }
    }
}