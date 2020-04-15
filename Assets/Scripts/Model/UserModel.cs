using System;

namespace deckmaster
{
    [Serializable]
    public class UserModel
    {
        // the number of public decks for my user.
        public int count;

        public UserDeckData[] results;

        public UserModel()
        {
            results = Array.Empty<UserDeckData>();
        }
    }
}

