namespace PersistentStorage
{
    public class CardEntry
    {
        public int Id;
        public string Name;
        public int DeckId;
        public string DateUpdated; // YYYY-MM-DD HH:MM:SS

        public CardEntry(int Id, string Name, int deckId)
        {
            this.Id = Id;
            this.Name = Name;
            this.DeckId = deckId;
        }

        public CardEntry(int Id, string Name, int deckId, string DateUpdated)
        {
            this.Id = Id;
            this.Name = Name;
            this.DateUpdated = DateUpdated;
            this.DeckId = deckId;
        }

        public static CardEntry GetFakeCardEntry()
        {
            return new CardEntry(0, "Test_Card", 0);
        }
    }
}
