using System;

namespace ClickHouse.Net.Entities
{
    /// <summary>
    /// Object representing table partition
    /// </summary>
    public class Part
    {
        public string Partition { get; set; }
        
        public string Name { get; set; }
        
        public byte Active { get; set; }

        public UInt64 Marks { get; set; }

        public UInt64 MarksSize { get; set; }

        public UInt64 Rows { get; set; }

        public UInt64 Bytes { get; set; }

        public DateTime ModificationTime { get; set; }

        public DateTime RemoveTime { get; set; }

        public UInt32 Refcount { get; set; }

        public DateTime MinDate { get; set; }

        public DateTime MaxDate { get; set; }

        public long MinBlockNumber { get; set; }

        public long MaxBlockNumber { get; set; }

        public UInt32 Level { get; set; }

        public UInt64 PrimaryKeyBytesInMemory { get; set; }

        public UInt64 PrimaryKeyBytesInMemoryAllocated { get; set; }

        public string Database { get; set; }

        public string Table { get; set; }

        public string Engine { get; set; }
    }
}
