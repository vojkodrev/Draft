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

            foreach (string card in Cards)
                result += String.Format("1 {0}{1}", card, Environment.NewLine);

            foreach (string card in SideboardCards)
                result += String.Format("sb: 1 {0}{1}", card, Environment.NewLine);

            return result;
        }
    }
}
