using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestLib.Database
{
    public class Aid
    {
        public long Id { get; set; }
        public long GuildId { get; set; }
        public DateTime Timestamp { get; set; }
        public int TimestampSlot { get; set; }
        public long? ParsedFromMessageId { get; set; }
        public string SourceProvince { get; set; }
        public string TargetProvince { get; set; }
        public int? Runes { get; set; }
        public int? Acres { get; set; }
        public int? Soldiers { get; set; }
        public int? Gold { get; set; }
        public int? Food { get; set; }
    }
}
