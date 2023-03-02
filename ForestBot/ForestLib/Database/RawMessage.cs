using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ForestLib.Database
{
    public class RawMessage
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public long Author { get; set; }
        public string MessageContent { get; set; }
        public string Source { get; set; }
        public string ChannelName { get; set; }
        public long ChannelId { get; set; }
        public string? GuildName { get; set; }
        public long? GuildId { get; set; }
    }
}
