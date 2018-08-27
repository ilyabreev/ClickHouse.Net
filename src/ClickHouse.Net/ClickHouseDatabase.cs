using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

        public void CreateTable(Table table, CreateOptions options = null)
        {
            var commandText = _commandFormatter.CreateTable(table, options);
            Execute(cmd => { cmd.ExecuteNonQuery(); }, commandText);
        }
        
        public void CreateDatabase(Database db, CreateOptions options = null)
        {
            CreateDatabase(db.Name, options);
            foreach (var table in db.Tables)
            {
                ChangeDatabase(db.Name);
                CreateTable(table, options);
            }
        }

        public void CreateDatabase(string databaseName, CreateOptions options = null)
        {
            var commandText = _commandFormatter.CreateDatabase(databaseName, options);
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

        public void DropTable(string tableName, DropOptions options = null)
        {
            var commandText = _commandFormatter.DropTable(tableName, options);
            Execute(cmd =>
            {
                cmd.ExecuteNonQuery();
            }, commandText);
        }

        public void RenameTable(string currentName, string newName)
        {
            Execute(cmd =>
            {
                cmd.ExecuteNonQuery();
            }, $"RENAME TABLE {currentName} TO {newName}");
        }

        public object[][] ExecuteSelectCommand(string commandText, bool header = false)
        {
            var rows = new List<object[]>();
            Execute(cmd =>
            {
                var headerAdded = false;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.NextResult()) // Так как кликхаус может в одном запросе выдавать данные "по кускам" и это будет отдельный набор данных.
                    {
                        if (header && !headerAdded)
                        {
                            var columns = new List<string>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                            }

                            rows.Add(columns.ToArray());
                            headerAdded = true;
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

        public IEnumerable<T> ExecuteQueryMapping<T>(string commandText, IEnumerable<ClickHouseParameter> parameters = null, IColumnNamingConvention convention = null) where T : new()
        {
            var data = new List<T>();
            Execute(cmd =>
            {
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }
                }
                
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            var obj = new T();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var propertyName = convention?.GetPropertyName(reader.GetName(i)) ?? reader.GetName(i);
                                obj.GetType().GetProperty(propertyName)?.SetValue(obj, reader[i], null);
                            }

                            data.Add(obj);
                        }
                    }
                }
            }, commandText);
            return data;
        }

        public Part[] SystemParts(string database)
        {
            return ExecuteQueryMapping<Part>(
                "SELECT * FROM system.parts WHERE database = @database", 
                new[]
                {
                    new ClickHouseParameter
                    {
                        ParameterName = "database",
                        Value = database
                    }
                },
                new UnderscoreNamingConvention()).ToArray();
        }

        public void FreezePartition(string table, string partition)
        {
            Execute(cmd =>
            {
                cmd.AddParameter("partition", partition);
                cmd.ExecuteNonQuery();
            }, $"ALTER TABLE {table} FREEZE PARTITION @partition");
        }

        public void BackupDatabase(string database)
        {
            var parts = SystemParts(database);
            foreach (var part in parts)
            {
                FreezePartition(part.Table, part.Partition);
            }
        }

        public void BulkInsert<T>(string tableName, IEnumerable<string> columns, IEnumerable<T> bulk)
        {
            var query = _commandFormatter.BulkInsert(tableName, columns);
            Execute(cmd =>
            {
                cmd.Parameters.Add(new ClickHouseParameter
                {
                    ParameterName = "bulk",
                    Value = bulk
                });
                cmd.ExecuteNonQuery();
            }, query);
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

        private T Execute<T>(Func<ClickHouseCommand, T> body, string commandText)
        {
            T result = default(T);
            Execute(cmd =>
            {
                result = body(cmd);
            }, commandText);
            return result;
        }

        private static void AssignProperty(object item, string propertyName, object value)
        {
            var propertyInfo = item.GetType().GetProperty(propertyName);
            var propertyType = propertyInfo?.PropertyType;

            // Use most simple signature for Parse method with one argument.
            var parseMethod = propertyType?.GetMethod("Parse", new[] { typeof(string) });

            // If value is sting - assign it to property.
            if (propertyType == typeof(string))
            {
                propertyInfo.SetValue(item, value, null);
                return;
            }

            // If Type not declare Parse method.
            if (parseMethod == null)
            {
                throw new InvalidOperationException("This Type not contain Parse method");
            }

            // Casting to string guarantees correct argument for Parse method.
            var parsedValue = parseMethod.Invoke(null, new object[] { value.ToString() });
            propertyInfo.SetValue(item, parsedValue, null);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}