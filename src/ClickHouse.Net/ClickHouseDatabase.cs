using System;
using System.Collections.Generic;
using System.Data;
using ClickHouse.Ado;

namespace ClickHouse.Net
{
    public class ClickHouseDatabase : IClickHouseDatabase
    {
        private ClickHouseConnectionSettings _connectionSettings;
        public IDbConnection Connection { get; private set; }

        public ClickHouseDatabase(ClickHouseConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
            Connection = new ClickHouseConnection(connectionSettings);
        }

        public void ChangeConnectionSettings(ClickHouseConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
            Connection = new ClickHouseConnection(connectionSettings);
        }

        public IClickHouseDatabase OpenConnection()
        {
            Connection.Open();
            return this;
        }

        public object[][] ExecuteSelectCommand(string commandText)
        {
            var rows = new List<object[]>();
            using (var connection = new ClickHouseConnection(_connectionSettings))
            using (var command = connection.CreateCommand(commandText))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.NextResult()) // Так как кликхаус может в одном запросе выдавать данные "по кускам" и это будет отдельный набор данных.
                    {
                        var columnsNames = new List<string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            columnsNames.Add(reader.GetName(i));
                        }

                        while (reader.Read())
                        {
                            var row = new object[reader.FieldCount];
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i];
                            }

                            rows.Add(row);
                        }
                    }
                }
            }

            return rows.ToArray();
        }

        public void ExecuteInsertCommand<T>(string commandText, IEnumerable<T> bulk)
        {
            using (var connection = new ClickHouseConnection(_connectionSettings))
            using (var command = connection.CreateCommand(commandText))
            {
                connection.Open();
                command.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "bulk",
                    Value = bulk
                });
                command.ExecuteNonQuery();
            }
        }
        
        public IEnumerable<string> ReadAsStringsList(string commandText)
        {
            var result = new List<string>();
            using (var connection = new ClickHouseConnection(_connectionSettings))
            using (var command = connection.CreateCommand(commandText))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            result.Add((string)reader.GetValue(0));
                        }
                    }
                }
            }

            return result;
        }
        
        public bool ColumnExists(string tableName, string columnName)
        {
            CheckConnection();
            using (var command = Connection.CreateCommand("SELECT COUNT(*) FROM system.columns WHERE table=@table AND name=@column"))
            {
                command.AddParameter("table", tableName);
                command.AddParameter("column", columnName);
                var execResult = ExecuteScalar(command);

                return execResult;
            }
        }

        public bool TableExists(string databaseName, string tableName)
        {
            CheckConnection();
            using (var command = Connection.CreateCommand("SELECT COUNT(*) FROM system.tables WHERE database=@database AND name=@table"))
            {
                command.AddParameter("database", databaseName);
                command.AddParameter("table", tableName);
                var execResult = ExecuteScalar(command);

                return execResult;
            }
        }

        public void AddColumn(string tableName, Column column)
        {
            ExecuteNonQuery($"ALTER TABLE {tableName} ADD COLUMN {column} {column.After}");
        }

        public void CreateColumnIfNotExists(string tableName, Column column)
        {
            if (!ColumnExists(tableName, column.Name))
            {
                AddColumn(tableName, column);
            }
        }

        public void CreateTableIfNotExists(Table table)
        {
            ExecuteNonQuery($"CREATE TABLE IF NOT EXISTS {table.Name} ({string.Join(',', table.Columns)}) ENGINE = {table.Engine}");
        }

        public void CreateDBIfNotExists(string databaseName)
        {
            ExecuteNonQuery($"CREATE DATABASE IF NOT EXISTS {databaseName}");
        }

        public void CreateTableIfNotExistsAndPopulate(string tableName, string engine, string query)
        {
            ExecuteNonQuery($"CREATE TABLE IF NOT EXISTS {_connectionSettings.Database}.{tableName} ENGINE = {engine} AS {query}");
        }

        public void CreateDBIfNotExists(Database database)
        {
            CreateDBIfNotExists(database.Name);
            Connection.ChangeDatabase(database.Name);
            foreach (var table in database.Tables)
            {
                CreateTableIfNotExists(table);
            }
        }

        public void Dispose()
        {
            Connection.Close();
        }

        public bool DatabaseExists(string databaseName)
        {
            CheckConnection();
            using (var command = Connection.CreateCommand("SELECT COUNT(*) FROM system.databases WHERE name=@database"))
            {
                command.AddParameter("database", databaseName);
                var execResult = ExecuteScalar(command);
                return execResult;
            }
        }

        public void DeleteTableIfExists(string tableName)
        {
            CheckConnection();
            using (var command = Connection.CreateCommand($"DROP TABLE IF EXISTS {tableName}"))
            {
                ExecuteNonQuery(command);
            }
        }

        public void RenameTable(string currentName, string newName)
        {
            CheckConnection();
            using (var command = Connection.CreateCommand($"RENAME TABLE {currentName} TO {newName}"))
            {
                ExecuteNonQuery(command);
            }
        }

        public void ExecuteNonQuery(IDbCommand command)
        {
            command.ExecuteNonQuery();
        }

        public bool ExecuteScalar(IDbCommand command)
        {
            return (ulong?)command.ExecuteScalar() > 0;
        }

        public void ExecuteNonQuery(string commandText)
        {
            using (var command = Connection.CreateCommand(commandText))
            {
                ExecuteNonQuery(command);
            }
        }

        private void CheckConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection isn't open");
            }
        }
    }
}