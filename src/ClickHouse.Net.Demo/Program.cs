using System;
using System.Linq;
using ClickHouse.Ado;
using ClickHouse.Net.Entities;

namespace ClickHouse.Net.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new ClickHouseDatabase(
                new ClickHouseConnectionSettings("Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=192.168.0.163;Port=9000;User=default;Password=;SocketTimeout=600000;Database=TeasersStat;"),
                new ClickHouseCommandFormatter(),
                new ClickHouseConnectionFactory(),
                null);
            /*
            db.Open();
            db.BackupDatabase("TeasersStat");
            db.Close();
            */
            var columns = db.DescribeTable("LastTeasersShows").ToArray();
        }
    }
}
