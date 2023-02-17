using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UtopiaLib;

namespace UtopiaLibTest
{
    [TestClass]
    public class UtopiaDateTest
    {
        private void AssertDate(string expected, UtopiaDate actual)
        {
            Assert.AreEqual(expected, actual.ToString());
        }

        [TestMethod]
        public void TestCtors()
        {
            AssertDate("Jan1 Y0", new UtopiaDate(0));
            AssertDate("Jan24 Y0", new UtopiaDate(23));
            AssertDate("Feb11 Y0", new UtopiaDate(34));
            AssertDate("Feb11 Y1", new UtopiaDate(34 + (24 *7)));
        }

        [TestMethod]
        public void TestAdd()
        {
            var d1 = new UtopiaDate(3, 5, 15);
            AssertDate("May15 Y3", d1.AddTicks(0));
            AssertDate("May18 Y3", d1.AddTicks(3));
            AssertDate("Jun11 Y3", d1.AddTicks(20));
            AssertDate("Feb5 Y4", d1.AddTicks(24 * 4 - 10));

            AssertDate("May12 Y3", d1.AddTicks(-3));
            AssertDate("Apr15 Y3", d1.AddTicks(-24));
            AssertDate("May13 Y2", d1.AddTicks((-24 * 7 - 2)));
        }
    }
}
