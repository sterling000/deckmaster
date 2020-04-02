using System;
using System.Collections.Generic;

namespace deckmaster
{
    [Serializable]
    public class DeckModel
    {
        /// <summary>
        /// The unique deck id from archidekt.
        /// </summary>
        public int id;
        
        /// <summary>
        /// Array of cards in the deck
        /// </summary>
        public CardModel[] cards;

        /// <summary>
        /// The Name of the deck
        /// </summary>
        public string name;

        /// <summary>
        /// A description of the deck.
        /// </summary>
        public string description;

        /// <summary>
        /// The last time the deck was updated on the server
        /// </summary>
        public string updatedAt;
    }
}
