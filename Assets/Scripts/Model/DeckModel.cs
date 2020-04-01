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
        /// the unique account id of the owner of the deck.
        /// </summary>
        public int ownerId;

        public CardModel[] cards;

        /// <summary>
        /// A list of cards for the deck.
        /// </summary>
        public List<CardModel> cardModels;

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

        public DateTime updatedAtDateTime;
        // todo add the colors of the deck here.
    }
}
