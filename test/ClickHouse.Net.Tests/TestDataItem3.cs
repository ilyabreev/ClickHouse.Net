using System;

namespace ClickHouse.Net.Tests
{
    public class TestDataItem3
    {
        public Guid Id { get; set; }

        public int Number { get; set; }

        public double Cost { get; set; }

        public string Name { get; set; }

        public UInt16 Ushort { get; set; }

        public UInt32 Uint { get; set; }

        public string GetInsertCommand()
        {
            return $"INSERT INTO Test(Id, Number, Cost, Name, Ushort, Uint)" +
                       $"VALUES ('{Id.ToString()}', {Number}, {Cost}, '{Name}', {Ushort}, {Uint})";
        }
        
        public override bool Equals(object obj)
        {
            if (obj is TestDataItem3 item)
            {
                return Id == item.Id && Number == item.Number && Math.Abs(Cost - item.Cost) < 0.001
                       && Name == item.Name && Ushort == item.Ushort && Uint == item.Uint;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + 1;
        }
    }
}
