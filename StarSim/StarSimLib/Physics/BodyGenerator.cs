using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace StarSimLib.Physics
{
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
        /// The current generation of <see cref="Body"/> instances that the generator is on.
        /// </summary>
        public static uint CurrentGeneration { get; private set; }

        /// <summary>
        /// Generates an array of <see cref="Body"/> instances, and generates <see cref="CircleShape"/> instances for
        /// each generated body. Returns the generated array and dictionary as a tuple.
        /// </summary>
        /// <param name="bodyCount">The number of <see cref="Body"/> instances to generate.</param>
        /// <param name="centralAttractor">Whether to have a central massive attractor.</param>
        /// <returns>The generated <see cref="Body"/> array, and <see cref="Body"/> to <see cref="CircleShape"/> map.</returns>
        public static (Body[] generatedBodies, Dictionary<Body, CircleShape> bodyCircleShapeMap) GenerateBodies(int bodyCount = 2, bool centralAttractor = false)
        {
            CurrentGeneration++;

            uint id = 0;
            Body[] generatedBodies = new Body[bodyCount];
            Dictionary<Body, CircleShape> bodyCircleShapeMap = new Dictionary<Body, CircleShape>();

            for (int i = 0; i < generatedBodies.Length; i++)
            {
                float mass = (float)(Rng.NextDouble() * Constants.SolarMass);

                Vector3d position = OrbitGenerator.RandomPosition();
                Vector3d velocity = OrbitGenerator.RandomOrbit(position);

                generatedBodies[i] = new Body(position, velocity, mass, CurrentGeneration, id);
                bodyCircleShapeMap.Add(generatedBodies[i], new CircleShape(1) { FillColor = Color.White });

                id++;
            }

            if (centralAttractor && bodyCount >= 2)
            {
                bodyCircleShapeMap.Remove(generatedBodies[0]);
                generatedBodies[0] = new Body(new Vector3d(), new Vector3d(), Constants.CentralBodyMass, CurrentGeneration, 0);
                bodyCircleShapeMap.Add(generatedBodies[0], new CircleShape(4) { FillColor = Color.Red });
            }

            return (generatedBodies, bodyCircleShapeMap);
        }
    }
}