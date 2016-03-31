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
        public Body PrimeFocus { get; protected set; }

        //Orbital Elements
        public float SemiMajorAxis { get; protected set; }
        public float Eccentricity { get; protected set; }
        public float ArgumentOfPeriapsis { get; protected set; }
        public float LongitudeOfAscendingNode { get; protected set; }
        public float Inclination { get; protected set; }
        public float MeanAnomaly { get; protected set; }

        //Extras
        public float SemiMinorAxis { get { return SemiMajorAxis * (float)Math.Sqrt(1 - Math.Pow(Eccentricity, 2)); }  }
        public float FociToCenter { get { return (float)Math.Sqrt(Math.Pow(SemiMajorAxis, 2) - Math.Pow(SemiMinorAxis, 2)); } }
        public float Periapsis { get { return SemiMajorAxis * (1 - Eccentricity); } }
        public float Apoapsis { get { return SemiMajorAxis * (1 + Eccentricity); } }
        public float Period { get { return (float)(2 * Math.PI * Math.Sqrt(Math.Pow(SemiMajorAxis, 3) / PrimeFocus.GM)); } }
        public float SphereOfInfluence {  get { return PrimeFocus.Parent != null ? SemiMajorAxis * (float)Math.Pow(PrimeFocus.Mass / PrimeFocus.Parent.Mass, 0.4) : float.PositiveInfinity; } }

        public Conic(float SemiMajorAxis, float Eccentricity, float ArgumentOfPeriapsis, float LongitudeOfAscendingNode, float Inclination, float MeanAnomaly, Body focus)
        {
            this.SemiMajorAxis = SemiMajorAxis;
            this.Eccentricity = Eccentricity;
            this.ArgumentOfPeriapsis = ArgumentOfPeriapsis;
            this.LongitudeOfAscendingNode = LongitudeOfAscendingNode;
            this.Inclination = Inclination;
            this.MeanAnomaly = MeanAnomaly;
            this.PrimeFocus = focus;
        }

        public Conic(Vector2 Position, Vector2 Velocity, Body focus)
        {
            PrimeFocus = focus;

            //Convert to Vector3 so it works with the equation examples
            var r = new Vector3(Position, 0);
            var v = new Vector3(Velocity, 0);

            FromCartesian(r, v, PrimeFocus.GM);
        }

        public Conic(Vector3 Position, Vector3 Velocity, Body b)
        {
            PrimeFocus = b;

            FromCartesian(Position, Velocity, PrimeFocus.GM);
        }

        private void FromCartesian(Vector3 r, Vector3 v, float U)
        {
            //Orbital Momentum vector
            var h = Vector3.Cross(r, v);
            //Eccentricity vector
            var e = (Vector3.Cross(v, h) / U) - (r / r.Length());
            //Node vector
            var n = new Vector3(-h.Y, h.X, 0);

            SemiMajorAxis = 1 / (2 / r.Length() - v.LengthSquared() / U);

            Eccentricity = e.Length();

            ArgumentOfPeriapsis = (float)(e.Z >= 0 ? Math.Acos(Vector3.Dot(e, n) / (e.Length() * n.Length())) : 2 * Math.PI - Math.Acos(Vector3.Dot(e, n) / (e.Length() * n.Length())));
            if (float.IsNaN(ArgumentOfPeriapsis)) ArgumentOfPeriapsis = (float)(n.Z >= 0 ? Math.Atan2(e.Y, e.X) : 2 * Math.PI - Math.Atan2(e.Y, e.X));
            
            LongitudeOfAscendingNode = (float)(n.Y >= 0 ? Math.Acos(n.X / n.Length()) : 2 * Math.PI - Math.Acos(n.X / n.Length()));
            if (float.IsNaN(LongitudeOfAscendingNode)) LongitudeOfAscendingNode = 0f;

            Inclination = (float)Math.Acos(h.Z / h.Length());

            //True anomaly
            var V = (float)(Vector3.Dot(r, v) >= 0 ? Math.Acos(Vector3.Dot(e, r) / (e.Length() * r.Length())) : 2 * Math.PI - Math.Acos(Vector3.Dot(e, r) / (e.Length() * r.Length())));
            //Eccentric Anomaly
            var E = (float)(2 * Math.Atan(Math.Tan(V / 2) / Math.Sqrt(((1 + Eccentricity) / (1 - Eccentricity)))));
            MeanAnomaly = E - Eccentricity * (float)Math.Sin(E);            
        }

        public void ToCartesian(float dT, out Vector3 r, out Vector3 v)
        {
            //Mean Anomaly M(t)
            var MdT = MeanAnomaly + dT * Math.Sqrt(PrimeFocus.GM / Math.Pow(SemiMajorAxis, 3));
            MdT = (MdT + 2 * Math.PI) % (2 * Math.PI);

            //Ecccentric anomaly
            var E0 = Eccentricity > 0.8 ? Math.PI : MdT;
            
            //Netwon-Raphson method
            var FuncE = new Func<double,double>((Ej) => Ej - (Ej - Eccentricity * Math.Sin(Ej) - MdT) / (1 - Eccentricity * Math.Cos(Ej)));
            var Edt = FuncE(E0);

            //True Anomaly
            var VdT = 2 * Math.Atan2(Math.Sqrt(1 + Eccentricity) * Math.Sin(Edt / 2), Math.Sqrt(1 - Eccentricity) * Math.Cos(Edt / 2));

            //Distance to central body
            var rc = SemiMajorAxis * (1 - Eccentricity * Math.Cos(Edt));

            //Calc position
            r = (float)rc * new Vector3((float)Math.Cos(VdT), (float)Math.Sin(VdT), 0);

            //Calc velocity
            v = (float)(Math.Sqrt(PrimeFocus.GM * SemiMajorAxis) / rc) * new Vector3((float)-Math.Sin(Edt), (float)(Math.Sqrt(1 - Math.Pow(Eccentricity, 2)) * Math.Cos(Edt)), 0);

            //r = v = new Vector3();
        }
    }
}
