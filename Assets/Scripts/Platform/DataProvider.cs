using System;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace deckmaster
{
    public abstract class DataProvider
    {
        public abstract UniTask GetUserModelTask(Subject<UserModel> subject);
        public abstract UniTask GetDeckModelTask(int id, ReplaySubject<DeckModel> subject);
        
        protected DeckModel ParseDeckModel(string deck)
        {
            DeckModel deckModel = JsonUtility.FromJson<DeckModel>(deck);
            foreach (CardModel cardModel in deckModel.cards)
            {
                if (cardModel.Category == Category.Undefined)
                {
                    // try to parse the category from the card type
                    Enum.TryParse(cardModel.card.oracleCard.types[0], out Category category);
                    cardModel.Category = category;
                }
            }
            return deckModel;
        }

        protected UserModel ParseUserModel(string decks)
        {
            return JsonUtility.FromJson<UserModel>(decks);
        }
    }
}

