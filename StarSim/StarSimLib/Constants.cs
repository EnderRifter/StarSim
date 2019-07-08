using System;
using System.Collections.Generic;
using System.Text;
using StarSimLib.Data_Structures;

namespace StarSimLib
{
    /// <summary>
    /// Holds common constants and helper functions.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The amount of bodies that are rendered by default.
        /// </summary>
        public const int BodyCount = 10;

        /// <summary>
        /// The mass of the central body, if it is included.
        /// </summary>
        public const double CentralBodyMass = SolarMass * 1e6;

        /// <summary>
        /// The amount of degrees by which the view will be rotated in any given direction.
        /// </summary>
        public const double EulerRotationStep = 1;

        /// <summary>
        /// The frame rate limit for the program.
        /// </summary>
        public const uint FrameRate = 144;

        /// <summary>
        /// The gravitational constant (m^3 kg^-1 s^-2).
        /// </summary>
        public const double G = 6.673e-11f;

        /// <summary>
        /// The minimum width of a tree. Subtrees are not created when if their width would be smaller than this value,
        /// to prevent widths of NaN as a result of division errors.
        /// </summary>
        public const double MinimumTreeWidth = 1;

        /// <summary>
        /// The number of seconds that each simulation tick represents.
        /// </summary>
        public const double SecondsPerTick = 1e8f;

        /// <summary>
        /// The tick rate limit for the simulation.
        /// </summary>
        public const uint SimulationRate = 60;

        /// <summary>
        /// Softens the force between <see cref="Body"/>s to avoid infinities.
        /// </summary>
        public const double SofteningFactor = 700;

        /// <summary>
        /// The square of the <see cref="SofteningFactor"/>.
        /// </summary>
        public const double SofteningFactor2 = SofteningFactor * SofteningFactor;

        /// <summary>
        /// The mass of the sun (1.98892e30f).
        /// </summary>
        public const double SolarMass = 1.98892e30f;

        /// <summary>
        /// The number of previous positions that will be stored by a body
        /// </summary>
        public const int StoredPreviousPositionCount = 30;

        /// <summary>
        /// The time step for the simulation.
        /// </summary>
        public const double TimeStep = SecondsPerTick * (SimulationRate / (float)FrameRate) * SimulationRate;

        /// <summary>
        /// The tolerance of the mass grouping approximation in the simulation. A body is only accelerated when the
        /// ratio of the tree's width to the distance (from the tree's center of mass to the body) is less than this.
        /// </summary>
        public const double TreeTheta = 0.5;

        /// <summary>
        /// The maximum radius within which <see cref="Body"/>s will be placed (1e18f).
        /// </summary>
        public const double UniverseSize = 1e18f;

        /// <summary>
        /// The amount by which the zoom level will be increased or decreased.
        /// </summary>
        public const double ZoomStep = 0.05;

        /// <summary>
        /// Converts an enumerable to its string form.
        /// </summary>
        /// <typeparam name="T">The type of item held in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable instance to convert.</param>
        /// <param name="itemPrinter">
        /// The function used to convert a single item to a string, defaults to <see cref="object.ToString()"/></param>
        /// <returns>The string representing the given enumerable.</returns>
        public static string ConvertEnumerableToString<T>(IEnumerable<T> enumerable, Func<T, string> itemPrinter = null)
        {
            StringBuilder enumerableStringBuilder = new StringBuilder("[");

            if (itemPrinter == null)
            {
                foreach (T item in enumerable)
                {
                    enumerableStringBuilder.Append($"{item?.ToString() ?? "null"},");
                }
            }
            else
            {
                foreach (T item in enumerable)
                {
                    enumerableStringBuilder.Append($"{itemPrinter(item) ?? "null"},");
                }
            }

            enumerableStringBuilder.Append("]");

            return enumerableStringBuilder.ToString();
        }
    }
}