using System.Text;
using System.Text.RegularExpressions;
using ForestLib.Database;
using ForestLib.KewlStuff;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;

namespace ForestLib;

public class BotParser
{
    private interface IItemExtractor<T>
    {
        T ExtractItem(DateTime timestamp, long guildId, long rawMessageId, Match m, int slot);
    }

    private List<T> ParseMessages<T>(Regex regex, IItemExtractor<T> extractor, DateTime timestamp, long guildId, long rawMessageId, string message)
    {
        var ops = new List<T>();
        int slot = -1;
        foreach (string l in message.Split("\n"))
        {
            slot++;
            string line = StringUtil.UncommaNumbers(l.Trim());
            Match m = regex.Match(line);
            if (!m.Success)
            {
                throw new ArgumentException("Cannot parse line [" + l + "]");
            }

            try
            {
                ops.Add(extractor.ExtractItem(timestamp, guildId, rawMessageId, m, slot));
            }
            catch (Exception)
            {
                Console.WriteLine("error parsing [" + line + "]");
                throw;
            }
        }
        return ops;
    }

    // \*{2})?>>(\s*__FAIL__\s*(\(-(\d+)? \w+\))?)?(\s+\*{2}(\d+)\*{2}\s*)?\|((\d+) sent \([\d\.]*\))?

    // ???? Welcome to Jurassic Park welcome to jurassic par#  <<__natures blessing__>> __FAIL__ | 10% guilds (99% BE (m.9.9))
    // ????? Welcome to Jurassic Park welcome to jurassic par#  <<__rob the towers__ **| Shirakawa Kaede (2:9)**>> __FAIL__ (-2 thieves)|42 sent (0.1)|1.74 (m.1.74)|rNW 0.95
    // ???? if you know a UNIX system if you know a unix syste#  <<__sloth__ **| Death Note (3:10)**>> __FAIL__ (-3 wizards)|22% guilds (98% BE|2 (m.2.6))|rNW 1.11
    // ???? if you know a UNIX system if you know a unix syste#  <<__sloth__ **| Death Note (3:10)**>> **5**|22% guilds (98% BE|2 (m.2.6))|rNW 1.11
    // ?? Barney the purple dinosaur barney the purple dinosau#  <<__fanaticism__ **| erectopus**>> **6**
    // :star2::green_heart:  ---I will shoot you myself--- ---i will shoot you myself--- <<__ghost workers__>> **13** | 21% guilds (93% BE (m.19.5))
    // :comet::broken_heart: High Treeson high treeso#  <<__pitfalls__ **| VLOOKUppercut (5:2)**>> __FAIL__ __REFLECTED (8)__|10.4% guilds (92% BE|2.12 (m.5.27)) vs 2.07 (m.3.02)|rNW 1.05
    private readonly Regex _opRegex = new Regex(
        @"(\?+|(:\w+:)+)\s*([\w\- ]+)#?\s+<<__([a-z ]+)__( \*{2}\| (([\w\- ]+)(\((\d+:\d+)\))?)\s*\*{2})?>>(\s*__FAIL__\s*(\(-(\d+)? \w+\))?(__REFLECTED\s*\((\d+)\)__)?)?(\s*\*{2}(\d+)\*{2}\s*)?\|?((\d+) sent \([\d\.]*\))?");

