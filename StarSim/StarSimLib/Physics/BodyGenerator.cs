using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using StarSimLib.Data_Structures;

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
        /// The current generation of <see cref="Body"/> instances that the generator is on.
        /// </summary>
        public static uint CurrentGeneration { get; private set; }

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