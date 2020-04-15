using UniRx.Toolkit;
using UnityEngine;

namespace deckmaster
{
    public class CardViewPool : ObjectPool<CardView>
    {
        private readonly CardView prefab;

        private readonly Transform hierarchyParent;

        public CardViewPool(CardView prefab, Transform hierarchyParent)
        {
            this.prefab = prefab;
            this.hierarchyParent = hierarchyParent;
        }

        protected override CardView CreateInstance()
        {
            CardView view = GameObject.Instantiate<CardView>(prefab);
            view.transform.SetParent(hierarchyParent);

            return view;
        }
    }
}

