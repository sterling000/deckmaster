using UnityEngine;
using System.Collections;

namespace PersistentStorage
{
    public class DeckListEntry
    {
        public string Id;
        public string Name;
        public string DateCreated; // YYYY-MM-DD HH:MM:SS
        public string DateUpdated; // YYYY-MM-DD HH:MM:SS

        public DeckListEntry(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public DeckListEntry(string Id, string Name, string DateCreated, string DateUpdated)
        {
            this.Id = Id;
            this.Name = Name;
            this.DateCreated = DateCreated;
            this.DateUpdated = DateUpdated;
        }

        public static DeckListEntry GetFakeDeckListEntry()
        {
            return new DeckListEntry("0", "Test_Deck");
        }
    }
}

