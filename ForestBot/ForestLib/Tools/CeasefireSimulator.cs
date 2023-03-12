using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForestLib.AgeSettings;

namespace ForestLib.Tools
{
    public class CeasefireSimulator
    {
        private readonly Age100Settings age;

        public CeasefireSimulator(Age100Settings age)
        {
            this.age = age;
        }
    }
}
