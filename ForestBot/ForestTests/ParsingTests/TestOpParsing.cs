using ForestLib;
using ForestLib.Database;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace ForestTests.ParsingTests
{
    [TestClass]
    public class TestOpParsing
    {
        readonly string ops =
            @"???? Welcome to Jurassic Park welcome to jurassic par#  <<__natures blessing__>> __FAIL__ | 10% guilds (99% BE (m.9.9))
???? Welcome to Jurassic Park welcome to jurassic par#  <<__natures blessing__>> **14** | 10% guilds (99% BE (m.9.9))
???? Welcome to Jurassic Park welcome to jurassic par#  <<__natures blessing__>> __FAIL__ | 10% guilds (99% BE (m.9.9))
???? if you know a UNIX system if you know a unix syste#  <<__sloth__ **| Death Note (3:10)**>> __FAIL__ (-3 wizards)|22% guilds (98% BE|2 (m.2.6))|rNW 1.11
???? if you know a UNIX system if you know a unix syste#  <<__sloth__ **| Death Note (3:10)**>> **5**|22% guilds (98% BE|2 (m.2.6))|rNW 1.11
????? Welcome to Jurassic Park welcome to jurassic par#  <<__rob the towers__ **| Shirakawa Kaede (2:9)**>> __FAIL__ (-2 thieves)|42 sent (0.1)|1.74 (m.1.74)|rNW 0.95
????? Troodon troodo#  <<__rob the towers__ **| Twin Peak Mountains (2:9)**>> __FAIL__ (- thieves)|55 sent (0.13)|1.87 (m.1.87)|rNW 1.27
????? Welcome to Jurassic Park welcome to jurassic par#  <<__rob the towers__ **| Lebron (2:9)**>> **285**|42 sent (0.1)|1.74 (m.1.74)|rNW 1.47
????? Welcome to Jurassic Park welcome to jurassic par#  <<__rob the towers__ **| RedHead Afuro Samurai (2:9)**>> __FAIL__ (-2 thieves)|79 sent (0.19)|1.74 (m.1.74)|rNW 0.97
????? Welcome to Jurassic Park welcome to jurassic par#  <<__rob the towers__ **| waggy (2:9)**>> __FAIL__ (-4 thieves)|91 sent (0.22)|1.74 (m.1.74)|rNW 1
????? BRONTY THE BRONTOSAURUS bronty the brontosauru#  <<__spy on throne__ **| MJ Touched No One (1:9)**>>|63 sent (0.15)|1.49 (m.1.49)|rNW 1.02
???? Spinosaurus spinosauru#  <<__builders boon__>> **23** | 19.5% guilds (99% BE (m.38.6))
????? Welcome to Jurassic Park welcome to jurassic par#  <<__rob the towers__ **| waggy (2:9)**>> **314**|91 sent (0.22)|1.74 (m.1.74)|rNW 1
????? BRONTY THE BRONTOSAURUS bronty the brontosauru#  <<__spy on throne__ **| Facebook the PsyOp Launches (1:9)**>>|63 sent (0.15)|1.49 (m.1.49)|rNW 1.01
????? BRONTY THE BRONTOSAURUS bronty the brontosauru#  <<__rob the towers__ **| Facebook the PsyOp Launches (1:9)**>> **1236**|163 sent (0.39)|1.49 (m.1.49)|rNW 1.01
????? BRONTY THE BRONTOSAURUS bronty the brontosauru#  <<__rob the towers__ **| Facebook the PsyOp Launches (1:9)**>> __FAIL__ (-6 thieves)|163 sent (0.39)|1.49 (m.1.49)|rNW 1.01
????? Sharp Tooth sharp toot#  <<__spy on throne__ **| waggy (2:9)**>>|10 sent (0.02)|1.73 (m.1.73)|rNW 0.89
????? Barney the purple dinosaur barney the purple dinosau#  <<__rob the towers__ **| Tsatthoghua (2:9)**>> __FAIL__ (-5 thieves)|137 sent (0.33)|1.9 (m.2.33)|rNW 0.91
????? Sharp Tooth sharp toot#  <<__spy on throne__ **| Shirakawa Kaede (2:9)**>>|10 sent (0.02)|1.73 (m.1.73)|rNW 1.1
????? Barney the purple dinosaur barney the purple dinosau#  <<__rob the towers__ **| Tsatthoghua (2:9)**>> __FAIL__ (-3 thieves)|137 sent (0.33)|1.9 (m.2.33)|rNW 0.91
????? Barney the purple dinosaur barney the purple dinosau#  <<__rob the towers__ **| Tsatthoghua (2:9)**>> **644**|137 sent (0.33)|1.9 (m.2.33)|rNW 0.91
????? Sharp Tooth sharp toot#  <<__spy on throne__ **| RedHead Afuro Samurai (2:9)**>>|10 sent (0.02)|1.73 (m.1.73)|rNW 1.19
????? Barney the purple dinosaur barney the purple dinosau#  <<__rob the towers__ **| RedHead Afuro Samurai (2:9)**>> __FAIL__ (-6 thieves)|137 sent (0.33)|1.9 (m.2.33)|rNW 1.24
????? Barney the purple dinosaur barney the purple dinosau#  <<__rob the towers__ **| RedHead Afuro Samurai (2:9)**>> **1247**|137 sent (0.33)|1.9 (m.2.33)|rNW 1.24
????? Barney the purple dinosaur barney the purple dinosau#  <<__rob the towers__ **| RedHead Afuro Samurai (2:9)**>> **362**|137 sent (0.33)|1.9 (m.2.33)|rNW 1.24
????? Sharp Tooth sharp toot#  <<__rob the towers__ **| RedHead Afuro Samurai (2:9)**>> **276**|36 sent (0.09)|1.73 (m.1.73)|rNW 1.19
????? Sharp Tooth sharp toot#  <<__rob the towers__ **| RedHead Afuro Samurai (2:9)**>> **216**|26 sent (0.06)|1.73 (m.1.73)|rNW 1.19
????? Sharp Tooth sharp toot#  <<__spy on throne__ **| Ghost Rider (2:9)**>>|10 sent (0.02)|1.73 (m.1.73)|rNW 1.19
????? Sharp Tooth sharp toot#  <<__rob the towers__ **| Ghost Rider (2:9)**>> __FAIL__ (-2 thieves)|45 sent (0.11)|1.73 (m.1.73)|rNW 1.19
????? Sharp Tooth sharp toot#  <<__rob the towers__ **| Ghost Rider (2:9)**>> **321**|45 sent (0.11)|1.73 (m.1.73)|rNW 1.06";

        [TestMethod]
        public void TestParse()
        {
            BotParser parser = new BotParser();
            var ret = parser.ParseOps(DateTime.UtcNow, 123L, 456L, ops);
            Assert.AreEqual("Welcome to Jurassic Park", ret[0].SourceProvince);
        }


        [TestMethod]
        public void DatabaseLoader_Operations()
        {
            ForestContext db = new ForestContext();
            var messages = db.RawMessages.Where(m => m.ChannelName == "bot-ops" && m.Source == "Bot").ToList();
            var parsedMessages = db.Operations.Select(o => o.ParsedFromMessageId).Distinct().ToHashSet();
            BotParser parser = new BotParser();
            foreach (var m in messages)
            {
                try
                {
                    if (parsedMessages.Contains(m.Id))
                    {
                        continue;
                    }

                    var ops = parser.ParseOps(m.Timestamp, m.GuildId ?? 0, m.Id, m.MessageContent);
                    foreach (var op in ops)
                    {
                        db.Operations.Add(op);
                    }

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }
    }
}