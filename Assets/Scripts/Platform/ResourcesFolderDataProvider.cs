using System;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace deckmaster
{
    public class ResourcesFolderDataProvider : DataProvider, IDisposable
    {
        CompositeDisposable disposables = new CompositeDisposable();

        public override async UniTask GetUserModelTask(ReplaySubject<UserModel> subject)
        {
            TextAsset[] textAssets = await UniTask.Run(() => Resources.LoadAll<TextAsset>(""));

            UserModel model = new UserModel();
            model.count = textAssets.Length;
            model.results = new UserDeckData[textAssets.Length];
            for (var i = 0; i < textAssets.Length; i++)
            {
                TextAsset asset = textAssets[i];
                DeckModel deckModel = JsonUtility.FromJson<DeckModel>(asset.text);
                model.results[i] = new UserDeckData()
                {
                    id = deckModel.id,
                    name = deckModel.name
                };
            }

            Debug.Log("OnUserModelComplete");
            subject.OnNext(model);
        }

        public override async UniTask GetDeckModelTask(int id, ReplaySubject<DeckModel> subject)
        {
            TextAsset asset = Resources.Load<TextAsset>($"{id}");
            DeckModel deckModel = ParseDeckModel(asset.text);

            Debug.Log("OnGetDeckModelComplete");
            subject.OnNext(deckModel);
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}

