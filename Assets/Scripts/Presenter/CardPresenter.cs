using UnityEngine;

namespace deckmaster
{
    public class CardPresenter : MonoBehaviour
    {
        [SerializeField]
        private CardView prefab;

        public void Create(CardModel model)
        {
            CardView view = Instantiate(prefab);//pool.Rent();
            view.transform.SetParent(this.transform);
            view.Initialize(model);
        }

        public void Clear()
        {
            foreach (CardView cardView in transform.GetComponentsInChildren<CardView>())
            {
                Destroy(cardView.gameObject);
            }
        }
    }

}
