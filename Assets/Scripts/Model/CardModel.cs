using System;

namespace deckmaster
{
    [Serializable]
    public class CardModel
    {
        public CardData card;
        public int quantity;
        public string category;
        public int slot;

        public Category Category
        {
            get
            {
                if (!Enum.TryParse(category, out Category result))
                {
                    result = Category.Undefined;
                }
                return result;
            }
            set
            {
                if (!Enum.TryParse(value.ToString(), out Category result))
                {
                    result = Category.Undefined;
                }
                category = result.ToString();
            }
        }
    }
}