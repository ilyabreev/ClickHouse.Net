using System;
using System.Collections.Generic;
using System.Data;
using ClickHouse.Ado;

namespace ClickHouse.Net
{
    /// <summary>
    /// Интерфес управления структурой базы данных
    /// </summary>
    public interface IClickHouseDatabase : IDisposable
    {
        /// <summary>
        /// Соединение
        /// </summary>
        IDbConnection Connection { get; }
        
        /// <summary>
        /// Добавить столбец к таблице
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        void AddColumn(string tableName, Column column);

        /// <summary>
        /// Проверяет наличие столбца в таблице и, в случае отсутствия, создает его
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        void CreateColumnIfNotExists(string tableName, Column column);

        /// <summary>
        /// Проверяет наличие базы данных и, в случае отсутствия, создает ее (вместе со всеми содержащимися в ней таблицами)
        /// </summary>
        /// <param name="database"></param>
        void CreateDBIfNotExists(Database database);

        /// <summary>
        /// Проверяет наличие базы данных и, в случае отсутствия, создает ее
        /// </summary>
        /// <param name="databaseName"></param>
        void CreateDBIfNotExists(string databaseName);

        /// <summary>
        /// Проверяет наличие таблицы и, в случае ее отсутствия, создает ее
        /// </summary>
        /// <param name="table"></param>
        void CreateTableIfNotExists(Table table);

        /// <summary>
        /// Проверяет существование базы данных
        /// </summary>
        /// <param name="databaseName"></param>
        bool DatabaseExists(string databaseName);

        /// <summary>
        /// Проверяет существование столбца в таблице
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        bool ColumnExists(string tableName, string columnName);

        /// <summary>
        /// Открывает соединение с базой данных
        /// </summary>
        IClickHouseDatabase OpenConnection();

        /// <summary>
        /// Проверяет существование таблицы в базе данных
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="tableName"></param>
        bool TableExists(string databaseName, string tableName);

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
        /// Выполнить запрос, который возвращает строки
        /// </summary>
        /// <param name="command">запрос</param>
        void ExecuteNonQuery(IDbCommand command);

        /// <summary>
        /// Выполнить запрос, который возвращает число 
        /// </summary>
        /// <param name="command">запрос</param>
        /// <returns></returns>
        bool ExecuteScalar(IDbCommand command);

        /// <summary>
        /// Выборка строк из БД
        /// </summary>
        /// <param name="commandText">запрос</param>
        /// <returns></returns>
        object[][] ExecuteSelectCommand(string commandText);

        void ExecuteInsertCommand<T>(string commandText, IEnumerable<T> bulk);
        IEnumerable<string> ReadAsStringsList(string commandText);
        void ChangeConnectionSettings(ClickHouseConnectionSettings connectionSettings);
    }
}