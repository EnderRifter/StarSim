using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
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

            View windowView = new View(new Vector2f(window.Size.X / 2f, window.Size.Y / 2f), new Vector2f(window.Size.X, window.Size.Y));
            window.SetView(windowView);

            bodies = BodyGenerator.GenerateBodies(Constants.BodyCount, true);
            bodyShapeMap = BodyGenerator.GenerateShapes(bodies);
#if DEBUG
            bodyPositionUpdater = BodyUpdater.UpdateBodiesBruteForce;
#else
            bodyPositionUpdater = BodyUpdater.UpdateBodiesBruteForce;
#endif
            bodyDrawer = new Drawer(window, ref bodies, ref bodyShapeMap);
            Rng = new Random();
        }

        /// <summary>
        /// Whether the simulation is paused at any given time.
        /// </summary>
        private static bool IsSimulationPaused { get; set; }

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
                    // toggle the paused state
                    IsSimulationPaused = !IsSimulationPaused;
                    break;

                case Keyboard.Key.W:
                case Keyboard.Key.Up:
                    // rotate the view north
                    break;

                case Keyboard.Key.A:
                case Keyboard.Key.Left:
                    // rotate the view west
                    break;

                case Keyboard.Key.S:
                case Keyboard.Key.Down:
                    // rotate the view south
                    break;

                case Keyboard.Key.D:
                case Keyboard.Key.Right:
                    // rotate the view east
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
            Console.WriteLine($"Mouse moved: {eventArgs.X} {eventArgs.Y}");
        }

        /// <summary>
        /// Handles key presses of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseButtonEventArgs"/> associated with the key press.</param>
        private static void HandleMousePressed(object sender, MouseButtonEventArgs eventArgs)
        {
            Console.WriteLine($"Mouse pressed: {eventArgs.X} {eventArgs.Y}, {eventArgs.Button}");
        }

        /// <summary>
        /// Handles key releases of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseButtonEventArgs"/> associated with the key press.</param>
        private static void HandleMouseReleased(object sender, MouseButtonEventArgs eventArgs)
        {
            Console.WriteLine($"Mouse released: {eventArgs.X} {eventArgs.Y}, {eventArgs.Button}");
        }

        /// <summary>
        /// Handles scrolling of the mouse wheel.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseWheelScrollEventArgs"/> associated with the key press.</param>
        private static void HandleMouseScrolled(object sender, MouseWheelScrollEventArgs eventArgs)
        {
            if (eventArgs.Delta > 0)
            {
                bodyDrawer.Scale(1.1f);
            }
            else if (eventArgs.Delta < 0)
            {
                bodyDrawer.Scale(0.9f);
            }
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
            window.KeyPressed += HandleKeyPressed;
            window.MouseButtonPressed += HandleMousePressed;
            window.MouseButtonReleased += HandleMouseReleased;
            //window.MouseMoved += HandleMouseMoved;
            window.MouseWheelScrolled += HandleMouseScrolled;

            bodyDrawer.DrawBodies();

            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();

                if (!IsSimulationPaused)
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