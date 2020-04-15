using UniRx;
using UnityEngine;

namespace deckmaster
{
    public class CardPresenter : MonoBehaviour
    {
        private CardViewPool pool;

        [SerializeField]
        private CardView prefab;

        public int PreloadCount = 100;

        public int PreloadThreshold = 25;

        // Use this for initialization
        void Start()
        {
            //pool = new CardViewPool(prefab, this.transform);
            //pool.PreloadAsync(PreloadCount, PreloadThreshold).Subscribe();
        }

        public void Create(CardModel model)
        {
            CardView view = Instantiate(prefab);//pool.Rent();
            view.transform.SetParent(this.transform);
            view.Initialize(model);
            //view.Model = model;
        }

        public void Clear()
        {
            foreach (CardView cardView in transform.GetComponentsInChildren<CardView>())
            {
                pool.Return(cardView);
            }
        }
    }

}
