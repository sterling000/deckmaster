using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace PersistentStorage
{
    public class DeckCardDb : SQLiteHelper
    {
        private const string TABLE_NAME = "DeckCards";
        private const string KEY_ID = "id";
        private const string KEY_DECK = "deck";
        private const string KEY_NAME = "name";
        private const string KEY_DATE_UPDATED = "date_updated"; // YYYY-MM-DD HH:MM:SS

        public DeckCardDb(string persistentDataPath) : base(persistentDataPath)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "DROP TABLE IF EXISTS " + TABLE_NAME + "; " + 
                              "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                              KEY_ID + " INTEGER, " +
                              KEY_DECK + " INTEGER REFERENCES DeckInfo(id), " +
                              KEY_NAME + " TEXT NOT NULL, " +
                              KEY_DATE_UPDATED + " DATETIME DEFAULT CURRENT_TIMESTAMP );";
            cmd.ExecuteNonQuery();
        }

        public void CreateOrUpdateData(List<CardEntry> cardEntries)
        {
            using (IDbCommand cmd = GetDbCommand())
            {
                using (IDbTransaction transaction = GetDbTransaction())
                {
                    foreach (CardEntry card in cardEntries)
                    {
                        cmd.CommandText = "INSERT OR REPLACE INTO " + TABLE_NAME
                                                                    + " ( "
                                                                    + KEY_ID + ", "
                                                                    + KEY_DECK + ", "
                                                                    + KEY_NAME + ", "
                                                                    + KEY_DATE_UPDATED + " ) "

                                                                    + "VALUES ( "
                                                                    + card.Id + ", "
                                                                    + card.DeckId + ", "
                                                                    + "@name" + ", "
                                                                    + "CURRENT_TIMESTAMP ) ";
                        IDbDataParameter nameParameter = cmd.CreateParameter();
                        nameParameter.ParameterName = "@name";
                        nameParameter.Value = card.Name;
                        cmd.Parameters.Add(nameParameter);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public void CreateOrUpdateData(CardEntry card)
        {
            using (IDbCommand cmd = GetDbCommand())
            {
                cmd.CommandText = "INSERT OR REPLACE INTO " + TABLE_NAME
                                                            + " ( "
                                                            + KEY_ID + ", "
                                                            + KEY_DECK + ", "
                                                            + KEY_DATE_UPDATED + " ) "

                                                            + "VALUES ( "
                                                            + card.Id + ", "
                                                            + card.DeckId + ", "
                                                            + "CURRENT_TIMESTAMP ); ";
                cmd.ExecuteNonQuery(); 
            }  
        }

        public Dictionary<int, int> QueryStaples()
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "WITH counted AS " +
                                "(SELECT deck, " +
                                    "(SELECT count(*) " +
                                    "FROM " + TABLE_NAME + " AS d2 " +
                                    "WHERE d.id = d2.id) as cnt " +
                                "FROM " + TABLE_NAME + " AS d) " +
                              "SELECT deck, sum(cnt > 1) AS Duplicates " +
                              "FROM counted " +
                              "GROUP BY deck " +
                              "ORDER BY Duplicates DESC;";

            IDataReader reader = cmd.ExecuteReader();
            Dictionary<int, int> results = new Dictionary<int, int>();
            while (reader.Read())
            {
                results.Add(reader.GetInt32(0), reader.GetInt32(1));
            }
            reader.Close();
            return results;
        }

        public List<int> QueryDeckStaples(int deckListId)
        {
            List<int> results = new List<int>();
            using (IDbCommand cmd = GetDbCommand())
            {
                cmd.CommandText = "SELECT d1." + KEY_ID + ", " +
                                  "(SELECT COUNT(*) from DeckCards d2 WHERE d2." + KEY_ID + " = " + "d1." + KEY_ID + ") as idcounter" +
                                  " FROM  " + TABLE_NAME + " d1 " +
                                  "WHERE idcounter > 1 and " + KEY_DECK + " = " + deckListId + ";";

                IDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    results.Add(reader.GetInt32(0));
                }
                reader.Close();
            }

            return results;
        }
    }
}

