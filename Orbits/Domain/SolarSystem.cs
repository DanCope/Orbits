using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbits.Domain
{
    public class SolarSystem
    {
        public List<Body> Bodies { get; set; }

        public SolarSystem()
        {
            Bodies = new List<Body>();
        }
    }
}
