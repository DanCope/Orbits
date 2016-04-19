using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbits.Domain
{
    public class ConicBody : Body
    {
        private Conic _conic;
        public override Conic Conic { get { return _conic; } }
        public ConicBody(string name, float mass, float radius, TimeSpan rotationPeriod, Body parent, Conic conic) 
            : base(name, mass, radius, rotationPeriod, parent, new Vector2(), new Vector2())
        {
            _conic = conic;
        }

        public ConicBody(string name, float mass, float radius, TimeSpan rotationPeriod, double semiMajorAxis, double eccentricity, double argumentOfPeriapsis, double meanAnomaly, Body parent)
            : base(name, mass, radius, rotationPeriod, parent, new Vector2(), new Vector2())
        {
            _conic = new Conic(semiMajorAxis, eccentricity, argumentOfPeriapsis, 0, 0, meanAnomaly, parent);
        }

        protected override void DoMovement(float T, float dT)
        {
            Vector3 r, v;
            Conic.ToCartesian(T, out r, out v);


            var ca = Math.Cos(Math.PI);
            var sa = Math.Sin(Math.PI);

            this.Position = new Vector2(r.X, r.Y);
            this.Velocity = new Vector2(v.X, v.Y);
        }
    }
}
