using TMPro;
using UniRx;
using UnityEngine;

namespace deckmaster
{
    public class DeckView : MonoBehaviour
    {
        public TextMeshProUGUI Name;
        public TextMeshProUGUI StaplesCount;
        public CardView cardView;
        public RectTransform contentRectTransform;

        public void AddCard(CardModel card, int slot)
        {
            CardView view = Instantiate(cardView);
            view.nameText.text = card.card.oracleCard.name;
            view.transform.SetParent(contentRectTransform);
            view.index.text = contentRectTransform.childCount.ToString();
            view.slotText.text = slot.ToString();
        }
    }
}