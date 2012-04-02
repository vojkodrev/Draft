using System;
using System.Collections.Generic;
using System.Linq;

namespace Draft.DraftSites
{
    public class CardEventArgs : EventArgs
    {
        public Card Card { get; set; }
    }
}
