using System.Collections.Generic;
using System.Data;
using ClickHouse.Ado;
using ClickHouse.Net.Entities;

namespace ClickHouse.Net
{
    /// <summary>
    /// Интерфес управления структурой базы данных
    /// </summary>
    public interface IClickHouseDatabase
    {
        /// <summary>
        /// Create a new database
        /// </summary>
        /// <param name="db">Object representing database structure</param>
        /// <param name="ifNotExists">Try to create a database only if it does not exist</param>
        void CreateDatabase(Database db, bool ifNotExists = true);

        /// <summary>
        /// Проверяет наличие базы данных и, в случае отсутствия, создает ее
        /// </summary>
        /// <param name="databaseName"></param>
        void CreateDatabase(string databaseName, bool ifNotExists = true);
        
        /// <summary>
        /// Проверяет существование базы данных
        /// </summary>
        /// <param name="databaseName"></param>
        bool DatabaseExists(string databaseName);
        
        /// <summary>
        /// Check if table exists in current database
        /// </summary>
        /// <param name="tableName">Table name</param>
        bool TableExists(string tableName);

        /// <summary>
        /// Удалить таблицу если она существует
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        void DeleteTableIfExists(string tableName);

        /// <summary>
        /// Создать таблице и заполнить ее данными по запросу query
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <param name="engine">Движок</param>
        /// <param name="query">Запрос для заполнения данных таблицы</param>
        void CreateTableIfNotExistsAndPopulate(string tableName, string engine, string query);

        /// <summary>
        /// Переименовать таблицы
        /// </summary>
        /// <param name="currentName">Текущее название</param>
        /// <param name="newName">Новое название</param>
        void RenameTable(string currentName, string newName);
        
        /// <summary>
        /// Выполнить запрос, который возвращает число 
        /// </summary>
        /// <param name="command">запрос</param>
        /// <returns></returns>
        bool ExecuteExists(IDbCommand command);

        /// <summary>
        /// Выборка строк из БД
        /// </summary>
        /// <param name="commandText">запрос</param>
        /// <returns></returns>
        object[][] ExecuteSelectCommand(string commandText);


        IEnumerable<string> ReadAsStringsList(string commandText);

        /// <summary>
        /// Change current connection settings for database. It doesn't affect already opened connection.
        /// </summary>
        /// <param name="connectionSettings"></param>
        void ChangeConnectionSettings(ClickHouseConnectionSettings connectionSettings);

        /// <summary>
        /// Open a database connection in order to execute multiple commands inside one connection
        /// </summary>
        void Open();

        /// <summary>
        /// Close a current database connection
        /// </summary>
        void Close();

        /// <summary>
        /// Change current database if there is owned connection opened
        /// </summary>
        /// <param name="dbName"></param>
        void ChangeDatabase(string dbName);

        /// <summary>
        /// Execute a query that produces no results
        /// </summary>
        /// <param name="commandText">Text of query</param>
        void ExecuteNonQuery(string commandText);

        /// <summary>
        /// Create a new table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="columns">Collection of column definitions</param>
        /// <param name="engine">Table engine</param>
        /// <param name="ifNotExists">Try to create table only if it does not exist</param>
        void CreateTable(string tableName, IEnumerable<string> columns, string engine, bool ifNotExists = true);

        /// <summary>
        /// Create a new table
        /// </summary>
        /// <param name="table">Object representing table structure</param>
        /// <param name="ifNotExists">Try to create table only if it does not exist</param>
        void CreateTable(Table table, bool ifNotExists = true);

        /// <summary>
        /// Bulk insert rows into table
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="tableName">Table name</param>
        /// <param name="columns">List of columns</param>
        /// <param name="bulk">Data to insert</param>
        void BulkInsert<T>(string tableName, IEnumerable<string> columns, IEnumerable<T> bulk);
    }
}