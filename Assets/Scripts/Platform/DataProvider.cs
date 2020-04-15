using UniRx;
using UniRx.Async;
using UnityEngine;

namespace deckmaster
{
    public abstract class DataProvider
    {
        public abstract void GetUserModelThreaded(Subject<UserModel> subject);
        public abstract UniTaskVoid GetUserModelTask(Subject<UserModel> subject);
        public abstract void GetDeckModel(int id, ReplaySubject<DeckModel> subject);
        protected DeckModel ParseDeckModel(string deck)
        {
            return JsonUtility.FromJson<DeckModel>(deck);
        }

       
        protected UserModel ParseUserModel(string decks)
        {
            return JsonUtility.FromJson<UserModel>(decks);
        }
    }
}

