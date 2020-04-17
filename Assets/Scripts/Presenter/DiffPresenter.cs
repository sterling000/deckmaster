using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UniRx;
using UnityEngine;

namespace deckmaster
{
    public class DiffPresenter : MonoBehaviour
    {
        private DeckModel lastDeckModel;

        [SerializeField]
        private TextMeshProUGUI debug;

        private CanvasGroup canvas;

        void Start()
        {
            MessageBroker.Default.Receive<DiffDeckToggledMessage>().Subscribe(OnNextDeckToggled).AddTo(this);
            canvas = GetComponent<CanvasGroup>();
        }

        private void OnNextDeckToggled(DiffDeckToggledMessage message)
        {
            if (lastDeckModel != null && message.Selected)
            {
                StringBuilder builder = new StringBuilder();
                // todo: not sure if there is a faster algorithm for this quite yet.
                List<CardModel> commonStaples = new List<CardModel>();
                List<CardModel> binderStaples = new List<CardModel>();
                foreach (CardModel cardModel in message.Model.Staples)
                {
                    if (lastDeckModel.Staples.Select(model => model.slot).Contains(cardModel.slot))
                    {
                        commonStaples.Add(cardModel);
                    }
                    else
                    {
                        binderStaples.Add(cardModel);
                    }
                }

                foreach (CardModel cardModel in commonStaples.OrderBy(model => model.slot))
                {
                    builder.AppendLine($"<color=green>{cardModel.card.oracleCard.name} {cardModel.slot}</color>");
                }
                foreach (CardModel cardModel in binderStaples.OrderBy(model => model.slot))
                {
                    builder.AppendLine($"{cardModel.card.oracleCard.name} {cardModel.slot}");
                }

                debug.text = builder.ToString();
                lastDeckModel = null;
                // todo: before we return we should clear the lastDeckModel so we don't get three selected, but there is probably a better way to do this with a stream.
                canvas.alpha = 1;
                canvas.interactable = true;
                canvas.blocksRaycasts = true;
            }
            else if(message.Selected)
            {
                lastDeckModel = message.Model;
            }
            else
            {
                lastDeckModel = null;
            }
        }
    }
}