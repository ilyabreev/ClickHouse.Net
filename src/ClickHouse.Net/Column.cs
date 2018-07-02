namespace ClickHouse.Net
{
    public class Column
    {
        /// <summary>
        /// Представляет столбец таблицы
        /// </summary>
        /// <param name="name">Имя столбца</param>
        /// <param name="type">Тип столбца</param>
        /// <param name="defaultExpression">Выражение по умолчанию для столбца (если необходимо)</param>
        /// <param name="after">Столбец, после которого добавить (Указывается только имя столбца)</param>
        public Column(string name, string type, string defaultExpression = null, string after = null)
        {
            Name = name;
            Type = type;
            DefaultExpression = defaultExpression;
            After = !(string.IsNullOrEmpty(after)) ? $@"AFTER {after}" : string.Empty;
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public string DefaultExpression { get; set; }

        public string After { get; set; }

        public override string ToString()
        {
            return $"{Name} {Type} {DefaultExpression} {After}";
        }
    }
}
