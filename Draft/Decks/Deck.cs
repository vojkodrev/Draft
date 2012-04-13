using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Draft.Decks
{
    public class Deck
    {

        public Deck()
        {
            Cards = new List<string>();
            SideboardCards = new List<string>();
        }

        public List<string> Cards { get; set; }
        public List<string> SideboardCards { get; set; }

        public string Generate()
        {
            string result = "";

            Cards.GroupBy(i => i).ToList().ForEach(i => result += String.Format("{0} {1}{2}", i.Count(), i.Key, Environment.NewLine));
            SideboardCards.GroupBy(i => i).ToList().ForEach(i => result += String.Format("sb: {0} {1}{2}", i.Count(), i.Key, Environment.NewLine));

            return result;
        }
    }
}
