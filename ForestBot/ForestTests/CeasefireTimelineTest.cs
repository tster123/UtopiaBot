﻿using ForestLib;
using ForestLib.AgeSettings;
using ForestLib.Tools;

namespace ForestTests
{
    [TestClass]
    public class CeasefireTimelineTest
    {
        [TestMethod]
        public void TestTimeline()
        {
            CeasefireTimeline timeline = new CeasefireTimeline(new Age100Settings(), new StrategySettings());
            var ret = timeline.GetTimeline(new UtopiaDate(3, 7, 21));
            foreach (var e in ret)
            {
                Console.WriteLine(e);
            }
        }
    }
}