    private class TmOpExtractor : IItemExtractor<TmOperation>
    {
        public TmOperation ExtractItem(DateTime timestamp, long guildId, long rawMessageId, Match m, int slot)
        {
            string opName = m.Groups[4].Value;
            string provName = m.Groups[3].Value;
            provName = provName.Substring(0, 1 + (provName.Length / 2)).Trim();
            string? targetProv = m.Groups[7].Value.Nullify();
            string? targetKingdom = m.Groups[9].Value.Nullify();
            string? failString = m.Groups[10].Value.Nullify();
            string? reflectedString = m.Groups[13].Value.Nullify();
            string? reflectedDamage = m.Groups[14].Value.Nullify();
            string? lost = m.Groups[12].Value.Nullify();
            if (failString != null && lost == null) lost = "0";
            string? damage = m.Groups[16].Value.Nullify();
            string? thievesSent = m.Groups[18].Value.Nullify();
            if (damage == null && reflectedDamage != null) damage = reflectedDamage;
            return new TmOperation
            {
                OpName = opName,
                SourceProvince = provName,
                TargetProvince = targetProv,
                TargetKingdom = targetKingdom,
                Success = failString == null,
                SelfCasualties = lost.Intify(),
                Damage = damage.Intify(),
                ThievesSent = thievesSent.Intify(),
                GuildId = guildId,
                ParsedFromMessageId = rawMessageId,
                Timestamp = timestamp,
                TimestampSlot = slot,
                Reflected = reflectedString != null
            };
        }
    }


    public string? Nullify(string v) => v.Trim() == "" ? null : v;
    public int? Intify(string? v) => v == null ? null : int.Parse(v);
    private readonly IItemExtractor<TmOperation> _opExtractor = new TmOpExtractor();

    public List<TmOperation> ParseOps(DateTime timestamp, long guildId, long rawMessageId, string message)
    {
        return ParseMessages(_opRegex, _opExtractor, timestamp, guildId, rawMessageId, message);
    }

    /*
:crossed_swords: and a clever girl [**and a clever gir#**] attacked __Boat__ (4:3)|razed: **150**|loss: **47 Skeletons, 5 Ghouls and 47 horses**|kills: **88 (+115 prisoners)**|return: 11.97|33088off (1 gens)
:crossed_swords: Powerzoriucas [**powerzoriuca#**] attacked __Bears with Microscopes__ (4:3)|captured: **97**|loss: **110 Goblins, 36 Ogres and 147 horses**|kills: **123 (+83 prisoners)**|return: 10.08|248 spec creds|204 peasants|50065off (2 gens)
:crossed_swords: Troodon [**troodo#**] attacked __Wazzapi__ (4:3)|killed: **1,013 peasants, thieves, and wizards**|loss: **85 Swordsmen and 85 horses**|kills: **25 (+48 prisoners)**|return: 12.06|28835off (1 gens)
:crossed_swords: Welcome to Jurassic Park [**welcome to jurassic par#**] attacked ____ (:)|bounce: ****|loss: **53 Goblins and 53 horses**|kills: **15 (+29 prisoners)**|return: 10.30|15242off (1 gens)
:crossed_swords: where life will find a way [**where life will find a wa#**] attacked __Dr Stone__ (3:10)|captured: **13**|loss: **87 Strongarms**|kills: **40**|return: 12.78|15 spec creds|43 peasants|11686off (1 gens)
:crossed_swords: Therizinosaurus [**therizinosauru#**] attacked __100 oz of laxatives__ (7:4)|learn: **6,382 **|loss: **83 Skeletons and 83 horses**|kills: **46 (+92 prisoners)**|return: 12.35|53856off (2 gens)
:crossed_swords: and a clever girl [**and a clever gir#**] attacked __Kill the Queen__ (4:3)|plundered: **38,909 gold coins, 81,765 bushels and 26,057 runes**|loss: **89 Skeletons and 89 horses**|kills: **474 (+78 prisoners)**|return: 16.63|SPREAD PLAGUE|58684off (1 gens)
:crossed_swords: ---If U dont do your job--- [**---if u dont do your job--#**] attacked __Udontwannaknow__ (4:1)|captured: **45**|loss: **100 Griffins**|kills: **77 (+65 prisoners)**|return: 12.90|93 spec creds|183 peasants|21434off (2 gens)
:crossed_swords: Bunny LumberShredders [**bunny lumbershredder#**] attacked __Diagon Alley__ (2:11)|captured: **74**|loss: **68 Elf Lords and 63 horses**|kills: **40 (+60 prisoners)**|return: 10.88|87 spec creds|0 OS promoted|343 peasants|22014off (2 gens)
:crossed_swords: Baobab Tree [**baobab tre#**] attacked __Bruce Pollos Hermanos__ (5:12)|captured: **319**|loss: **699 Strongarms, 388 Brutes and 1,088 horses**|kills: **702 (+75 prisoners)**|return: 12.56|1175 spec creds|676 peasants|224462off (3 gens)
:crossed_swords: PalmTree [**palmtre#**] attacked __SUM__ (5:2)|captured: **137**|loss: **no troops or horses**|kills: **261 (+105 prisoners)**|return: 8.54|357 spec creds|115434off (2 gens)
    */
    private Regex _attackRegex = new Regex(
        @":crossed_swords: ([\w -]+)\[\*\*[\w -]*#\*\*\] attacked __([\w -]*)__ \((\d*:\d*)\)\|(\w+):\s+\*\*([^*]*)\s*\*\*\|loss: \*\*([\w\d\s,]+)\*\*\|kills: \*\*(\d+)( \(\+(\d+) prisoners\))?\*\*\|return: ([\d\.]+)\|((\d+) spec creds\|)?(([\d,]+) OS promoted\|)?(([\d,]+) peasants\|)?(SPREAD PLAGUE\|)?(\d+)off \((\d) gens\)");

