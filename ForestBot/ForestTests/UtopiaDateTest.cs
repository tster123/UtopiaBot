using ForestLib;

namespace ForestTests
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
            AssertDate("Jan  1 Y0", new UtopiaDate(0));
            AssertDate("Jan 24 Y0", new UtopiaDate(23));
            AssertDate("Feb 11 Y0", new UtopiaDate(34));
            AssertDate("Feb 11 Y1", new UtopiaDate(34 + (24 *7)));
        }

        [TestMethod]
        public void TestAdd()
        {
            var d1 = new UtopiaDate(3, 5, 15);
            AssertDate("May 15 Y3", d1.AddTicks(0));
            AssertDate("May 18 Y3", d1.AddTicks(3));
            AssertDate("Jun 11 Y3", d1.AddTicks(20));
            AssertDate("Feb  5 Y4", d1.AddTicks(24 * 4 - 10));

            AssertDate("May 12 Y3", d1.AddTicks(-3));
            AssertDate("Apr 15 Y3", d1.AddTicks(-24));
            AssertDate("May 13 Y2", d1.AddTicks((-24 * 7 - 2)));
        }
    }
}
