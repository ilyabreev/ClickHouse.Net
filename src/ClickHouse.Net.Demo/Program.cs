using ClickHouse.Ado;
using ClickHouse.Net.Entities;
using ClickHouse.Net.Tests;
using System;
using System.Collections.Generic;
using System.Linq;

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
                null,
                null);
            /*
            db.Open();
            db.BackupDatabase("TeasersStat");
            db.Close();
            */
            var columns = db.DescribeTable("LastTeasersShows").ToArray();

            
            /* Get data using default binder
            db.Open();
            for (int i = 0; i < 10; i++)
            {
                var res = db.DatabaseExists("test");
                Console.WriteLine($"Database test exists: {res}");
            }
            
            // Execute query and get result
            var queryMappingSuccess = CastRawValuesToClassTypeProperties(db);
            db.Close();
            */
            
            /* Using custom binder */
            /*CustomBinderDemo();*/
        }

        public static void CustomBinderDemo()
        {
            var db = new ClickHouseDatabase(
                /*new ClickHouseConnectionSettings("Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=192.168.0.163;Port=9000;User=default;Password=;SocketTimeout=600000;Database=TeasersStat;"),*/
                new ClickHouseConnectionSettings("Compress=True;CheckCompressedHash=False;Compressor=lz4;Host=localhost;Port=9000;User=default;Password=;SocketTimeout=600000;Database=system;"),
                new ClickHouseCommandFormatter(),
                new ClickHouseConnectionFactory(),
                null,
                new NotImplementedPropertyBinder());
            
            db.Open();
            
            // Execute query and get result
            var queryMappingSuccess = CastRawValuesToClassTypeProperties(db);
            db.Close();
        }
        
        public static bool CastRawValuesToClassTypeProperties(IClickHouseDatabase db)
        {
            if (db.TableExists("Test"))
            {
                db.DropTable("Test");
            }

            db.CreateTable(new Table
            {
                Engine = "MergeTree(date, (date, Id, Number, Cost), 8192)",
                Name = "Test",
                Columns = new List<Column>()
                {
                    new Column("date", "Date"),
                    new Column("Id", "String"),
                    new Column("Number", "Int32"),
                    new Column("Cost", "Float64"),
                    new Column("Name", "String"),
                    new Column("Ushort", "UInt16"),
                    new Column("Uint", "UInt32"),

                }
            });

            var testItem = new TestDataItem3
            {
                Id = Guid.NewGuid(),
                Number = -96,
                Cost = 31,
                Name = "Jon Skeet",
                Ushort = 150,
                Uint = 65536
            };

            var command = testItem.GetInsertCommand();
            db.ExecuteNonQuery(command);

            command = "SELECT Id, Number, Cost, Name, Ushort, Uint FROM Test";
            var resultItem = db.ExecuteQueryMapping<TestDataItem3>(command, convention: new UnderscoreNamingConvention()).Single();

            db.DropTable("Test");
            return testItem.Equals(resultItem);
        }
    }
}
