using Microsoft.Xna.Framework;
using System;

namespace Orbits.Domain
{
    public class Body
    {
        //Gravitational constant
        private const float G = 6.67408e-11F;

        //Standard gravitational parameter
        public float GM {  get { return G * Mass; } }

        public string Name { get; set; }
        public float Mass { get; set; }
        public float Radius { get; set; }
        public TimeSpan RotationPeriod { get; set; }

        public Body Parent { get; set; }
        public virtual Conic Conic { get { return new Conic(Position, Velocity, Parent); } }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; private set; }
        public double Rotation { get; set; }

        /// <summary>
        /// Define an orbitting body
        /// </summary>
        /// <param name="name">String identifier</param>
        /// <param name="mass">Measured in kg</param>
        /// <param name="radius">Measured in m</param>
        /// <param name="parent">Parent body which this body will orbit</param>
        /// <param name="position">Starting position, measured in m from system origin</param>
        /// <param name="velocity">Starting velocity, measure in m/s</param>
        public Body(string name, float mass, float radius, TimeSpan rotationPeriod, Body parent, Vector2 position, Vector2 velocity)
        {
            Name = name;
            Mass = mass;
            Radius = radius;
            RotationPeriod = rotationPeriod;

            Parent = parent;
            Position = position;
            Velocity = velocity;
            Rotation = 0;
        }

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

        public void Step(float T, float dT)
        {
            DoMovement(T, dT);

            DoRotation(T, dT);
        }

        protected virtual void DoMovement(float T, float dT)
        {
            //Using the LeapFrog algorithm
            //Orbit logic
            Velocity += Acceleration * (dT / 2);
            Position += Velocity * dT;

            //Should apply force from significant siblings too
            var force = Force(Parent); // + Force(Siblings)
            Acceleration = force / Mass; // + Thrust

            Velocity += Acceleration * (dT / 2);
        }

        protected virtual void DoRotation(float T, float dT)
        {
            var rotationCompletion = (T % RotationPeriod.TotalSeconds) / RotationPeriod.TotalSeconds;
            Rotation = (2 * Math.PI) * rotationCompletion;
        }
    }
}
