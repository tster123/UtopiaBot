using System.Text.RegularExpressions;
using ForestLib.Database;
using Newtonsoft.Json.Linq;

namespace ForestLib
{

    public enum ActionType
    {
        Attack,
        Spell,
        Thievery
    }
    public class Action
    {
        public ActionType Type;
        public string? OpName;
        public bool Success;
        public int Damage;
        public int ThievesLost;
        public int WizardsLost;
        public int HorsesLost;
        public int OffSpecsLost;
        public int ElitesLost;
        public int AttackKills;
        public int PrisonersGained;
        public int PeasantsGained;
        public double ReturnHours;
        public int SpecCredits;
        public int BuildCredits;
        public int Promotions;
        public int OffenseSent;
        public int GeneralsSent;
    }
    public class BotParser
    {
        // \*{2})?>>(\s*__FAIL__\s*(\(-(\d+)? \w+\))?)?(\s+\*{2}(\d+)\*{2}\s*)?\|((\d+) sent \([\d\.]*\))?

        // ???? Welcome to Jurassic Park welcome to jurassic par#  <<__natures blessing__>> __FAIL__ | 10% guilds (99% BE (m.9.9))
        // ????? Welcome to Jurassic Park welcome to jurassic par#  <<__rob the towers__ **| Shirakawa Kaede (2:9)**>> __FAIL__ (-2 thieves)|42 sent (0.1)|1.74 (m.1.74)|rNW 0.95
        // ???? if you know a UNIX system if you know a unix syste#  <<__sloth__ **| Death Note (3:10)**>> __FAIL__ (-3 wizards)|22% guilds (98% BE|2 (m.2.6))|rNW 1.11
        // ???? if you know a UNIX system if you know a unix syste#  <<__sloth__ **| Death Note (3:10)**>> **5**|22% guilds (98% BE|2 (m.2.6))|rNW 1.11
        // ?? Barney the purple dinosaur barney the purple dinosau#  <<__fanaticism__ **| erectopus**>> **6**
        private Regex OpRegex = new Regex(
            @"\?*\s*([\w\- ]+)#  <<__([a-z ]+)__( \*{2}\| (([\w\- ]+)(\((\d+:\d+)\))?)\s*\*{2})?>>(\s*__FAIL__\s*(\(-(\d+)? \w+\))?)?(\s*\*{2}(\d+)\*{2}\s*)?\|?((\d+) sent \([\d\.]*\))?");

        public List<TmOperation> ParseOps(DateTime timestamp, long guildId, long rawMessageId, string message)
        {
            var ops = new List<TmOperation>();
            int slot = -1;
            foreach (string l in message.Split("\n"))
            {
                slot++;
                string line = l.Trim();
                Match m = OpRegex.Match(l);
                if (!m.Success)
                {
                    throw new ArgumentException("Cannot parse line [" + l + "]");
                }
                string opName = m.Groups[2].Value;
                string provName = m.Groups[1].Value;
                provName = provName.Substring(0, 1 + (provName.Length / 2)).Trim();
                string? targetProv = Nullify(m.Groups[5].Value);
                string? targetKingdom = Nullify(m.Groups[7].Value);
                string? failString = Nullify(m.Groups[8].Value);
                string? lost = Nullify(m.Groups[10].Value);
                if (failString != null && lost == null) lost = "0";
                string? damage = Nullify(m.Groups[12].Value);
                string? thievesSent = Nullify(m.Groups[14].Value);
                ops.Add(new TmOperation
                {
                    OpName = opName,
                    SourceProvince = provName,
                    TargetProvince = targetProv,
                    TargetKingdom = targetKingdom,
                    Success = failString == null,
                    SelfCasualties = Intify(lost),
                    Damage = Intify(damage),
                    ThievesSent = Intify(thievesSent),
                    GuildId = guildId,
                    ParsedFromMessageId = rawMessageId,
                    Timestamp = timestamp,
                    TimestampSlot = slot,
                    Reflected = false
                });
            }
            return ops;
        }

        public string? Nullify(string v) => v.Trim() == "" ? null : v;
        public int? Intify(string? v) => v == null ? null : int.Parse(v);
    }
}