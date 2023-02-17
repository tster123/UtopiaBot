using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UtopiaLib;
using UtopiaLib.Tools;

namespace UtopiaLibTest
{
    [TestClass]
    public class CeasefireTimelineTest
    {
        [TestMethod]
        public void TestTimeline()
        {
            CeasefireTimeline timeline = new CeasefireTimeline();
            var ret = timeline.GetTimeline(new UtopiaDate(0, 5, 17));
            foreach (var e in ret)
            {
                Console.WriteLine(e);
            }
        }
    }
}
