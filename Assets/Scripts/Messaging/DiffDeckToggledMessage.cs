namespace deckmaster
{
    public class DiffDeckToggledMessage
    {
        public DeckModel Model;
        public bool Selected;

        public DiffDeckToggledMessage(DeckModel model, bool selected)
        {
            Model = model;
            Selected = selected;
        }
    }
}