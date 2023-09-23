using ForestLib;
using ForestLib.AgeSettings;
using ForestLib.AgeSettings.Ages;
using ForestLib.Tools;

namespace ForestTests;

[TestClass]
public class CeasefireTimelineTest
{
    [TestMethod]
    public void TestTimeline()
    {
        CeasefireTimeline timeline = new CeasefireTimeline(new Age100Settings(), new StrategySettings());
        var ret = timeline.GetTimeline(new UtopiaDate(2, 5, 3));
        foreach (var e in ret)
        {
            Console.WriteLine(e);
        }
    }
}