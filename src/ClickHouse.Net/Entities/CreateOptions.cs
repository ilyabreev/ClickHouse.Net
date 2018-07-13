namespace ClickHouse.Net.Entities
{
    /// <summary>
    /// Options related to all database objects creation
    /// </summary>
    public class CreateOptions
    {
        /// <summary>
        /// Create object only if it not exists
        /// </summary>
        public bool IfNotExists { get; set; }
    }
}
