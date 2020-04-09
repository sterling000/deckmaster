namespace PersistentStorage
{
    public class DeckInfoEntry
    {
        public int Id;
        public string Name;
        public string DateUpdated; // YYYY-MM-DD HH:MM:SS

        public DeckInfoEntry(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public DeckInfoEntry(int Id, string Name, string DateUpdated)
        {
            this.Id = Id;
            this.Name = Name;
            this.DateUpdated = DateUpdated;
        }

        public static DeckInfoEntry GetFakeDeckListEntry()
        {
            return new DeckInfoEntry(0, "Test_Deck");
        }
    }
}

