using System;
using System.Collections.Generic;
using System.Threading;
using SFML.Graphics;
using SFML.Window;
using StarSimLib;
using StarSimLib.Data_Structures;
using StarSimLib.Graphics;
using StarSimLib.Physics;
using StarSimLib.UI;

namespace StarSim
{
    /// <summary>
    /// Main program class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Holds all the <see cref="Body"/> instances that should be simulated.
        /// </summary>
        private static readonly Body[] bodies;

        /// <summary>
        /// The renderer used to display the <see cref="Body"/> instances on the screen.
        /// </summary>
        private static readonly Drawer bodyDrawer;

        /// <summary>
        /// The body position update algorithm to use.
        /// </summary>
        private static readonly UpdateDelegate bodyPositionUpdater;

        /// <summary>
        /// The input handler to use to provide interactivity to the simulator.
        /// </summary>
        private static readonly InputHandler inputHandler;

        /// <summary>
        /// Caches a random number generator to use for all randomised positions and velocities.
        /// </summary>
        private static readonly Random Rng;

        /// <summary>
        /// The SFML.NET window to which everything is rendered.
        /// </summary>
        private static readonly RenderWindow window;

        /// <summary>
        /// Maps a <see cref="Body"/> to the <see cref="CircleShape"/> that represents it, and is drawn to the
        /// screen at the <see cref="Body"/>s position.
        /// </summary>
        private static Dictionary<Body, CircleShape> bodyShapeMap;

        /// <summary>
        /// Initialises a new instance of the <see cref="Program"/> class,
        /// </summary>
        static Program()
        {
            // we construct a new window instance, but immediately hide it so that we can configure the rest of the app
            window = new RenderWindow(VideoMode.DesktopMode, "N-Body Simulator", Styles.Default, new ContextSettings());
            window.SetVisible(false);

            bodies = BodyGenerator.GenerateBodies(Constants.BodyCount, true);
            bodyShapeMap = BodyGenerator.GenerateShapes(bodies);

#if DEBUG
            //bodyPositionUpdater = BodyUpdater.UpdateBodiesBruteForce;
            bodyPositionUpdater = BodyUpdater.UpdateBodiesBarnesHut;
#else
            bodyPositionUpdater = BodyUpdater.UpdateBodiesBarnesHut;
#endif

            bodyDrawer = new Drawer(window, ref bodies, ref bodyShapeMap);
            inputHandler = new InputHandler(ref bodies, ref bodyDrawer);
            Rng = new Random();
        }

        /// <summary>
        /// Entry point for our application.
        /// </summary>
        /// <param name="args">Any command line arguments passed to the program.</param>
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Press 'enter' to continue...");

            Console.ReadLine();

            // we reveal the window so that the user may interact with our program
            window.SetVisible(true);

            // we configure the window and its close handler, so that we may close the window once we are done with it
            window.SetFramerateLimit(Constants.FrameRate);
            window.Closed += (sender, eventArgs) => ((RenderWindow)sender).Close();

            // we apply event handlers to allow for interactivity inside the window
            window.KeyPressed += inputHandler.HandleKeyPressed;
            //window.KeyReleased += inputHandler.HandleKeyReleased;
            window.MouseButtonPressed += inputHandler.HandleMousePressed;
            window.MouseButtonReleased += inputHandler.HandleMouseReleased;
            //window.MouseMoved += inputHandler.HandleMouseMoved;
            window.MouseWheelScrolled += inputHandler.HandleMouseScrolled;

            bodyDrawer.DrawBodies();

            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();

                if (!inputHandler.IsSimulationPaused)
                {
                    bodyPositionUpdater(bodies, Constants.TimeStep);
                }

                bodyDrawer.DrawBodies();

                window.Display();
            }

            Console.WriteLine("Goodbye, World!");
            Console.WriteLine("Press 'enter' to quit...");
            Console.ReadLine();
        }
    }
}