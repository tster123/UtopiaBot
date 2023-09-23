using ForestLib;
using ForestLib.Database;

namespace ForestTests.ParsingTests;

[TestClass]
public class TestAttackParsing
{

    readonly string attacks =
            @":crossed_swords: and a clever girl [**and a clever gir#**] attacked __Boat__ (4:3)|razed: **150**|loss: **47 Skeletons, 5 Ghouls and 47 horses**|kills: **88 (+115 prisoners)**|return: 11.97|33088off (1 gens)
:crossed_swords: Powerzoriucas [**powerzoriuca#**] attacked __Bears with Microscopes__ (4:3)|captured: **97**|loss: **110 Goblins, 36 Ogres and 147 horses**|kills: **123 (+83 prisoners)**|return: 10.08|248 spec creds|204 peasants|50065off (2 gens)
:crossed_swords: Troodon [**troodo#**] attacked __Wazzapi__ (4:3)|killed: **1,013 peasants, thieves, and wizards**|loss: **85 Swordsmen and 85 horses**|kills: **25 (+48 prisoners)**|return: 12.06|28835off (1 gens)
:crossed_swords: Welcome to Jurassic Park [**welcome to jurassic par#**] attacked ____ (:)|bounce: ****|loss: **53 Goblins and 53 horses**|kills: **15 (+29 prisoners)**|return: 10.30|15242off (1 gens)
:crossed_swords: where life will find a way [**where life will find a wa#**] attacked __Dr Stone__ (3:10)|captured: **13**|loss: **87 Strongarms**|kills: **40**|return: 12.78|15 spec creds|43 peasants|11686off (1 gens)
:crossed_swords: Hold on to your butts [**hold on to your butt#**] attacked __The Simpsons__ (3:10)|captured: **37**|loss: **93 Skeletons**|kills: **39 (+77 prisoners)**|return: 14.00|83 spec creds|SPREAD PLAGUE|27936off (2 gens)
:crossed_swords: Therizinosaurus [**therizinosauru#**] attacked __100 oz of laxatives__ (7:4)|learn: **6,382 **|loss: **83 Skeletons and 83 horses**|kills: **46 (+92 prisoners)**|return: 12.35|53856off (2 gens)
:crossed_swords: and a clever girl [**and a clever gir#**] attacked __Kill the Queen__ (4:3)|plundered: **38,909 gold coins, 81,765 bushels and 26,057 runes**|loss: **89 Skeletons and 89 horses**|kills: **474 (+78 prisoners)**|return: 16.63|SPREAD PLAGUE|58684off (1 gens)
:crossed_swords: ---If U dont do your job--- [**---if u dont do your job--#**] attacked __Udontwannaknow__ (4:1)|captured: **45**|loss: **100 Griffins**|kills: **77 (+65 prisoners)**|return: 12.90|93 spec creds|183 peasants|21434off (2 gens)
:crossed_swords: Bunny LumberShredders [**bunny lumbershredder#**] attacked __Diagon Alley__ (2:11)|captured: **74**|loss: **68 Elf Lords and 63 horses**|kills: **40 (+60 prisoners)**|return: 10.88|87 spec creds|32 OS promoted|343 peasants|22014off (2 gens)";

    [TestMethod]
    public void TestMethod1()
    {
        BotParser parser = new BotParser();
        var ret = parser.ParseAttacks(DateTime.UtcNow, 123L, 456L, attacks);
        Assert.AreEqual("and a clever girl", ret[0].SourceProvince);
        var plunder = ret[7];
        Assert.AreEqual(38909, plunder.PlunderGold);
        Assert.AreEqual(26057, plunder.PlunderRunes);
        Assert.AreEqual(81765, plunder.PlunderFood);
    }

    [TestMethod]
    public void TestElfLords()
    {
        string a =
            @":crossed_swords: The Shawshank Revelation [**the shawshank revelatio#**] attacked __Bento Box Yakult__ (6:10)|captured: **72**|loss: **1 Ranger, 156 Elf Lords and 158 horses**|kills: **196**|return: 11.90|123 spec creds|286 peasants|57052off (2 gens)";
        BotParser parser = new BotParser();
        var ret = parser.ParseAttacks(DateTime.UtcNow, 123L, 456L, a);
        var elfLords = ret[0];
        Assert.AreEqual(156, elfLords.LostElites);
        Assert.IsNull(elfLords.Promotions);
    }

    [TestMethod]
    public void TestPromotions()
    {
        string a = attacks.Split("\n")[9];
        BotParser parser = new BotParser();
        var ret = parser.ParseAttacks(DateTime.UtcNow, 123L, 456L, a);
        Attack attack = ret[0];
        Assert.AreEqual(68, attack.LostElites);
        Assert.AreEqual(32, attack.Promotions);

    }

    [TestMethod]
    public void DatabaseLoader_Attacks()
    {
        ForestContext db = new ForestContext();
        var messages = db.RawMessages.Where(m => m.ChannelName == "bot-attacks" && m.Source == "Bot").ToList();
        var parsedMessages = db.Attacks.Select(o => o.ParsedFromMessageId).Distinct().ToHashSet();
        BotParser parser = new BotParser();
        foreach (var m in messages)
        {
            try
            {
                if (parsedMessages.Contains(m.Id))
                {
                    continue;
                }
                List<Attack> ops = parser.ParseAttacks(m.Timestamp, m.GuildId ?? 0, m.Id, m.MessageContent);
                foreach (var op in ops)
                {
                    db.Attacks.Add(op);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(m);
                Console.WriteLine(e);
            }

        }
    }
}