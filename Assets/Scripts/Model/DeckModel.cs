using System;
using System.Collections.Generic;

namespace deckmaster
{
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

        /// <summary>
        /// A list of cards for the deck.
        /// </summary>
        public List<CardModel> cards;

        /// <summary>
        /// The name of the deck
        /// </summary>
        public string name;

        /// <summary>
        /// A description of the deck.
        /// </summary>
        public string description;

        /// <summary>
        /// The last time the deck was updated on the server
        /// </summary>
        public DateTime updatedAt;

        // todo add the colors of the deck here.
    }
}
