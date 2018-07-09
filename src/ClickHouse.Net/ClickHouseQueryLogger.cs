using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ClickHouse.Net
{
    public class ClickHouseQueryLogger : IClickHouseQueryLogger
    {
        private readonly ILogger _logger;
        private Stopwatch _timer;

        public ClickHouseQueryLogger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ClickHouseQueryLogger>();
        }

        public void BeforeQuery()
        {
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void AfterQuery(string queryText)
        {
            _timer.Stop();

            var executingTime = _timer.ElapsedMilliseconds;
            var logMessage = $"Clickhouse query: {queryText} completed in {executingTime} ms.";
            _logger.LogInformation(logMessage);

            _timer.Reset();
        }
    }
}
