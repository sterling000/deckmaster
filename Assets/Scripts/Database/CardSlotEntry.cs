namespace PersistentStorage
{
    public class CardSlotEntry
    {
        public int Id;
        public string Name;
        public string DateUpdated; // YYYY-MM-DD HH:MM:SS

        public CardSlotEntry(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public CardSlotEntry(int Id, string Name, string DateUpdated)
        {
            this.Id = Id;
            this.Name = Name;
            this.DateUpdated = DateUpdated;
        }

        public static CardSlotEntry GetFakeSlotEntry()
        {
            return new CardSlotEntry(0, "Test_Slot");
        }
    }
}

