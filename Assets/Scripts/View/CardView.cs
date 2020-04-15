using TMPro;
using UniRx;
using UnityEngine;

namespace deckmaster
{
    public class CardView : MonoBehaviour
    {
        public CardModel Model;
        
        public TextMeshProUGUI nameText;

        public TextMeshProUGUI slotText;

        void Start()
        {
            Model.card.oracleCard.ObserveEveryValueChanged(card => card.name).SubscribeToText(nameText).AddTo(this);
            Model.ObserveEveryValueChanged(model => model.slot).Select(i => i.ToString()).SubscribeToText(slotText).AddTo(this);
        }
        public void Initialize(CardModel model)
        {
            Model = model;
            Model.card.oracleCard.ObserveEveryValueChanged(card => card.name).SubscribeToText(nameText).AddTo(this);
            Model.ObserveEveryValueChanged(card => card.slot).Select(i => i.ToString()).SubscribeToText(slotText).AddTo(this);
        }
    }
}

