using Microsoft.Xna.Framework;
using System;

namespace Orbits.Domain
{
    public class Body
    {
        public const float G = 6.67408e-11F;

        public float GM {  get { return G * Mass; } }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }

        public float Mass { get; set; }
        public float Radius { get; set; }

        public Vector2 Force(Body OpBody)
        {
            return Force(OpBody.Position, OpBody.Mass);
        }

        public Vector2 Force(Vector2 OpPostion, float OpMass)
        {
            Vector2 r = (OpPostion - Position);
            var force = G * OpMass * Mass / r.LengthSquared();
            return force * r / r.Length();
        }
    }
}
