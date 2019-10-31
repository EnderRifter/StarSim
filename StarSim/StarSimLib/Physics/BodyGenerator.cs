using SFML.Graphics;
using SFML.System;

using StarSimLib.Data_Structures;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StarSimLib.Physics
{
    /// <summary>
    /// Takes the mass of a <see cref="Body"/> instance and returns a colour for the <see cref="CircleShape"/>
    /// that will represent the <see cref="Body"/> of the given mass.
    /// </summary>
    /// <returns>The colour of the <see cref="CircleShape"/> for a <see cref="Body"/> of the given mass.</returns>
    public delegate Color MassToColourDelegate(double mass);

    /// <summary>
    /// Takes the mass of a <see cref="Body"/> instance and returns a radius for the <see cref="CircleShape"/>
    /// that will represent the <see cref="Body"/> of the given mass.
    /// </summary>
    /// <returns>The radius of the <see cref="CircleShape"/> for a <see cref="Body"/> of the given mass.</returns>
    public delegate float MassToRadiusDelegate(double mass);

    /// <summary>
    /// Provides methods for generating <see cref="Body"/> instances.
    /// </summary>
    public static class BodyGenerator
    {
        /// <summary>
        /// Caches a random number generator to use for all randomised positions and velocities.
        /// </summary>
        private static readonly Random Rng = new Random();

        /// <summary>
        /// Default implementation of the <see cref="MassToColourDelegate"/>.
        /// </summary>
        public static readonly MassToColourDelegate DefaultColourDelegate =
            mass => mass >= Constants.CentralBodyMass ? Color.Red : Color.White;

        /// <summary>
        /// Default implementation of the <see cref="MassToRadiusDelegate"/>.
        /// </summary>
        public static readonly MassToRadiusDelegate DefaultRadiusDelegate =
            mass => mass >= Constants.CentralBodyMass ? 4f : 2f;

        /// <summary>
        /// Implementation of the <see cref="MassToColourDelegate"/> that creates a range of colours for
        /// bodies of differing mass.
        /// </summary>
        public static readonly MassToColourDelegate RainbowColourDelegate =
            mass => ColorFromHsv(mass % 360, 1, 1);

        /// <summary>
        /// The current generation of <see cref="Body"/> instances that the generator is on.
        /// </summary>
        public static uint CurrentGeneration { get; private set; }

        /// <summary>
        /// Creates a new ARGB colour from a HSV colour.
        /// </summary>
        /// <param name="hue">The hue value for the new colour, between 0-360.</param>
        /// <param name="saturation">The saturation value for the new colour, between 0-1.</param>
        /// <param name="value">The value for the new colour, between 0-1.</param>
        /// <returns>The created ARGB colour.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Color ColorFromHsv(double hue, double saturation, double value)
        {
            double c = value * saturation;
            double k = hue / 60;
            double x = c * (1 - Math.Abs(k % 2 - 1));
            double m = value - c;

            double baseRed = 0, baseGreen = 0, baseBlue = 0;

            if (0 <= k && k <= 1)
            {
                baseRed = c;
                baseGreen = x;
            }
            else if (1 < k && k <= 2)
            {
                baseRed = x;
                baseGreen = c;
            }
            else if (2 < k && k <= 3)
            {
                baseGreen = c;
                baseBlue = x;
            }
            else if (3 < k && k <= 4)
            {
                baseGreen = x;
                baseBlue = c;
            }
            else if (4 < k && k <= 5)
            {
                baseRed = x;
                baseBlue = c;
            }
            else if (5 < k && k <= 6)
            {
                baseRed = c;
                baseBlue = x;
            }

            return new Color((byte)((baseRed + m) * 255), (byte)((baseGreen + m) * 255), (byte)((baseBlue + m) * 255), 255);
        }

        /// <summary>
        /// Generates an array of <see cref="Body"/> instances, possibly including a central attractor.
        /// </summary>
        /// <param name="bodyCount">The number of <see cref="Body"/> instances to generate.</param>
        /// <param name="centralAttractor">Whether to have a central massive attractor.</param>
        /// <returns>The generated <see cref="Body"/> array.</returns>
        public static Body[] GenerateBodies(int bodyCount = 2, bool centralAttractor = false)
        {
            CurrentGeneration++;

            uint id = 0;
            Body[] generatedBodies = new Body[bodyCount];

            for (int i = 0; i < generatedBodies.Length; i++)
            {
                float mass = (float)(Rng.NextDouble() * Constants.SolarMass);

                Vector4 position = OrbitGenerator.RandomPosition();
                Vector4 velocity = OrbitGenerator.RandomOrbit(position);

                generatedBodies[i] = new Body(position, velocity, mass, CurrentGeneration, id);

                id++;
            }

            if (centralAttractor && bodyCount >= 2)
            {
                generatedBodies[0] = new Body(new Vector4(), new Vector4(), Constants.CentralBodyMass, CurrentGeneration, 0);
            }

            return generatedBodies;
        }

        /// <summary>
        /// Generates a <see cref="Dictionary{TKey,TValue}"/> mapping each given <see cref="Body"/> to a
        /// <see cref="CircleShape"/> that represents the body instance in 2D space.
        /// </summary>
        /// <param name="bodies">The <see cref="Body"/> instances for which to generate the shapes.</param>
        /// <returns>
        /// A <see cref="Dictionary{TKey,TValue}"/> mapping the given <see cref="Body"/> instances to their <see cref="CircleShape"/> instances.
        /// </returns>
        public static Dictionary<Body, CircleShape> GenerateShapes(IEnumerable<Body> bodies) =>
            GenerateShapes(bodies, DefaultRadiusDelegate, DefaultColourDelegate);

        /// <summary>
        /// Generates a <see cref="Dictionary{TKey,TValue}"/> mapping each given <see cref="Body"/> to a
        /// <see cref="CircleShape"/> that represents the body instance in 2D space.
        /// </summary>
        /// <param name="bodies">The <see cref="Body"/> instances for which to generate the shapes.</param>
        /// <param name="massToRadiusDelegate">
        /// The function to use to derive the radius of the <see cref="CircleShape"/> that will represent a <see cref="Body"/>
        /// instance from its mass.
        /// </param>
        /// <param name="massToColourDelegate">
        /// The function to use to derive the colour of the <see cref="CircleShape"/> that will represent a <see cref="Body"/>
        /// instance from its mass.
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{TKey,TValue}"/> mapping the given <see cref="Body"/> instances to their <see cref="CircleShape"/> instances.
        /// </returns>
        public static Dictionary<Body, CircleShape> GenerateShapes(IEnumerable<Body> bodies,
            MassToRadiusDelegate massToRadiusDelegate, MassToColourDelegate massToColourDelegate)
        {
            Dictionary<Body, CircleShape> bodyCircleShapeMap = new Dictionary<Body, CircleShape>();

            foreach (Body body in bodies)
            {
                CircleShape shape = new CircleShape(massToRadiusDelegate(body.Mass))
                {
                    FillColor = massToColourDelegate(body.Mass)
                };

                shape.Origin = new Vector2f(shape.Radius, shape.Radius);

                bodyCircleShapeMap.Add(body, shape);
            }

            return bodyCircleShapeMap;
        }
    }
}