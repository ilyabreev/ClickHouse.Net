using System;

namespace ClickHouse.Net.Tests
{
    public class TestDataItem
    {
        public string Id { get; set; }

        public double Value { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TestDataItem item)
            {
                return item.Id == Id && item.Value.Equals(Value);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + 1;
        }
    }
}