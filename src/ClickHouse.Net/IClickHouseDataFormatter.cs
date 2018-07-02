using System.Collections.Generic;

namespace ClickHouse.Net
{
    /// <summary>
    /// Обрабатывает сырой результат выполнения запроса
    /// </summary>
    /// <typeparam name="T">Тип, в который необходимо конвертировать данные</typeparam>
    /// <param name="rawData">Исходные данные</param>
    /// <returns></returns>
    public interface IClickHouseDataFormatter
    {
        IEnumerable<T> ConvertFromRawData<T>(object[][] rawData) where T : new();
    }
}
