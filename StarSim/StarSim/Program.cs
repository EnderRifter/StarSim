using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using StarSimLib;
using StarSimLib.Graphics;
using StarSimLib.Physics;

namespace StarSim
{
    /// <summary>
    /// Main program class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The renderer used to display the <see cref="Body"/> instances on the screen.
        /// </summary>
        private static readonly Drawer bodyDrawer;

        /// <summary>
        /// The body position update algorithm to use.
        /// </summary>
        private static readonly UpdateDelegate bodyPositionUpdater;

        /// <summary>
        /// Caches a random number generator to use for all randomised positions and velocities.
        /// </summary>
        private static readonly Random Rng;

        /// <summary>
        /// The SFML.NET window to which everything is rendered.
        /// </summary>
        private static readonly RenderWindow window;

        /// <summary>
        /// Holds all the <see cref="Body"/> instances that should be simulated.
        /// </summary>
        private static Body[] bodies;

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

            (bodies, bodyShapeMap) = BodyGenerator.GenerateBodies(Constants.BodyCount, true);
#if DEBUG
            bodyPositionUpdater = BodyUpdater.UpdateBodiesBruteForce;
#else
            bodyPositionUpdater = BodyUpdater.UpdateBodiesBruteForce;
#endif
            bodyDrawer = new Drawer(window, ref bodies, ref bodyShapeMap);
            Rng = new Random();
        }

        /// <summary>
        /// Handles key presses.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="KeyEventArgs"/> associated with the key press.</param>
        private static void HandleKeyPressed(object sender, KeyEventArgs eventArgs)
        {
            switch (eventArgs.Code)
            {
                case Keyboard.Key.Space:
                    bodyPositionUpdater(bodies, Constants.TimeStep);
                    break;

                case Keyboard.Key.G:
                    (bodies, bodyShapeMap) = BodyGenerator.GenerateBodies(Constants.BodyCount, true);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handles motions of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseMoveEventArgs"/> associated with the key press.</param>
        private static void HandleMouseMoved(object sender, MouseMoveEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Handles key presses of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseButtonEventArgs"/> associated with the key press.</param>
        private static void HandleMousePressed(object sender, MouseButtonEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Handles key releases of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseButtonEventArgs"/> associated with the key press.</param>
        private static void HandleMouseReleased(object sender, MouseButtonEventArgs eventArgs)
        {
        }

        /// <summary>
        /// Entry point for our application.
        /// </summary>
        /// <param name="args">Any command line arguments passed to the program.</param>
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Press 'enter' to continue...");
            Console.ReadLine();

            // we reveal the window so that the user may interact with our program
            window.SetVisible(true);

            // we configure the window and its close handler, so that we may close the window once we are done with it
            window.SetFramerateLimit(Constants.FrameRate);
            window.Closed += (sender, eventArgs) => ((RenderWindow)sender).Close();

            // we apply event handlers to allow for interactivity inside the window
            window.KeyPressed += HandleKeyPressed;
            window.MouseButtonPressed += HandleMousePressed;
            window.MouseButtonReleased += HandleMouseReleased;
            window.MouseMoved += HandleMouseMoved;

            bodyDrawer.DrawBodies();

            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();

                bodyPositionUpdater(bodies, Constants.TimeStep);
                bodyDrawer.DrawBodies();

                window.Display();
            }

            Console.WriteLine("Goodbye World!");
            Console.WriteLine("Press 'enter' to quit...");
            Console.ReadLine();
        }
    }
}