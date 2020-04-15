using System;
using System.IO;
using System.Net;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace deckmaster
{
    public class ArchidektDataProvider : DataProvider, IDisposable
    {
        private CompositeDisposable disposables = new CompositeDisposable();

        public override async UniTask GetUserModelTask(Subject<UserModel> subject)
        {
            
            var webRequest = WebRequest.Create($"https://archidekt.com/api/decks/cards/?orderBy=-createdAt&owner=Wildcard&ownerexact=true&pageSize=50") as HttpWebRequest;
            webRequest.Method = WebRequestMethods.Http.Get;

            try
            {
                using (HttpWebResponse response = await webRequest.GetResponseAsync() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        UserModel model = ParseUserModel(reader.ReadToEnd());
                        subject.OnNext(model);
                    }
                }
            }
            catch (WebException ex)
            {
                Debug.LogError(ex.Message);
                return;
            }
        }

        private void OnNextUserData(Unit obj)
        {
            Debug.Log($"OnNextUserData");
        }

        private void OnUserDataError(Exception obj)
        {
            throw obj;
        }

        private void OnUserDataComplete()
        {
            Debug.Log($"OnUserDataComplete");
        }

        public override async UniTask GetDeckModelTask(int id, ReplaySubject<DeckModel> subject)
        {
            
            var webRequest = WebRequest.Create($"https://archidekt.com/api/decks/{id}/") as HttpWebRequest;
            
            webRequest.Method = WebRequestMethods.Http.Get;

            try
            {
                using (HttpWebResponse response = await webRequest.GetResponseAsync() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        DeckModel model = ParseDeckModel(reader.ReadToEnd());
                        subject.OnNext(model);
                    }
                }
            }
            catch (WebException ex)
            {
                Debug.LogError(ex.Message);
                return;
            }
        }

        private void OnNextDeckList(Unit obj)
        {
            Debug.Log($"OnNextDeckList");
        }

        private void OnDeckListError(Exception e)
        {
            throw e;
        }

        private void OnDeckListComplete()
        {
            Debug.Log($"OnDeckListComplete");
        }

        public UserModel ParseUserModel(string decks)
        {
            return base.ParseUserModel(decks);
        }

        public DeckModel ParseDeckModel(string deck)
        {
            return base.ParseDeckModel(deck);
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}

