using System;
using System.Collections.Generic;
using System.Linq;

namespace ClickHouse.Net
{
    public class ClickHouseDataFormatter : IClickHouseDataFormatter
    {
        public IEnumerable<T> ConvertFromRawData<T>(object[][] rawData) where T : new()
        {
            if (!rawData.Any())
            {
                return Enumerable.Empty<T>();
            }

            var props = typeof(T).GetProperties();

            if (props.Length != rawData.First().Length)
            {
                throw new InvalidOperationException("Model Type columns count not equals raw data columns count. Check you Model Type...");
            }

            var result = new List<T>();

            foreach (var row in rawData)
            {
                T item = new T();
                for (var i = 0; i < row.Length; i++)
                {
                    if (row[i] == null)
                    {
                        props[i].SetValue(item, row[i]);
                        continue;
                    }

                    if (row[i].ToString().Equals("NaN", StringComparison.InvariantCultureIgnoreCase)
                        || row[i].ToString().Equals("inf", StringComparison.InvariantCultureIgnoreCase)
                        || row[i].ToString().Equals("infinity", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (row[i] is int)
                        {
                            props[i].SetValue(item, 0);
                        }

                        if (row[i] is double)
                        {
                            props[i].SetValue(item, (double)0);
                        }

                        if (row[i] is ulong)
                        {
                            props[i].SetValue(item, (ulong)0);
                        }
                    }
                    else
                    {
                        props[i].SetValue(item, row[i]);
                    }
                }

                result.Add(item);
            }

            return result;
        }
    }
}
