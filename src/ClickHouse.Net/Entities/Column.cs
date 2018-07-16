namespace ClickHouse.Net.Entities
{
    public class Column
    {
        /// <summary>
        /// Represents a column schema in ClickHouse table
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="type">Column type</param>
        /// <param name="defaultExpression">Default expression</param>
        /// <param name="after">Add column after column specified in this parameter</param>
        public Column(string name, string type, string defaultExpression = null, string after = null)
        {
            Name = name;
            Type = type;
            DefaultExpression = (!string.IsNullOrWhiteSpace(defaultExpression) ? " " + defaultExpression : string.Empty);
            After = !(string.IsNullOrEmpty(after)) ? $@" AFTER {after}" : string.Empty;
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public string DefaultExpression { get; set; }

        public string After { get; set; }

        public override string ToString()
        {
            return $"{Name} {Type}{DefaultExpression}{After}";
        }
    }
}
