using TMPro;
using UnityEngine;

namespace deckmaster
{
    public class DeckView : MonoBehaviour
    {
        public TextMeshProUGUI Name;

        public CardView cardView;
        public RectTransform contentRectTransform;

        public void AddCard(CardModel card)
        {
            CardView view = Instantiate(cardView);
            view.nameText.text = card.card.oracleCard.name;
            view.transform.SetParent(contentRectTransform);
        }
    }
}