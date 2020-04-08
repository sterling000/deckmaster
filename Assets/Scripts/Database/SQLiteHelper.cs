//using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;
using System.Data.SQLite;

namespace PersistentStorage
{
    public class SQLiteHelper
    {
        private const string dbName = "mainDB.db";

        public string dbConnectionString;

        public IDbConnection dbConnection;

        public SQLiteHelper(string persistentDataPath)
        {
            dbConnectionString = $"URI=file:{persistentDataPath}/{dbName}";
            Debug.Log($"dbConnectionString: {dbConnectionString}");

            dbConnection = new SQLiteConnection(dbConnectionString);
            dbConnection.Open();
        }

        ~SQLiteHelper()
        {
            dbConnection.Close();
        }

        // virtual functions

        public virtual IDataReader GetDataById(int id)
        {
            Debug.Log("This function is not implemented.");
            throw null;
        }

        public virtual IDataReader GetDataByString(string str)
        {
            Debug.Log("This function is not implemented.");
            throw null;
        }

        public virtual void DeleteDataById(int id)
        {
            Debug.Log("This function is not implemented.");
            throw null;
        }

        public virtual void DeleteDataByString(string str)
        {
            Debug.Log("This function is not implemented.");
            throw null;
        }

        public virtual IDataReader GetAllData()
        {
            Debug.Log("This function is not implemented.");
            throw null;
        }

        public virtual void DeleteAllData()
        {
            Debug.Log("This function is not implemented.");
            throw null;
        }

        public virtual IDataReader GetNumberOfRows()
        {
            Debug.Log("This function is not implemented.");
            throw null;
        }

        // helper functions
        public IDbCommand GetDbCommand()
        {
            return dbConnection.CreateCommand();
        }

        public IDbTransaction GetDbTransaction()
        {
            return dbConnection.BeginTransaction();
        }

        public IDataReader GetAllData(string tableName)
        {
            IDbCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {tableName}";
            IDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public void DeleteAllData(string tableName)
        {
            IDbCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = $"DROP TABLE IF EXISTS {tableName}";
            cmd.ExecuteNonQuery();
        }

        public IDataReader GetNumOfRows(string table_name)
        {
            IDbCommand cmd = dbConnection.CreateCommand();
            cmd.CommandText = $"SELECT COALESCE(MAX(id)+1, 0) FROM {table_name}";

            IDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public void Close()
        {
            dbConnection.Close();
        }
    }
}

