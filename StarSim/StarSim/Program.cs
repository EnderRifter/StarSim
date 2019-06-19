using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using StarSimLib;
using StarSimLib.Physics;

namespace StarSim
{
    internal class Program
    {
        /// <summary>
        /// Caches a random number generator to use for all randomised positions and velocities.
        /// </summary>
        private static readonly Random RNG = new Random();

        /// <summary>
        /// Holds all the <see cref="Body"/> instances that should be simulated.
        /// </summary>
        private static Body[] _bodies;

        /// <summary>
        /// The current generation of <see cref="Body"/>s that we have. Increments each time the bodies are regenerated
        /// via <see cref="GenerateBodies(int, bool)"/>.
        /// </summary>
        private static uint _generation;

        /// <summary>
        /// Maps a <see cref="Body"/> to the <see cref="CircleShape"/> that represents it, and is drawn to the
        /// screen at the <see cref="Body"/>s position.
        /// </summary>
        private static Dictionary<Body, CircleShape> BodyShapeMap;

        /// <summary>
        /// Draws each <see cref="Body"/> in the given <see cref="IEnumerable{T}"/> to the given window.
        /// </summary>
        /// <param name="bodies">The collection of <see cref="Body"/>s to draw.</param>
        /// <param name="renderTarget">The target to draw the <see cref="Body"/>s to.</param>
        /// <param name="originOffset">
        /// The offset that has to be applied to the positions of the <see cref="CircleShape"/>, so that they appear
        /// to be in the centre of the screen and not the top left corner.
        /// </param>
        private static void DrawBodies(IEnumerable<Body> bodies, RenderWindow renderTarget, Vector2u originOffset)
        {
            foreach (Body body in bodies)
            {
                // get the cached shape and projects the current bodies 3D position down to a 2D position,
                // which is used as the new position of the shape.
                CircleShape shape = BodyShapeMap[body];

                shape.Position = new Vector2f(
                    (float)(body.Position.X * Constants.UniverseScalingFactor + originOffset.X),
                    (float)(body.Position.Y * Constants.UniverseScalingFactor + originOffset.Y)
                    );

                shape.Draw(renderTarget, RenderStates.Default);
            }
        }

        private static void GenerateBodies(int bodyCount = 2, bool centralAttractor = false)
        {
            _generation++;

            uint id = 0;
            _bodies = new Body[bodyCount];
            BodyShapeMap = new Dictionary<Body, CircleShape>();

            Console.WriteLine($"=========== Generation {_generation} ===========");
            for (int i = 0; i < _bodies.Length; i++)
            {
                float mass = (float)(RNG.NextDouble() * Constants.SolarMass);

                Vector3d position = OrbitGenerator.RandomPosition();
                Vector3d velocity = OrbitGenerator.RandomOrbit(position); //new Vector3d();

                _bodies[i] = new Body(position, velocity, mass, _generation, id);
                BodyShapeMap.Add(_bodies[i], new CircleShape(1) { FillColor = Color.White });

                Console.WriteLine(_bodies[i]);
                id++;
            }

            if (centralAttractor && bodyCount >= 2)
            {
                BodyShapeMap.Remove(_bodies[0]);
                _bodies[0] = new Body(new Vector3d(), new Vector3d(), Constants.CentralBodyMass, _generation, 0);
                BodyShapeMap.Add(_bodies[0], new CircleShape(4) { FillColor = Color.Red });
            }
        }

        /// <summary>
        /// Handles key presses.
        /// </summary>
        /// <param name="sender">The <see cref="RenderWindow"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="KeyEventArgs"/> about the key press.</param>
        private static void HandleKeyPressed(object sender, KeyEventArgs eventArgs)
        {
            switch (eventArgs.Code)
            {
                case Keyboard.Key.Space:
                    UpdateBodiesBruteForce(_bodies, Constants.TimeStep);
                    break;

                case Keyboard.Key.G:
                    GenerateBodies(10, true);
                    break;

                default:
                    break;
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            GenerateBodies(Constants.BodyCount, true);

            Console.WriteLine("Press 'enter' to continue...");
            Console.ReadLine();

            RenderWindow window = new RenderWindow(VideoMode.DesktopMode, "N-Body Simulator", Styles.Default);
            window.SetFramerateLimit(Constants.FrameRate);
            window.Closed += (sender, eventArgs) => ((RenderWindow)sender).Close();
            window.KeyPressed += HandleKeyPressed;

            // caches the delta between the screen origin (top left corner) and the world origin,
            // which should be the centre of the screen
            uint wx = window.Size.X, wy = window.Size.Y;
            uint dx = wx / 2, dy = wy / 2;
            Vector2u originOffset = new Vector2u(dx, dy);

            DrawBodies(_bodies, window, originOffset);

            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();

                UpdateBodiesBruteForce(_bodies, Constants.TimeStep);
                DrawBodies(_bodies, window, originOffset);

                window.Display();
            }
        }

        /// <summary>
        /// Updates the positions of all the given <see cref="Body"/>s with O(n^2) time complexity, with the given time step.
        /// </summary>
        /// <param name="bodies">The collection of <see cref="Body"/>s whose positions to update.</param>
        /// <param name="deltaTime">The time step.</param>
        private static void UpdateBodiesBruteForce(IEnumerable<Body> bodies, double deltaTime)
        {
            IEnumerable<Body> bodyEnumerable = bodies as Body[] ?? bodies.ToArray();
            Vector3d forceVector = new Vector3d();

            foreach (Body body in bodyEnumerable)
            {
                // resets the force vector to avoid another instantiation and allocation
                forceVector.X = 0;
                forceVector.Y = 0;
                forceVector.Z = 0;

                // use LINQ expression as it is more concise; sum attraction vectors for all other bodies
                forceVector = bodyEnumerable.Where(b => b != body).Aggregate(forceVector, (current, b) => current + Body.GetForceBetween(body, b));

                body.Update(forceVector, deltaTime);
            }
        }
    }
}