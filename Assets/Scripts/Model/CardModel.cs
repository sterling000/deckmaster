using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace deckmaster
{
    public class CardModel
    {
        /// <summary>
        /// A unique identifier for this card, taken from archidekt.
        /// </summary>
        public int id;

        /// <summary>
        /// Another unique identifier for the card, taken from archidekt.
        /// </summary>
        public string uid;
        
        /// <summary>
        /// unknown as of yet, but might have to do with the tags on cards in archidekt.
        /// </summary>
        public string options;

        /// <summary>
        /// todo: Let make this into an enum flag. cause i like readable enum flags, and determine if we require the colorIdentiy property from archidekt as well.
        /// </summary>
        public string colors;

        /// <summary>
        /// The card name as read from oracle.
        /// </summary>
        public string name;

        /// <summary>
        /// Mana cost in {2}{W} format
        /// </summary>
        public string cost;

        /// <summary>
        /// Converted mana cost
        /// </summary>
        public int cmc;

        /// <summary>
        /// Do i own the card or not.
        /// </summary>
        public bool owned;

        /// <summary>
        /// Which category it belongs to in the deck i.e. draw, interaction, ramp, land
        /// </summary>
        public string category;
    }
}

