using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForestLib.State;

namespace ForestTests.State
{
    [TestClass]
    public class ScienceTests
    {
        [TestMethod]
        public void EffectCalculation()
        {
            var s = new Science(0.0724, 1.1);
            s.Books = 133391;
            Assert.AreEqual(.205, s.Effect, .002);

            s = new Science(0.0724, 1.1);
            s.Effect = .205;
            Assert.AreEqual(133391.0, s.Books, 1000.0);
        }
    }
}
