using System.Data;
using UnityEngine;

namespace PersistentStorage
{
    public class DeckListDb : SQLiteHelper
    {
        private const string TABLE_NAME = "DeckLists";
        private const string KEY_ID = "id";
        private const string KEY_NAME = "name";
        private const string KEY_DATE_CREATED = "date_created"; // YYYY-MM-DD HH:MM:SS
        private const string KEY_DATE_UPDATED = "date_updated"; // YYYY-MM-DD HH:MM:SS
        private string[] COLUMNS = new[] {KEY_ID, KEY_NAME, KEY_DATE_CREATED, KEY_DATE_UPDATED};

        public DeckListDb(string persistentDataPath) : base(persistentDataPath)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
                              KEY_ID + " TEXT PRIMARY KEY, " +
                              KEY_NAME + " TEXT, " +
                              KEY_DATE_CREATED + " DATETIME DEFAULT CURRENT_TIMESTAMP, " +
                              KEY_DATE_UPDATED + " DATETIME DEFAULT CURRENT_TIMESTAMP ) ";
            cmd.ExecuteNonQuery();
        }

        public void AddData(DeckListEntry deckList)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "INSERT INTO " + TABLE_NAME
                                             + " ( "
                                             + KEY_ID + ", "
                                             + KEY_NAME + " ) "

                                             + "VALUES ( '"
                                             + deckList.Id + "', '"
                                             + deckList.Name + "' ) "; // should parse out apostrophies from the deckname so we don't break our sql
            cmd.ExecuteNonQuery();
        }

        public void CreateOrUpdateData(DeckListEntry deckList)
        {
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "INSERT OR REPLACE INTO " + TABLE_NAME
                                                        + " ( "
                                                        + KEY_ID + ", "
                                                        + KEY_NAME + ", "
                                                        + KEY_DATE_CREATED + ", "
                                                        + KEY_DATE_UPDATED + " ) "

                                                        + "VALUES ( '"
                                                        + deckList.Id + "', '"
                                                        + deckList.Name + "', " // should parse out apostrophies from the deckname so we don't break our sql
                                                        + "( SELECT " + KEY_DATE_CREATED + " FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = " + deckList.Id + ") , " // this should leave the created timestamp unchanged.
                                                        + "CURRENT_TIMESTAMP ) ";
            cmd.ExecuteNonQuery();
        }

        public override IDataReader GetDataById(int id)
        {
            return base.GetDataById(id);
        }

        public override IDataReader GetDataByString(string str)
        {
            Debug.Log($"Getting DeckList: {str}");
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + str + "'";
            return cmd.ExecuteReader();
        }

        public override void DeleteDataById(int id)
        {
            base.DeleteDataById(id);
        }

        public override void DeleteDataByString(string str)
        {
            Debug.Log($"Deleting DeckList: {str}");
            IDbCommand cmd = GetDbCommand();
            cmd.CommandText = "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + str + "'";
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

