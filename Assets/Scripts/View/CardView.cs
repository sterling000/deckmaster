using TMPro;
using UnityEngine;

namespace deckmaster
{
    public class CardView : MonoBehaviour
    {
        public CardModel Model;
        
        public TextMeshProUGUI nameText;

        public TextMeshProUGUI slotText;

        void OnEnable()
        {
            nameText.text = Model.card.oracleCard.name;
        }
    }
}

