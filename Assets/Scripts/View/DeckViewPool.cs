using UniRx.Toolkit;
using UnityEngine;

namespace deckmaster
{
    public class DeckViewPool : ObjectPool<DeckView>
    {
        private readonly DeckView prefab;
        private readonly Transform hierarchyParent;

        public DeckViewPool(
            DeckView prefab,
            Transform hierarchyParent)
        {
            this.prefab = prefab;
            this.hierarchyParent = hierarchyParent;
        }

        protected override DeckView CreateInstance()
        {
            DeckView view = GameObject.Instantiate<DeckView>(prefab);
            view.transform.SetParent(hierarchyParent);

            return view;
        }
    }
}

