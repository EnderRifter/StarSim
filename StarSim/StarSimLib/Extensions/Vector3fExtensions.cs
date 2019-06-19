using System;
using SFML.System;

namespace StarSimLib.Extensions
{
    /// <summary>
    /// Provides additional functionality to the <see cref="Vector3f"/> struct.
    /// </summary>
    public static class Vector3fExtensions
    {
        /// <summary>
        /// Returns the magnitude of the given <see cref="Vector3f"/>.
        /// </summary>
        /// <param name="vector">The vector whose magnitude to return.</param>
        /// <returns>The magnitude of the given <see cref="Vector3f"/>.</returns>
        public static float Magnitude(this Vector3f vector)
        {
            // returns the magnitude of the vector, via pythagoras
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z + vector.Z);
        }
    }
}