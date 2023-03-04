using ForestLib;
using ForestLib.Tools;

namespace ForestTests
{
    [TestClass]
    public class CeasefireTimelineTest
    {
        [TestMethod]
        public void TestTimeline()
        {
            CeasefireTimeline timeline = new CeasefireTimeline(new AgeSettings(), new StrategySettings());
            var ret = timeline.GetTimeline(new UtopiaDate(3, 1, 1));
            foreach (var e in ret)
            {
                Console.WriteLine(e);
            }
        }
    }
}
