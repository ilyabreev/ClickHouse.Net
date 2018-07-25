namespace ClickHouse.Net
{
    /// <summary>
    /// Naming conventions for mapping data to objects
    /// </summary>
    public interface IColumnNamingConvention
    {
        /// <summary>
        /// Convert database column name to object property name
        /// </summary>
        /// <param name="columnName">database column name</param>
        /// <returns>property name</returns>
        string GetPropertyName(string columnName);
    }
}