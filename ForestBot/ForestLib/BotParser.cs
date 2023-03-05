﻿using System.Text.RegularExpressions;
using ForestLib.Database;
using ForestLib.KewlStuff;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;

namespace ForestLib
{
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
                string line = l.Trim();
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
        private readonly Regex _opRegex = new Regex(
            @"\?*\s*([\w\- ]+)#  <<__([a-z ]+)__( \*{2}\| (([\w\- ]+)(\((\d+:\d+)\))?)\s*\*{2})?>>(\s*__FAIL__\s*(\(-(\d+)? \w+\))?)?(\s*\*{2}(\d+)\*{2}\s*)?\|?((\d+) sent \([\d\.]*\))?");

        private class TmOpExtractor : IItemExtractor<TmOperation>
        {
            public TmOperation ExtractItem(DateTime timestamp, long guildId, long rawMessageId, Match m, int slot)
            {
                string opName = m.Groups[2].Value;
                string provName = m.Groups[1].Value;
                provName = provName.Substring(0, 1 + (provName.Length / 2)).Trim();
                string? targetProv = m.Groups[5].Value.Nullify();
                string? targetKingdom = m.Groups[7].Value.Nullify();
                string? failString = m.Groups[8].Value.Nullify();
                string? lost = m.Groups[10].Value.Nullify();
                if (failString != null && lost == null) lost = "0";
                string? damage = m.Groups[12].Value.Nullify();
                string? thievesSent = m.Groups[14].Value.Nullify();
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
                    Reflected = false
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
         */
        private Regex _attackRegex = new Regex(
        @":crossed_swords: ([\w -]+)\[\*\*[\w =]*#\*\*\] attacked __([\w -]*)__ \((\d*:\d*)\)\|(\w+):\s+\*\*([^*]*)\s*\*\*\|loss: \*\*([\w\d\s,]+)\*\*\|kills: \*\*(\d+)( \(\+(\d+) prisoners\))?\*\*\|return: ([\d\.]+)\|((\d+) spec creds\|)?((\d+) peasants\|)?(SPREAD PLAGUE\|)?(\d+)off \((\d) gens\)");

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
                ["Skeletons"] = "ospec",
                ["Drakes"] = "elites",
                ["Berserkers"] = "elites",
                ["Elf Lords"] = "elites",
                ["Beastmasters"] = "elites",
                ["Brutes"] = "elites",
                ["Knights"] = "elites",
                ["Ogres"] = "elites",
                ["Ghouls"] = "elites",
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
                string? peasants = m.Groups[14].Value.Nullify();
                bool spreadPlague = !m.Groups[15].Value.IsNullOrEmpty();
                string offense = m.Groups[16].Value;
                string gens = m.Groups[17].Value;

                string[] casualtyParts = casualties?.Replace(" and ", ", ").Split(",").Select(s => s.Trim()).ToArray() ?? Array.Empty<string>();
                int lostSoldiers = 0, lostOSpecs = 0, lostElites = 0, lostHorses = 0;
                foreach (var casualtyPart in casualtyParts)
                {
                    string[] split = casualtyPart.Trim().Split(" ");
                    if (split[0] == "1")
                    {
                        split[1] += "s";
                    }
                    if (!UnitType.TryGetValue(split[1], out string? resolvedType))
                    {
                        throw new ArgumentException("Cannot parse attack.  unrecognized casualty type: " + split[1]);
                    }

                    int value = int.Parse(split[0]);
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
                    SpecCredits = specCreds.ParseInt()
                };
            }
        }

        private readonly IItemExtractor<Attack> _attackExtractor = new AttackExtractor();

        public List<Attack> ParseAttacks(DateTime timestamp, long guildId, long rawMessageId, string message)
        {
            return ParseMessages(_attackRegex, _attackExtractor, timestamp, guildId, rawMessageId, message);
        }
    }
}