using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace PersistentStorage
{
    public class DeckInfoDb : SQLiteHelper
    {
        private const string TABLE_NAME = "DeckInfo";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_DATE_UPDATED = "date_updated"; // YYYY-MM-DD HH:MM:SS

        public DeckInfoDb(string persistentDataPath) : base(persistentDataPath)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                              KEY_ID + " INTEGER PRIMARY KEY, " +
                              KEY_NAME + " TEXT, " +
                              KEY_DATE_UPDATED + " DATETIME DEFAULT CURRENT_TIMESTAMP ) ";
            cmd.ExecuteNonQuery();
        }

        public void AddData(DeckInfoEntry deckInfo)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "INSERT INTO " + TABLE_NAME
                                             + " ( "
                                             + KEY_ID + ", "
                                             + KEY_NAME + " ) "

                                             + "VALUES ( "
                                             + deckInfo.Id + ", @name ) "; 

            IDbDataParameter nameParameter = cmd.CreateParameter();
            nameParameter.ParameterName = "@name";
            nameParameter.Value = deckInfo.Name;
            cmd.Parameters.Add(nameParameter);
            cmd.ExecuteNonQuery();
        }

        public void CreateOrUpdateData(List<DeckInfoEntry> deckInfos)
        {
            // not used currently because we fetch the decklists individually instead of in a batch.
            using (IDbCommand cmd = GetDbCommand())
            {
                using (IDbTransaction transaction = GetDbTransaction())
                {
                    // try catch before commit?
                    foreach (DeckInfoEntry deckInfo in deckInfos)
                    {
                        cmd.CommandText = "INSERT OR REPLACE INTO " + TABLE_NAME
                                                                    + " ( "
                                                                    + KEY_ID + ", "
                                                                    + KEY_NAME + ", "
                                                                    + KEY_DATE_UPDATED + " ) "

                                                                    + "VALUES ( "
                                                                    + deckInfo.Id + ", @name, "
                                                                    + "CURRENT_TIMESTAMP ) ";

                        IDbDataParameter nameParameter = cmd.CreateParameter();
                        nameParameter.ParameterName = "@name";
                        nameParameter.Value = deckInfo.Name;
                        cmd.Parameters.Add(nameParameter);

                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public void CreateOrUpdateData(DeckInfoEntry deckInfo)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "INSERT OR REPLACE INTO " + TABLE_NAME
                                                        + " ( "
                                                        + KEY_ID + ", "
                                                        + KEY_NAME + ", "
                                                        + KEY_DATE_UPDATED + " ) "

                                                        + "VALUES ( "
                                                        + deckInfo.Id + ", @name, "
                                                        + "CURRENT_TIMESTAMP ) ";
            
            IDbDataParameter nameParameter = cmd.CreateParameter();
            nameParameter.ParameterName = "@name";
            nameParameter.Value = deckInfo.Name;
            cmd.Parameters.Add(nameParameter);

            cmd.ExecuteNonQuery();
        }
        
        public override IDataReader GetDataByString(string str)
        {
            Debug.Log($"Getting DeckList: {str}");
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = " + str;
            return cmd.ExecuteReader();
        }
        
        public override void DeleteDataByString(string str)
        {
            Debug.Log($"Deleting DeckList: {str}");
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = " + str + "";
            cmd.ExecuteReader();
        }

        public override void DeleteAllData()
        {
            base.DeleteAllData(TABLE_NAME);
        }

        public override IDataReader GetAllData()
        {
            return base.GetAllData(TABLE_NAME);
        }

        public IDataReader GetLatestTimeStamp()
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "SELECT * FROM " + TABLE_NAME + " ORDER BY " + KEY_DATE_UPDATED + " DESC LIMIT 1";
            return cmd.ExecuteReader();
        }
    }
}

