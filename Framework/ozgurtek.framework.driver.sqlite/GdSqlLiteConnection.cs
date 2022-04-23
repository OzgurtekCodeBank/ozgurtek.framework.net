using ozgurtek.framework.core.Data;
using ozgurtek.framework.driver.sqlite.Extension;
using SQLite;
using System;
using System.Collections.Generic;

namespace ozgurtek.framework.driver.sqlite
{
    internal class GdSqlLiteConnection
    {
        private readonly string _connectionString;
        private SQLiteConnection _connection;

        public GdSqlLiteConnection(string source)
        {
            _connectionString = source;
        }

        public void BeginTransaction()
        {
            if (_connection != null)
                throw new Exception("Transaction Already Exists");

            _connection = new SQLiteConnection(_connectionString, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_connection == null)
                throw new Exception("Transaction not exists, use beginTransaction");

            _connection.Commit();
            _connection.Dispose();
            _connection = null;
        }

        public void RollbackTransaction()
        {
            if (_connection == null)
                throw new Exception("Transaction not exists, use beginTransaction");

            _connection.Rollback();
            _connection.Dispose();
            _connection = null;
        }

        public int ExecuteNonQuery(string commandText)
        {
            if (_connection != null)
            {
                SQLiteCommand command = new SQLiteCommand(_connection);
                command.CommandText = commandText;
                return command.ExecuteNonQuery();
            }

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache))
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = commandText;
                return command.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string commandText, object[] values)
        {
            if (_connection != null)
            {
                SQLiteCommand command = _connection.CreateCommand(commandText, values);
                return command.ExecuteNonQuery();
            }

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache))
            {
                SQLiteCommand command = connection.CreateCommand(commandText, values);
                return command.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(string commandText)
        {
            if (_connection != null)
            {
                SQLiteCommand command = new SQLiteCommand(_connection);
                command.CommandText = commandText;
                return command.ExecuteScalar<T>();
            }

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = commandText;
                return command.ExecuteScalar<T>();
            }
        }

        public IEnumerable<GdSqliteRow> ExecuteReader(string commandText, IGdTable table = null)
        {
            if (_connection != null)
            {
                IEnumerable<ReaderItem> executeReader = _connection.ExecuteReader(commandText);
                foreach (ReaderItem readerItem in executeReader)
                {
                    GdSqliteRow buffer = new GdSqliteRow();
                    buffer.Table = table;
                    foreach (string fieldName in readerItem.Fields)
                    {
                        object obj = readerItem[fieldName];
                        buffer.Put(fieldName, obj);
                    }
                    yield return buffer;
                }
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(_connectionString, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache))
                {
                    IEnumerable<ReaderItem> executeReader = connection.ExecuteReader(commandText);
                    foreach (ReaderItem readerItem in executeReader)
                    {
                        GdSqliteRow buffer = new GdSqliteRow();
                        buffer.Table = table;
                        foreach (string fieldName in readerItem.Fields)
                        {
                            object obj = readerItem[fieldName];
                            buffer.Put(fieldName, obj);
                        }
                        yield return buffer;
                    }
                }
            }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }
    }
}