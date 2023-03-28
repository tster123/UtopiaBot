using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestLib.Database
{
    public class Kingdom
    {
        public int KingdomId { get; set; }
        public int Age { get; set; }
        public long GuildId { get; set; }
        public string Location { get; set; }
    }
}