    private class AttackExtractor : IItemExtractor<Attack>
    {
        private Dictionary<string, string> UnitType = new()
        {
            ["Griffins"] = "ospec",
            ["Warriors"] = "ospec",
            ["Rangers"] = "ospec",
            ["Magicians"] = "ospec",
            ["Strongarms"] = "ospec",
            ["Swordsmen"] = "ospec",
            ["Goblins"] = "ospec",
            ["Quickblades"] = "ospec",
            ["Skeletons"] = "ospec",
            ["Night Rangers"] = "ospec",
            ["Drakes"] = "elites",
            ["Berserkers"] = "elites",
            ["Elf Lords"] = "elites",
            ["Beastmasters"] = "elites",
            ["Brutes"] = "elites",
            ["Knights"] = "elites",
            ["Drows"] = "elites",
            ["Ogres"] = "elites",
            ["Ghouls"] = "elites",
            ["Golems"] = "elites",
            ["soldiers"] = "soldiers",
            ["horses"] = "horses"
        };

        private Dictionary<string, string> AttackType = new()
        {
            ["captured"] = "trad",
            ["razed"] = "raze",
            ["recaptured"] = "ambush",
            ["killed"] = "mass",
            ["bounce"] = "bounce",
            ["plundered"] = "plunder"
        };

