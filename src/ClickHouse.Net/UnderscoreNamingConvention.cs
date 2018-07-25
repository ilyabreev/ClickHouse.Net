using System;
using System.Text.RegularExpressions;

namespace ClickHouse.Net
{
    /// <summary>
    /// Naming convention for underscore column notation (e.g. min_date)
    /// </summary>
    public class UnderscoreNamingConvention : IColumnNamingConvention
    {
        public static Regex Underscore = new Regex("_[^_]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        
        public string GetPropertyName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentException(nameof(columnName));
            }

            columnName = Underscore.Replace(columnName, match => match.Value[1].ToString().ToUpper());
            return columnName[0].ToString().ToUpper() + columnName.Substring(1);
        }
    }
}
