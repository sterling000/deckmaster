using UniRx;
using UnityEngine;

namespace deckmaster
{
    public class DeckPresenter : MonoBehaviour
    {
        private DeckViewPool pool;

        public DeckView prefab;

        public int PreloadCount = 20;

        public int PreloadThreshold = 2;

        void Start()
        {
            pool = new DeckViewPool(prefab, this.transform);
            pool.PreloadAsync(PreloadCount, PreloadThreshold).Subscribe();
        }

        public void Create(DeckModel model)
        {
            DeckView view = pool.Rent();
            view.Model = model;
        }

        void OnDestroy()
        {
            pool.Clear();
        }

        public void Clear()
        {
            foreach (DeckView view in transform.GetComponentsInChildren<DeckView>())
            {
                view.Clear();
                pool.Return(view);
            }
        }
    }
}

