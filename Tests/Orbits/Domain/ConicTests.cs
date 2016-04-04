using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Orbits.Domain;
using System;

namespace Tests.Orbits.Domain
{
    [TestClass]
    public class ConicTests
    {
        Body Terra;

        //Conic Cicular;
        //Conic Elliptical;

        [TestInitialize]
        public void Setup()
        {
            Terra = new StationaryBody("Terra", 5.972e24F, 5e6F, new TimeSpan(23, 56, 4));

            //Cicular = new Conic(Earth.Radius + 45e6F, 0, 0, 0, 0, 0, Earth);
            //Elliptical = new Conic(Earth.Radius + 45e6F, 0.2f, 0, 0, 0, 0, Earth);
        }

        [TestMethod]
        public void CheckCicularOrbit()
        {
            var Cicular = new Conic(Terra.Radius + 45e6F, 0, 0, 0, 0, 0, Terra); //50,000km orbit
            var accuracy = 1e-6d;

            //Check the basic otuputs
            Assert.AreEqual(Cicular.Apoapsis, Cicular.Periapsis);
            Assert.AreEqual(Cicular.SemiMajorAxis, Cicular.SemiMinorAxis);
            Assert.AreEqual(Cicular.SemiMajorAxis, Cicular.Periapsis);

            //Mean Anomaly
            Assert.AreEqual(0.0d, Cicular.MeanAnomalyAtTime(0), accuracy);
            Assert.AreEqual(Math.PI / 4, Cicular.MeanAnomalyAtTime((float)Cicular.Period / 8), accuracy);
            Assert.AreEqual(Math.PI / 2, Cicular.MeanAnomalyAtTime((float)Cicular.Period / 4), accuracy);
            Assert.AreEqual(Math.PI, Cicular.MeanAnomalyAtTime((float)Cicular.Period / 2), accuracy);
            Assert.AreEqual(2 * Math.PI, Cicular.MeanAnomalyAtTime((float)Cicular.Period), accuracy);
            Assert.AreEqual(Math.PI, Cicular.MeanAnomalyAtTime((float)(Cicular.Period + Cicular.Period / 2)), accuracy);
            Assert.AreEqual(2 * Math.PI, Cicular.MeanAnomalyAtTime((float)(2 * Cicular.Period)), accuracy);

            //Eccentricity Anomaly
            Assert.AreEqual(0.0d, Cicular.EccentricityAnomalyFromMeanAnomaly(0), accuracy);
            Assert.AreEqual(Math.PI / 4, Cicular.EccentricityAnomalyFromMeanAnomaly(Math.PI / 4), accuracy);
            Assert.AreEqual(Math.PI / 2, Cicular.EccentricityAnomalyFromMeanAnomaly(Math.PI / 2), accuracy);
            Assert.AreEqual(Math.PI, Cicular.EccentricityAnomalyFromMeanAnomaly(Math.PI), accuracy);
            Assert.AreEqual(2 * Math.PI, Cicular.EccentricityAnomalyFromMeanAnomaly(2 * Math.PI), accuracy);

            //True Anomaly
            Assert.AreEqual(0.0d, Cicular.TrueAnomalyFromEccentricAnomaly(0), accuracy);
            Assert.AreEqual(Math.PI / 4, Cicular.TrueAnomalyFromEccentricAnomaly(Math.PI / 4), accuracy);
            Assert.AreEqual(Math.PI / 2, Cicular.TrueAnomalyFromEccentricAnomaly(Math.PI / 2), accuracy);
            Assert.AreEqual(Math.PI, Cicular.TrueAnomalyFromEccentricAnomaly(Math.PI), accuracy);
            Assert.AreEqual(2 * Math.PI, Cicular.TrueAnomalyFromEccentricAnomaly(2 * Math.PI), accuracy);

            //Distance to center
            Assert.AreEqual(Cicular.Periapsis, Cicular.OrbitalRadiusFromEccentricAnomaly(0), accuracy);
            Assert.AreEqual(Cicular.SemiMajorAxis, Cicular.OrbitalRadiusFromEccentricAnomaly(0), accuracy);
            Assert.AreEqual(Cicular.SemiMinorAxis, Cicular.OrbitalRadiusFromEccentricAnomaly(Math.PI / 2), accuracy);
            Assert.AreEqual(Cicular.Apoapsis, Cicular.OrbitalRadiusFromEccentricAnomaly(Math.PI), accuracy);

            var p1 = Cicular.PositionVectorFromTrueAnomaly(0.0d);
            Assert.AreEqual(1, p1.X, accuracy);
            Assert.AreEqual(0, p1.Y, accuracy);

            var p3 = Cicular.PositionVectorFromTrueAnomaly(Math.PI / 2);
            Assert.AreEqual(0, p3.X, accuracy);
            Assert.AreEqual(1, p3.Y, accuracy);

            var p4 = Cicular.PositionVectorFromTrueAnomaly(Math.PI);
            Assert.AreEqual(-1, p4.X, accuracy);
            Assert.AreEqual(0, p4.Y, accuracy);

            var p5 = Cicular.PositionVectorFromTrueAnomaly(Math.PI + Math.PI / 2);
            Assert.AreEqual(0, p5.X, accuracy);
            Assert.AreEqual(-1, p5.Y, accuracy);

            var p6 = Cicular.PositionVectorFromTrueAnomaly(2 * Math.PI);
            Assert.AreEqual(1, p6.X, accuracy);
            Assert.AreEqual(0, p6.Y, accuracy);
        }

