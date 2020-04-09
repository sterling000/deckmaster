﻿using System.Collections.Generic;
using System.Data;

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

        public string GetNameById(int id)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = " + $"{id};";
            IDataReader reader = cmd.ExecuteReader();
            int ordinal = reader.GetOrdinal("name");
            string name = string.Empty;
            while (reader.Read())
            {
                name = reader.GetString(ordinal);
            }

            return name;
        }

        public Dictionary<int, int> QueryStaples()
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "SELECT " + KEY_DECK + ", "
                              + "SUM(case when cnt > 1 then 1 else 0 end) cnt "
                              + "FROM ( "
                              + "SELECT " + KEY_DECK + ", "
                              + "(SELECT count(*) from " + TABLE_NAME + " t1 where t1." + KEY_ID + " = t." + KEY_ID +
                              ") cnt "
                              + "FROM " + TABLE_NAME + " t "
                              + ") t "
                              + "GROUP BY " + KEY_DECK + ";";

                              // the following query uses window functions, not available until SQLite 3.25
            // cmd.CommandText = "WITH counted AS " +
            //                     "(SELECT deck, " +
            //                         "(SELECT count(*) " +
            //                         "FROM " + TABLE_NAME + " AS d2 " +
            //                         "WHERE d.id = d2.id) as cnt " +
            //                     "FROM " + TABLE_NAME + " AS d) " +
            //                   "SELECT deck, sum(cnt > 1) AS Duplicates " +
            //                   "FROM counted " +
            //                   "GROUP BY deck " +
            //                   "ORDER BY Duplicates DESC;";

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

        public List<int> QuerySlotList()
        {
            List<int> results = new List<int>();
            using (IDbCommand cmd = GetDbCommand())
            {
                cmd.CommandText = "SELECT d1." + KEY_ID + ", d1." + KEY_NAME + ", " +
                                  "(SELECT COUNT(*) from DeckCards d2 WHERE d2." + KEY_ID + " = " + "d1." + KEY_ID + ") as idcounter" +
                                  " FROM  " + TABLE_NAME + " d1 " +
                                  "WHERE idcounter > 1 GROUP BY " + KEY_ID + " ORDER BY idcounter desc;";

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

