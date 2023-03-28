using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestLib.Database
{
    public class Province
    {
        public int KingdomId { get; set; }
        public int ProvinceId { get; set; }
        public int Slot { get; set; }
        public string Name { get; set; }
        public long? DiscordUserId { get; set; }
    }
}
