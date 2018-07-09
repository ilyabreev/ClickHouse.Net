using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClickHouse.Ado;
using ClickHouse.Net.Entities;

namespace ClickHouse.Net
{
    public class ClickHouseDatabase : IClickHouseDatabase, IDisposable
    {
        private ClickHouseConnectionSettings _connectionSettings;
        private readonly IClickHouseCommandFormatter _commandFormatter;
        private readonly IClickHouseConnectionFactory _connectionFactory;
        private readonly IClickHouseQueryLogger _queryLogger;
        private ClickHouseConnection _connection;
        private bool _ownsConnection;

        public ClickHouseDatabase(
            ClickHouseConnectionSettings connectionSettings,
            IClickHouseCommandFormatter commandFormatter, 
            IClickHouseConnectionFactory connectionFactory,
            IClickHouseQueryLogger queryLogger)
        {
            _connectionSettings = connectionSettings;
            _commandFormatter = commandFormatter;
            _connectionFactory = connectionFactory;
            _queryLogger = queryLogger;
        }

        public void ChangeConnectionSettings(ClickHouseConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
        }

        public void Open()
        {
            if (_ownsConnection)
            {
                Close();
            }

            _connection = _connectionFactory.CreateConnection(_connectionSettings);
            _connection.Open();
            _ownsConnection = true;
        }

        public void Close()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Can't close connections that is not opened.");
            }

            _connection.Close();
            _ownsConnection = false;
        }

        public void ChangeDatabase(string dbName)
        {
            if (!_ownsConnection)
            {
                throw new InvalidOperationException(
                    "There is no effect in changing database on connection that is not owned by current instance of `ClickHouseDatabase`.");
            }

            _connection.ChangeDatabase(dbName);
        }

        public IEnumerable<string> ReadAsStringsList(string commandText)
        {
            return Execute(cmd =>
            {
                var result = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            result.Add((string) reader.GetValue(0));
                        }
                    }
                }

                return result;
            }, commandText);
        }
        
        public bool TableExists(string tableName)
        {
            return Execute(cmd =>
            {
                cmd.AddParameter("database", _connectionSettings.Database);
                cmd.AddParameter("table", tableName);
                return ExecuteExists(cmd);
            }, "SELECT COUNT(*) FROM system.tables WHERE database=@database AND name=@table");
        }

        public void CreateTable(Table table, bool ifNotExists = true)
        {
            CreateTable(table.Name, table.Columns.Select(c => c.ToString()), table.Engine, ifNotExists);
        }

        public void CreateTable(string tableName, IEnumerable<string> columns, string engine, bool ifNotExists = true)
        {
            Execute(cmd =>
            {
                cmd.ExecuteNonQuery();
            }, $"CREATE TABLE IF NOT EXISTS {tableName} ({string.Join(',', columns)}) ENGINE = {engine}");
        }

        public void CreateTableIfNotExistsAndPopulate(string tableName, string engine, string query)
        {
            Execute(cmd =>
            {
                cmd.ExecuteNonQuery();
            }, $"CREATE TABLE IF NOT EXISTS {_connectionSettings.Database}.{tableName} ENGINE = {engine} AS {query}");
        }

        public void CreateDatabase(Database db, bool ifNotExists = true)
        {
            CreateDatabase(db.Name, ifNotExists);
            foreach (var table in db.Tables)
            {
                ChangeDatabase(db.Name);
                CreateTable(table, ifNotExists);
            }
        }

        public void CreateDatabase(string databaseName, bool ifNotExists = true)
        {
            var commandText = _commandFormatter.CreateDatabase(databaseName, ifNotExists);
            Execute(cmd => { cmd.ExecuteNonQuery(); }, commandText);
        }
        
        public bool DatabaseExists(string databaseName)
        {
            return Execute(cmd =>
            {
                cmd.AddParameter("database", databaseName);
                return ExecuteExists(cmd);
            },
            "SELECT COUNT(*) FROM system.databases WHERE name=@database");
        }

        public void DeleteTableIfExists(string tableName)
        {
            Execute(cmd =>
            {
                cmd.ExecuteNonQuery();
            }, $"DROP TABLE IF EXISTS {tableName}");
        }

        public void RenameTable(string currentName, string newName)
        {
            Execute(cmd =>
            {
                cmd.ExecuteNonQuery();
            }, $"RENAME TABLE {currentName} TO {newName}");
        }

        public object[][] ExecuteSelectCommand(string commandText)
        {
            var rows = new List<object[]>();
            Execute(cmd =>
            {
                using (var reader = cmd.ExecuteReader())
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
            }, commandText);
            return rows.ToArray();
        }

        public void BulkInsert<T>(string tableName, IEnumerable<string> columns, IEnumerable<T> bulk)
        {
            var cmd = _commandFormatter.CreateInsertCommandText(tableName, columns);
            ExecuteBulkInsertCommand(cmd, bulk);
        }

        public void ExecuteNonQuery(string commandText)
        {
            Execute(cmd => { cmd.ExecuteNonQuery(); }, commandText);
        }

        public bool ExecuteExists(IDbCommand command)
        {
            return (ulong?) command.ExecuteScalar() > 0;
        }

        public void Execute(Action<ClickHouseCommand> body, string commandText)
        {
            _queryLogger?.BeforeQuery();
            if (_ownsConnection)
            {
                using (var command = _connection.CreateCommand(commandText))
                {
                    body(command);
                }
            }
            else
            {
                using (var connection = _connectionFactory.CreateConnection(_connectionSettings))
                using (var command = connection.CreateCommand(commandText))
                {
                    connection.Open();
                    body(command);
                }
            }

            _queryLogger?.AfterQuery(commandText);
        }

        private void ExecuteBulkInsertCommand<T>(string commandText, IEnumerable<T> bulk)
        {
            Execute(cmd =>
            {
                cmd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "bulk",
                    Value = bulk
                });
                cmd.ExecuteNonQuery();
            }, commandText);
        }

        private T Execute<T>(Func<ClickHouseCommand, T> body, string commandText)
        {
            T result = default(T);
            Execute(cmd =>
            {
                result = body(cmd);
            }, commandText);
            return result;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}