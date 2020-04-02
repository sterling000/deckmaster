using System;

namespace deckmaster
{
    [Serializable]
    public class DeckData
    {
        public DateTime updatedAt;

        public DateTime lastSeen;

        public DeckModel deckModel;

        public CardModel[] staples;
    }
}
