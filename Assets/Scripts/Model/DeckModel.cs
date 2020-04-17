﻿using System;
using System.Collections.Generic;
using System.Text;

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
        /// The filtered list of card id's that represent the staples.
        /// </summary>
        public List<CardModel> Staples;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (CardModel model in Staples)
            {
                builder.AppendLine($"{model.slot}");
            }
            return builder.ToString();
        }
    }
}
