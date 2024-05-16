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
        CeasefireTimeline timeline = new CeasefireTimeline(new Age105Settings(), new StrategySettings());
        var ret = timeline.GetTimelineWithExpedientAndHaste(new UtopiaDate(2, 2, 2));
        foreach (var e in ret)
        {
            Console.WriteLine(e);
        }

        Console.WriteLine("-------------------");
        ret = timeline.GetTimelineWithExpedient(new UtopiaDate(2, 2, 2));
        foreach (var e in ret)
        {
            Console.WriteLine(e);
        }
    }
}

/*
 * 
Jun  5 Y1 - War ended
   Jun  5 Y1 - Build towers (BB, double speed) (min 16.4% at 40.0% prod, 8.0% tools science)
   Jul 17 Y1 - Last date to activate Expedient
   Jul 18 Y1 - Calculate your draft start time
   Jan  5 Y2 - Raise wages to 200%
   Jan 17 Y2 - Build stables (BB + L&P)
   Feb  5 Y2 - Latest build 25-30% arms (w/ BB)
   Feb 11 Y2 - Estimated start of haste ritual
   Feb 16 Y2 - Last date to start training (w/ IA  & 4.00% valor science)
   Feb 22 Y2 - Switch to war build
   Mar  6 Y2 - Ceasefire ends
*/