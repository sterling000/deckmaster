using System;
using UniRx;

namespace deckmaster
{
    [Serializable]
    public class CardModel
    {
        public CardData card;
        public int quantity;
        public string category;

        public Category Category
        {
            get
            {
                Category result = Category.Undefined;
                Category.TryParse(category, out result);
                return result;
            }
        }
    }
}