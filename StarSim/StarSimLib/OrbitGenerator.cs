﻿using System;
using SFML.System;
using StarSimLib.Extensions;

namespace StarSimLib
{
    /// <summary>
    /// Initialises positions, velocities, and orbits for bodies.
    /// </summary>
    public static class OrbitGenerator
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
        private static readonly Random Rng = new Random();

        /// <summary>
        /// Returns a random position within the universe sphere (see <see cref="Constants.UniverseSize"/>).
        /// </summary>
        /// <returns>A 3D position vector.</returns>
        public static Vector3d RandomPosition()
        {
            double RandomPos()
            {
                return Constants.UniverseSize * Math.Exp(-1.8) * (.5 - Rng.NextDouble());
            }

            return new Vector3d(RandomPos(), RandomPos(), RandomPos());
        }

        /// <summary>
        /// Returns a randomised orbit around a central, heavy mass.
        /// </summary>
        /// <param name="positionVector">The position vector for the body for which to generate the orbit.</param>
        /// <returns>The magnitude of the velocity of the orbit.</returns>
        public static Vector3d RandomOrbit(Vector3d positionVector)
        {
            Vector3d position = positionVector;

            // F = G m1 - m2 / distance
            double distanceToCentralBody = positionVector.Magnitude();
            double numerator = Constants.G * Constants.SolarMass;
            double velocityMagnitude = Math.Sqrt(numerator / distanceToCentralBody);

            double absAngle = Math.Atan(Math.Abs(position.X / position.Y));
            double velocityTheta = Math.PI / 2 - absAngle;
            double velocityPhi = Rng.NextDouble() * Math.PI;

            double vx = -1 * Math.Sign(position.Y) * Math.Cos(velocityTheta) * velocityMagnitude;
            double vy = Math.Sign(position.X) * Math.Sin(velocityTheta) * velocityMagnitude;
            double vz = 0;

            // Randomly orient the orbit
            return !(Rng.NextDouble() <= 0.5f) ? new Vector3d(vx, vy, vz) : new Vector3d(-vx, -vy, -vz);
        }
    }
}