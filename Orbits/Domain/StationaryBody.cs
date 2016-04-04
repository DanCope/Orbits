using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbits.Domain
{
    public class StationaryBody : Body
    {
        /// <summary>
        /// Define a stationary body.
        /// </summary>
        /// <param name="name">String identifier</param>
        /// <param name="mass">Measured in kg</param>
        /// <param name="radius">Measured in m</param>
        public StationaryBody(string name, float mass, float radius, TimeSpan rotationPeriod)
            : base(name, mass, radius, rotationPeriod, null, new Vector2(), new Vector2())
        { }

        protected override void DoMovement(float T, float dT) { }
    }
}
