namespace ClickHouse.Net
{
    /// <summary>
    /// Provide custom implementation
    /// for converting into standart types
    /// Implement this interface if your need a specific way
    /// how convert row clickhouse data to your entities
    /// </summary>
    public interface IPropertyBinder
    {
        /// <summary>
        /// Declares how convert raw clickhouse response to standart types
        /// </summary>
        /// <param name="item">Instance of result class <typeparamref name="T"/> which is filling now</param>
        /// <param name="propertyName">property name which currently assigned</param>
        /// <param name="value">Property value</param>
        void BindProperty(object item, string propertyName, object value);
    }
}