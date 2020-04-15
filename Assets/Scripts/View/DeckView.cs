using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

namespace deckmaster
{
    public class DeckView : MonoBehaviour
    {
        public DeckModel Model;
        

        public TextMeshProUGUI Name;
        public TextMeshProUGUI StaplesCount;

        [SerializeField]
        private CardPresenter presenter;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        void OnEnable()
        {
            Model.ObserveEveryValueChanged(model => model.name).SubscribeToText(Name).AddTo(disposables);
            Model.cards.ToReactiveCollection().Select(cardModel => cardModel).ToObservable()
                .Where((cardModel, i) => Model.Staples.Contains(cardModel.card.id))
                .Subscribe(OnNextCard).AddTo(disposables);
            StaplesCount.text = Model.Staples.Count.ToString();
            presenter.gameObject.SetActive(false);
        }

        private void OnNextCard(CardModel card)
        {
            presenter.gameObject.SetActive(true);
            presenter.Create(card);
        }

        void OnDisable()
        {
            presenter.gameObject.SetActive(true);
            disposables.Dispose();
        }
    }
}