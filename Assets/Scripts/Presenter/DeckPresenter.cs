using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace deckmaster
{
    public class DeckPresenter : MonoBehaviour
    {
        private DeckViewPool pool;

        public DeckView prefab;

        public int PreloadCount = 20;

        public int PreloadThreshold = 2;

        [SerializeField]
        private ToggleGroup diffDeckToggles;

        void Start()
        {
            pool = new DeckViewPool(prefab, this.transform);
            pool.PreloadAsync(PreloadCount, PreloadThreshold).Subscribe();
        }

        public void Create(DeckModel model)
        {
            DeckView view = pool.Rent();
            view.Model = model;
            diffDeckToggles.RegisterToggle(view.diff);
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

