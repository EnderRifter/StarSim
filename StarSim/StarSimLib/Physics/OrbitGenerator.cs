using System;
using StarSimLib.Data_Structures;

namespace StarSimLib.Physics
{
    /// <summary>
    /// Provides methods for initialising positions, velocities, and orbits for <see cref="Body"/> instances.
    /// </summary>
    public static class OrbitGenerator
    {
        /// <summary>
        /// Random number generator.
        /// </summary>
        private static readonly Random Rng = new Random();

        /// <summary>
        /// Returns a randomised orbit around a central, heavy mass.
        /// </summary>
        /// <param name="positionVector">The position vector for the body for which to generate the orbit.</param>
        /// <returns>The magnitude of the velocity of the orbit.</returns>
        public static Vector4d RandomOrbit(Vector4d positionVector)
        {
            Vector4d position = positionVector;

            // F = G m1 - m2 / distance
            double distanceToCentralBody = position.Magnitude();
            double numerator = Constants.G * Constants.CentralBodyMass;
            double velocityMagnitude = Math.Sqrt(numerator / distanceToCentralBody);

            double absAngle = Math.Atan(Math.Abs(position.X / position.Y));
            double velocityTheta = Math.PI / 2 - absAngle;
            double velocityPhi = Rng.NextDouble() * Math.PI;

            double vertical = Math.Min(2e8 / distanceToCentralBody, 2e4);

            double vx = -1 * Math.Sign(position.Y) * Math.Cos(velocityTheta) * velocityMagnitude;
            double vy = (Rng.NextDouble() - 0.5) * vertical;
            double vz = Math.Sign(position.X) * Math.Sin(velocityTheta) * velocityMagnitude;

            // TODO: Implement 3d velocities and rendering
            // mapping of the 3d values to 2d values
            vy = vz;
            vz = 0;

            // Randomly orient the orbit
            return !(Rng.NextDouble() <= 0.5f) ? new Vector4d(vx, vy, vz) : new Vector4d(-vx, -vy, -vz);
        }

        /// <summary>
        /// Returns a random position within the universe sphere (see <see cref="Constants.UniverseSize"/>).
        /// </summary>
        /// <returns>A 3D position vector.</returns>
        public static Vector4d RandomPosition()
        {
            double RandomPos()
            {
                return Constants.UniverseSize * Math.Exp(-1.8) * (.5 - Rng.NextDouble());
            }

            return new Vector4d(RandomPos(), RandomPos(), RandomPos());
        }
    }
}