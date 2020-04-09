using System.Collections.Generic;
using System.Data;

namespace PersistentStorage
{
    public class CardSlotDb : SQLiteHelper
    {
        private const string TABLE_NAME = "CardSlots";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_DATE_UPDATED = "date_updated"; // YYYY-MM-DD HH:MM:SS
        
        public CardSlotDb(string persistentDataPath) : base(persistentDataPath)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                              KEY_ID + " INTEGER, " +
                              KEY_NAME + " TEXT NOT NULL, " +
                              KEY_DATE_UPDATED + " DATETIME DEFAULT CURRENT_TIMESTAMP );";
            cmd.ExecuteNonQuery();
        }

        public void CreateOrUpdateData(List<CardSlotEntry> slotEntries)
        {
            using (IDbCommand cmd = GetDbCommand())
            {
                using (IDbTransaction transaction = GetDbTransaction())
                {
                    foreach (CardSlotEntry slot in slotEntries)
                    {
                        cmd.CommandText = "INSERT OR REPLACE INTO " + TABLE_NAME
                                                                    + " ( "
                                                                    + KEY_ID + ", "
                                                                    + KEY_NAME + ", "
                                                                    + KEY_DATE_UPDATED + " ) "

                                                                    + "VALUES ( "
                                                                    + slot.Id + ", "
                                                                    + "@name" + ", "
                                                                    + "CURRENT_TIMESTAMP ) ";
                        IDbDataParameter nameParameter = cmd.CreateParameter();
                        nameParameter.ParameterName = "@name";
                        nameParameter.Value = slot.Name;
                        cmd.Parameters.Add(nameParameter);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }
    }

}