        [TestMethod]
        public void CheckEllipticalOrbit()
        {
            var Elliptical = new Conic(Terra.Radius + 45e6F, 0.2d, 0, 0, 0, 0, Terra); //50,000km orbit
            var accuracy = 1e-6d;

            Assert.AreEqual(Elliptical.Apoapsis, 60e6F, accuracy);
            Assert.AreEqual(Elliptical.Periapsis, 40e6F, accuracy);
        }

        [TestMethod]
        public void CheckSpecificOrbit()
        {
            //From http://www.castor2.ca/05_OD/01_Gauss/14_Kepler/index.html
            var conic = new Conic(new Vector3(5052458.7f, 1056271.3f, 5011636.6f), new Vector3(3858.9872f, 4276.3114f, -4807.0493f), Terra);
            var accuracy = 1e-3d;

            //Assert.AreEqual(73108163, conic.SemiMajorAxis, accuracy);
            Assert.AreEqual(0.0159858, conic.Eccentricity, accuracy);
            Assert.AreEqual(2.40429914663, conic.ArgumentOfPeriapsis, accuracy);
            Assert.AreEqual(3.68759754395, conic.LongitudeOfAscendingNode, accuracy);
            Assert.AreEqual(1.24002508663, conic.Inclination, accuracy);
            Assert.AreEqual(6.1954373041, conic.MeanAnomaly, accuracy);

            //var test2 = new Conic(73108163f, 0.0159858f, 2.40429914663f, 3.68759754395f, 1.24002508663f, 6.1954373041f, Earth);
            //Vector3 r, v;
            //test2.ToCartesian(0, out r, out v);
        }

        [TestMethod]
        public void CheckTripleConversion()
        {
            var accuracy = 1e-2d;
            Vector3 r, v;
            var Moon = new Body("Moon", 7.35e22F, 3626000, new TimeSpan(27, 7, 43, 14), Terra, new Vector2(0, -376671000), new Vector2(800, 0));
            Moon.Conic.ToCartesian(0, out r, out v);
            //var MoonRedux = new Body("Moon", 7.35e22F, 3626000, Terra, new Vector2(r.X, r.Y), new Vector2(v.X, v.Y));
            //MoonRedux.Conic.ToCartesian(0, out r, out v);

            //Assert.AreEqual(Moon.Conic.SemiMajorAxis, MoonRedux.Conic.SemiMajorAxis, 2);
            //Assert.AreEqual(Moon.Conic.Eccentricity, MoonRedux.Conic.Eccentricity, accuracy);
            //Assert.AreEqual(Moon.Conic.ArgumentOfPeriapsis, MoonRedux.Conic.ArgumentOfPeriapsis, accuracy);
            //Assert.AreEqual(Moon.Conic.LongitudeOfAscendingNode, MoonRedux.Conic.LongitudeOfAscendingNode, accuracy);
            //Assert.AreEqual(Moon.Conic.Inclination, MoonRedux.Conic.Inclination, accuracy);
            //Assert.AreEqual(Moon.Conic.MeanAnomaly, MoonRedux.Conic.MeanAnomaly, accuracy);

            //MoonRedux.Conic.ToCartesian(0, out r, out v);
            Assert.AreEqual(Moon.Position.X, r.X, accuracy);
            Assert.AreEqual(Moon.Position.Y, r.Y, accuracy);
            Assert.AreEqual(Moon.Velocity.X, v.X, accuracy);
            Assert.AreEqual(Moon.Velocity.Y, v.Y, accuracy);
        }
    }
}
