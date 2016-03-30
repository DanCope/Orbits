using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbits.Domain
{
    public class Conic
    {
        public float Eccentricity { get; set; }
        public float SemiMajorAxis { get; set; }
        public float SemiMinorAxis { get; set; }
        public float FociToCenter { get; set; }
        public float Inclination { get; set; }
        public float LongitudeOfAscendingNode { get; set; }
        public float ArgumentOfPeriapsis { get; set; }
        public float MeanAnomaly { get; set; }
        public float Period { get; set; }

        public float Periapsis { get; set; }
        public float Apoapsis { get; set; }
        public Conic(Vector2 Position, Vector2 Velocity, Body b)
        {
            //Convert to Vector3 so it works with the equation examples
            var r = new Vector3(Position, 0);
            var v = new Vector3(Velocity, 0);

            Setup(r, v, b.GM);
        }

        public Conic(Vector3 Position, Vector3 Velocity, Body b)
        {
            Setup(Position, Velocity, b.GM);
        }

        private void Setup(Vector3 r, Vector3 v, float U)
        {
            //Orbital Momentum vector
            var h = Vector3.Cross(r, v);
            //Eccentricity vector
            var e = (Vector3.Cross(v, h) / U) - (r / r.Length());
            //Node vector
            var n = new Vector3(-h.Y, h.X, 0);
            //True anomaly
            var V = (float)(Vector3.Dot(r, v) >= 0 ? Math.Acos(Vector3.Dot(e, r) / (e.Length() * r.Length())) : 2 * Math.PI - Math.Acos(Vector3.Dot(e, r) / (e.Length() * r.Length())));

            Inclination = (float)Math.Acos(h.Z / h.Length());
            Eccentricity = e.Length();

            LongitudeOfAscendingNode = (float)(n.Y >= 0 ? Math.Acos(n.X / n.Length()) : 2 * Math.PI - Math.Acos(n.X / n.Length()));
            if (float.IsNaN(LongitudeOfAscendingNode)) LongitudeOfAscendingNode = 0f;

            ArgumentOfPeriapsis = (float)(e.Z >= 0 ? Math.Acos(Vector3.Dot(e, n) / (e.Length() * n.Length())) : 2 * Math.PI - Math.Acos(Vector3.Dot(e, n) / (e.Length() * n.Length())));
            if (float.IsNaN(ArgumentOfPeriapsis)) ArgumentOfPeriapsis = (float)(n.Z >= 0 ? Math.Atan2(e.Y, e.X) : 2 * Math.PI - Math.Atan2(e.Y, e.X));

            var E = (float)(2 * Math.Atan(Math.Tan(V / 2) / Math.Sqrt(((1 + Eccentricity) / (1 - Eccentricity)))));
            MeanAnomaly = E - Eccentricity * (float)Math.Sin(E);

            SemiMajorAxis = 1 / (2 / r.Length() - v.LengthSquared() / U);
            SemiMinorAxis = SemiMajorAxis * (float)Math.Sqrt(1 - e.LengthSquared());
            FociToCenter = (float)Math.Sqrt(Math.Pow(SemiMajorAxis, 2) - Math.Pow(SemiMinorAxis, 2));

            Period = (float)(2 * Math.PI * Math.Sqrt(Math.Pow(SemiMajorAxis, 3) / U));

            Periapsis = SemiMajorAxis * (1 - Eccentricity);
            Apoapsis = SemiMajorAxis * (1 + Eccentricity);
        }
    }
}
