using SFML.Graphics;

namespace StarSimLib
{
    /// <summary>
    /// Holds common constants and helper functions.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The frame rate limit for the simulation.
        /// </summary>
        public const uint FrameRate = 60;

        /// <summary>
        /// The number of seconds that each frame represents.
        /// </summary>
        public const double SecondsPerFrame = 1e7f;

        /// <summary>
        /// The default time step for the simulation.
        /// </summary>
        public const double TimeStep = SecondsPerFrame * FrameRate;

        /// <summary>
        /// The gravitational constant (m^3 kg^-1 s^-2).
        /// </summary>
        public const double G = 6.673e-11f;

        /// <summary>
        /// The mass of the sun (1.98892e30f).
        /// </summary>
        public const double SolarMass = 1.98892e30f;

        /// <summary>
        /// Softens the force between <see cref="Body"/>s to avoid infinities.
        /// </summary>
        public const double SofteningFactor = 3e4f;

        /// <summary>
        /// The square of the <see cref="SofteningFactor"/>.
        /// </summary>
        public const double SofteningFactor2 = SofteningFactor * SofteningFactor;

        /// <summary>
        /// The maximum radius within which <see cref="Body"/>s will be placed (1e18f).
        /// </summary>
        public const double UniverseSize = 1e18f;

        /// <summary>
        /// Factor by which to multiply the position of <see cref="Body"/>s to scale them to the screen for displaying.
        /// </summary>
        public const double UniverseScalingFactor = 2500 / UniverseSize;

        /// <summary>
        /// The mass of the central body, if it is included.
        /// </summary>
        public const double CentralBodyMass = SolarMass * 1e6f;
    }
}