using System.Data;

namespace ozgurtek.framework.common.Data
{
    public abstract class GdAbstractDbDataSource
    {
        public virtual int ExecuteNonQuery(string commandText)
        {
            using (IDbConnection connection = GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    return command.ExecuteNonQuery();
                }
            }
        }

        public virtual object ExecuteScalar(string commandText)
        {
            using (IDbConnection connection = GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    return command.ExecuteScalar();
                }
            }
        }

        public virtual DataTable ExecuteTable(string commandText)
        {
            using (IDbConnection connection = GetConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    using (IDataReader dataReader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(dataReader);
                        return dataTable;
                    }
                }
            }
        }

        public override string ToString()
        {
            return ConnectionString;
        }

        public abstract string ConnectionString { get; }
        public abstract IDbConnection GetConnection();
        public abstract string Name { get; }
    }
}