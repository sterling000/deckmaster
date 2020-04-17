using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace deckmaster
{
    public class DeckView : MonoBehaviour
    {
        public DeckModel Model;
        
        public TextMeshProUGUI Name;
        public TextMeshProUGUI StaplesCount;

        [SerializeField]
        private CardPresenter presenter;

        public Toggle diff;

        void Start()
        {
            Model.ObserveEveryValueChanged(model => model.name).SubscribeToText(Name).AddTo(this);
            foreach (CardModel staple in Model.Staples.OrderBy(model => model.slot))
            {
                OnNextCard(staple);
            }
            StaplesCount.text = Model.Staples.Count.ToString();
            diff.OnValueChangedAsObservable().Skip(1).Subscribe(OnNextDiffToggle).AddTo(this);
            presenter.gameObject.SetActive(false);
        }

        private void OnNextDiffToggle(bool selected)
        {
            MessageBroker.Default.Publish(new DiffDeckToggledMessage(Model, selected));
        }

        private void OnNextCard(CardModel card)
        {
            presenter.Create(card);
        }

        public void Clear()
        {
            presenter.Clear();
        }
    }
}