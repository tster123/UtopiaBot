using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using ForestLib.AgeSettings;
using ForestLib.AgeSettings.Ages;
using ForestLib.State;

namespace ForestTests.State
{
    [TestClass]
    public class ScienceTests
    {
        [TestMethod]
        public void EffectCalculation()
        {
            var s = new ScienceEffect("foo", 0.0724);
            var state = new ProvinceState() { Race = new Race("science") { Science = 1.1 } };
            state.Buildings = new Buildings();
            state.Acres = 100;
            // TODO: add a library test
            var age = new Age100Settings();
            double effect = s.GetBonus(133391, state, age);
            Assert.AreEqual(.205, effect, .002);

            Assert.AreEqual(133391.0, s.CalculateBooksNeededFor(.205, state, age), 1000.0);
        }
    }
}
