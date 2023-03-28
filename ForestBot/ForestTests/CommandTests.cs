using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForestBot.Modules;

namespace ForestTests
{
    [TestClass]
    public class CommandTests
    {

        [TestMethod]
        public void TestWhoNeeds()
        {
            ExampleModule module = new ExampleModule(null!);
            module.WhoNeeds("sal");
        }
    }
}
