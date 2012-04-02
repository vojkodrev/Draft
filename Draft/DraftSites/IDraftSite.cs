using System;
using System.Collections.Generic;
using System.Linq;

namespace Draft.DraftSites
{
    public interface IDraftSite
    {
        void Login(string username, string password);
        void PickCard(string id);

        void GetCurrentPicks();
        event EventHandler<CardEventArgs> CurrentPickReceived;
        event EventHandler GetCurrentPicksStarted;
        event EventHandler GetCurrentPicksFinished;
        event EventHandler<ErrorEventArgs> GetCurrentPicksError;
        event EventHandler<TimeLeftEventArgs> TimeLeftReceived;
        void TerminateActionGetCurrentPicks();

        void GetPickedCards();
        event EventHandler<CardEventArgs> PickedCardReceived;
        event EventHandler GetPickedCardsStarted;
        event EventHandler GetPickedCardsFinished;
        event EventHandler<ErrorEventArgs> GetPickedCardsError;
        void TerminateActionGetPickedCards();        
    }
}