        public Attack ExtractItem(DateTime timestamp, long guildId, long rawMessageId, Match m, int slot)
        {
            string provName = m.Groups[1].Value.Trim();
            string? targetProv = m.Groups[2].Value.Nullify();
            string targetKingdom = m.Groups[3].Value.Trim();
            string attackType = m.Groups[4].Value;
            if (AttackType.ContainsKey(attackType))
            {
                attackType = AttackType[attackType];
            }

            string? damage = m.Groups[5].Value.Nullify();
            if (damage != null)
            {
                damage = damage.Replace(" peasants, thieves, and wizards", "").Replace(",", "");
            }

            int? plunderGold = null, plunderFood = null, plunderRunes = null;
            if (attackType == "plunder" && damage != null)
            {
                // 38909 gold coins 81765 bushels and 26057 runes
                MatchCollection matches = Regex.Matches(damage, @"(\d+) ([a-zA-Z ]+)");
                foreach (Match match in matches)
                {
                    int value = int.Parse(match.Groups[1].Value);
                    if (match.Groups[2].Value.ToLower().StartsWith("gold")) plunderGold = value;
                    if (match.Groups[2].Value.ToLower().StartsWith("bushel")) plunderFood = value;
                    if (match.Groups[2].Value.ToLower().StartsWith("rune")) plunderRunes = value;
                }

                damage = null;
            }

            // 53 Goblins and 53 horses
            // 47 Skeletons, 5 Ghouls and 47 horses
            string? casualties = m.Groups[6].Value.Nullify();
            string kills = m.Groups[7].Value;
            string? prisoners = m.Groups[9].Value.Nullify();
            string retHours = m.Groups[10].Value;
            string? specCreds = m.Groups[12].Value.Nullify();
            string? promotions = m.Groups[14].Value.Nullify();
            string? peasants = m.Groups[16].Value.Nullify();
            bool spreadPlague = !m.Groups[17].Value.IsNullOrEmpty();
            string offense = m.Groups[18].Value;
            string gens = m.Groups[19].Value;

            string[] casualtyParts;
            if (casualties != null && casualties == "no troops or horses")
            {
                casualtyParts = [];
            }
            else
            {
                casualtyParts = casualties?.Replace(" and ", ", ").Split(",").Select(s => s.Trim()).ToArray() ?? [];
            }
            int lostSoldiers = 0, lostOSpecs = 0, lostElites = 0, lostHorses = 0;
            foreach (var casualtyPartDirty in casualtyParts)
            {
                string casualtyPart = casualtyPartDirty.Trim();
                string numPart = casualtyPart.Substring(0, casualtyPart.IndexOf(" ")).Trim();
                string namePart = casualtyPart.Substring(casualtyPart.IndexOf(" ")).Trim();
                if (numPart == "1")
                {
                    namePart += "s";
                }
                if (!UnitType.TryGetValue(namePart, out string? resolvedType))
                {
                    throw new ArgumentException("Cannot parse attack.  unrecognized casualty type: " + namePart);
                }

                int value = int.Parse(numPart);
                if (resolvedType == "soldier") lostSoldiers = value;
                else if (resolvedType == "ospec") lostOSpecs = value;
                else if (resolvedType == "elites") lostElites = value;
                else if (resolvedType == "horses") lostHorses = value;
            }

            return new Attack
            {
                GuildId = guildId,
                ParsedFromMessageId = rawMessageId,
                Timestamp = timestamp,
                TimestampSlot = slot,
                AttackType = attackType,
                SourceProvince = provName,
                TargetProvince = targetProv,
                TargetKingdom = targetKingdom,
                Damage = damage.ParseInt(),
                PlunderGold = plunderGold,
                PlunderFood = plunderFood,
                PlunderRunes = plunderRunes,
                LostElites = lostElites,
                LostHorses = lostHorses,
                LostOffSpecs = lostOSpecs,
                LostSoldiers = lostSoldiers,
                GeneralsSent = gens.ParseInt(),
                Kills = kills.Replace(",", "").ParseInt(),
                SpreadPlague = spreadPlague,
                OffenseSent = offense.ParseInt(),
                Peasants = peasants.ParseInt(),
                Prisoners = prisoners.ParseInt(),
                ReturnHours = double.Parse(retHours),
                SpecCredits = specCreds.ParseInt(),
                Promotions = promotions.Intify()
            };
        }
    }

    private readonly IItemExtractor<Attack> _attackExtractor = new AttackExtractor();

    public List<Attack> ParseAttacks(DateTime timestamp, long guildId, long rawMessageId, string message)
    {
        return ParseMessages(_attackRegex, _attackExtractor, timestamp, guildId, rawMessageId, message);
    }
}

public static class StringUtil
{
    public static string? UncommaNumbers(string? str)
    {
        if (str == null || str.Length <= 2) return str;
        StringBuilder b = new StringBuilder(str.Length);
        b.Append(str[0]);
        for (int i = 1; i < str.Length - 1; i++)
        {
            if (str[i] == ',' &&
                str[i - 1] >= '0' && str[i - 1] <= '9' &&
                str[i + 1] >= '0' && str[i + 1] <= '9')
            {
                // skip
            }
            else
            {
                b.Append(str[i]);
            }
        }

        b.Append(str[^1]);
        return b.ToString();
    }
}