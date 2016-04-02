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
        public double SemiMajorAxis { get; protected set; }
        public double Eccentricity { get; protected set; }
        public double ArgumentOfPeriapsis { get; protected set; }
        public double LongitudeOfAscendingNode { get; protected set; }
        public double Inclination { get; protected set; }
        public double MeanAnomaly { get; protected set; }

        //Extras
        public double SemiMinorAxis { get { return SemiMajorAxis * (float)Math.Sqrt(1 - Math.Pow(Eccentricity, 2)); } }
        public double FociToCenter { get { return (float)Math.Sqrt(Math.Pow(SemiMajorAxis, 2) - Math.Pow(SemiMinorAxis, 2)); } }
        public double Periapsis { get { return SemiMajorAxis * (1 - Eccentricity); } }
        public double Apoapsis { get { return SemiMajorAxis * (1 + Eccentricity); } }
        public double Period { get { return (float)(2 * Math.PI * Math.Sqrt(Math.Pow(SemiMajorAxis, 3) / PrimeFocus.GM)); } }
        public double SphereOfInfluence { get { return PrimeFocus.Parent != null ? SemiMajorAxis * (float)Math.Pow(PrimeFocus.Mass / PrimeFocus.Parent.Mass, 0.4) : float.PositiveInfinity; } }

        public Conic(double SemiMajorAxis, double Eccentricity, double ArgumentOfPeriapsis, double LongitudeOfAscendingNode, double Inclination, double MeanAnomaly, Body focus)
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

        public void ToCartesian(float dT, out Vector3 r, out Vector3 v)
        {
            var MdT = MeanAnomalyAtTime(dT);
            var EdT = EccentricityAnomalyFromMeanAnomaly(MdT);
            var VdT = TrueAnomalyFromEccentricAnomaly(EdT);
            var RdT = OrbitalRadiusFromEccentricAnomaly(EdT);

            //Calc position
            var or = (float)RdT * PositionVectorFromTrueAnomaly(VdT);

            //Calc velocity
            var ov = (float)(Math.Sqrt(PrimeFocus.GM * SemiMajorAxis) / RdT) * new Vector3((float)-Math.Sin(EdT), (float)(Math.Sqrt(1 - Math.Pow(Eccentricity, 2)) * Math.Cos(EdT)), 0);

            var f = new Func<Vector3, Vector3>(O => new Vector3(
                   (float)(O.X * (Math.Cos(ArgumentOfPeriapsis) * Math.Cos(LongitudeOfAscendingNode) - Math.Sin(ArgumentOfPeriapsis) * Math.Cos(Inclination) * Math.Sin(LongitudeOfAscendingNode)) - O.Y * (Math.Sin(ArgumentOfPeriapsis) * Math.Cos(LongitudeOfAscendingNode) + Math.Cos(ArgumentOfPeriapsis) * Math.Cos(Inclination) * Math.Sin(LongitudeOfAscendingNode))),
                   (float)(O.X * (Math.Cos(ArgumentOfPeriapsis) * Math.Sin(LongitudeOfAscendingNode) + Math.Sin(ArgumentOfPeriapsis) * Math.Cos(Inclination) * Math.Cos(LongitudeOfAscendingNode)) + O.Y * (Math.Cos(ArgumentOfPeriapsis) * Math.Cos(Inclination) * Math.Cos(LongitudeOfAscendingNode) - Math.Sin(ArgumentOfPeriapsis) * Math.Sin(LongitudeOfAscendingNode))),
                   (float)(O.X * (Math.Sin(ArgumentOfPeriapsis) * Math.Sin(Inclination)) + O.Y * (Math.Cos(ArgumentOfPeriapsis) * Math.Sin(Inclination)))));

            r = f(or);
            v = f(ov);
        }

        public double MeanAnomalyAtTime(float dT)
        {
            //Mean Anomaly M(t)
            var MdT = MeanAnomaly + dT * Math.Sqrt(PrimeFocus.GM / Math.Pow(SemiMajorAxis, 3));
            return MdT = (MdT + 2 * Math.PI) % (2 * Math.PI);
        }

        public double EccentricityAnomalyFromMeanAnomaly(double MdT)
        {
            //Ecccentric anomaly
            var E0 = Eccentricity > 0.8 ? Math.PI : MdT;

            //Netwon-Raphson method
            var FuncE = new Func<double, double>((Ej) => Ej - (Ej - Eccentricity * Math.Sin(Ej) - MdT) / (1 - Eccentricity * Math.Cos(Ej)));
            var Edt = FuncE(E0);
            Edt = FuncE(Edt);

            return Edt;
        }

        public double TrueAnomalyFromEccentricAnomaly(double Edt)
        {
            //True Anomaly
            //var VdT = Math.Atan2(Math.Sqrt(1 - Math.Pow(Eccentricity, 2)) * Math.Sin(Edt), Math.Cos(Edt)-Eccentricity);
            return 2 * Math.Atan2(Math.Sqrt(1.0 + Eccentricity) * Math.Sin(Edt / 2), Math.Sqrt(1 - Eccentricity) * Math.Cos(Edt / 2));
        }

        public Vector3 PositionVectorFromTrueAnomaly(double VdT)
        {
            return new Vector3((float)Math.Cos(VdT), (float)Math.Sin(VdT), 0);
        }

        public double OrbitalRadiusFromEccentricAnomaly(double Edt)
        {
            return SemiMajorAxis * (1 - Eccentricity * Math.Cos(Edt));
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

            ArgumentOfPeriapsis = (e.Z >= 0 ? Math.Acos(Vector3.Dot(e, n) / (e.Length() * n.Length())) : 2 * Math.PI - Math.Acos(Vector3.Dot(e, n) / (e.Length() * n.Length())));
            if (double.IsNaN(ArgumentOfPeriapsis)) ArgumentOfPeriapsis = (h.Z >= 0 ? Math.Atan2(e.Y, e.X) : 2 * Math.PI - Math.Atan2(e.Y, e.X));

            LongitudeOfAscendingNode = (n.Y >= 0 ? Math.Acos(n.X / n.Length()) : 2 * Math.PI - Math.Acos(n.X / n.Length()));
            if (double.IsNaN(LongitudeOfAscendingNode)) LongitudeOfAscendingNode = 0;

            Inclination = Math.Acos(h.Z / h.Length());

            //True anomaly
            var V = (Vector3.Dot(r, v) >= 0 ? Math.Acos(Vector3.Dot(e, r) / (float)(e.Length() * r.Length())) : 2 * Math.PI - Math.Acos(Vector3.Dot(e, r) / (float)(e.Length() * r.Length())));
            //Eccentric Anomaly
            var E = (2 * Math.Atan(Math.Tan(V / 2) / Math.Sqrt(((1 + Eccentricity) / (1 - Eccentricity)))));
            MeanAnomaly = E - Eccentricity * Math.Sin(E);
            MeanAnomaly = (MeanAnomaly + 2 * Math.PI) % (2 * Math.PI);
        }
    }
}
