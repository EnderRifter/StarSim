using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using SFML.Graphics;
using SFML.Window;
using StarSimLib;
using StarSimLib.Contexts;
using StarSimLib.Cryptography;
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
        /// The interval between timer refreshes, in milliseconds.
        /// </summary>
        private const double TimerRefreshIntervalMs = 500;

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
        /// Maps a <see cref="Body"/> to the <see cref="CircleShape"/> that represents it, and is drawn to the
        /// screen at the <see cref="Body"/>s position.
        /// </summary>
        private static readonly Dictionary<Body, CircleShape> bodyShapeMap;

        /// <summary>
        /// The database context to use for the lifetime of the program.
        /// </summary>
        private static readonly SimulatorContext databaseContext;

        /// <summary>
        /// The input handler to use to provide interactivity to the simulator.
        /// </summary>
        private static readonly InputHandler inputHandler;

        /// <summary>
        /// Timer that manages FPS counter and other miscellaneous counters.
        /// </summary>
        private static readonly Timer miscTimer;

        /// <summary>
        /// The SFML.NET window to which everything is rendered.
        /// </summary>
        private static readonly RenderWindow window;

        /// <summary>
        /// The current amount of frames per second.
        /// </summary>
        private static double fps;

        /// <summary>
        /// Counts the frames elapsed since the last timer pulse, so that the FPS can be tracked.
        /// </summary>
        private static uint framesElapsed;

        /// <summary>
        /// Initialises a new instance of the <see cref="Program"/> class,
        /// </summary>
        static Program()
        {
            // set up the database context for the program
            databaseContext = new SimulatorContext();

            // we construct a new window instance, but immediately hide it so that we can configure the rest of the app
            window = new RenderWindow(VideoMode.DesktopMode, "N-Body Simulator: FPS ", Styles.Default, new ContextSettings());
            window.SetVisible(false);

            bodies = BodyGenerator.GenerateBodies(Constants.BodyCount, true);
            bodyShapeMap = BodyGenerator.GenerateShapes(bodies);

#if DEBUG
            bodyPositionUpdater = BodyUpdater.UpdateBodiesBruteForce;
#else
            bodyPositionUpdater = BodyUpdater.UpdateBodiesBarnesHut;
#endif

            bodyDrawer = new Drawer(window, ref bodies, ref bodyShapeMap);
            inputHandler = new InputHandler(ref bodies, ref bodyDrawer);

            // constructs a new timer and attaches a timer event handler that updates the fps and window title every interval
            miscTimer = new Timer(TimerRefreshIntervalMs) { AutoReset = true, Enabled = true };
            miscTimer.Elapsed += (sender, args) =>
            {
                fps = framesElapsed / (TimerRefreshIntervalMs / 1000);

                framesElapsed = 0;
                window.SetTitle($"N-Body Simulator: FPS {fps}");
            };
        }

        /// <summary>
        /// Converts the given enumerable to a string representation of its contents.
        /// </summary>
        /// <typeparam name="T">The type of object held in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to convert.</param>
        /// <param name="itemConverter">The custom function to use to convert a single enumerable item to its string form.</param>
        /// <returns>The contents of the enumerable as a string.</returns>
        private static string EnumerableToString<T>(IEnumerable<T> enumerable, Func<T, string> itemConverter = null)
        {
            StringBuilder enumerableStringBuilder = new StringBuilder("[");

            if (itemConverter == null)
            {
                // the default converter function is just calling the Object.ToString() function
                itemConverter = item => item.ToString();
            }

            foreach (T item in enumerable)
            {
                enumerableStringBuilder.Append($"{itemConverter(item) ?? ""},");
            }

            enumerableStringBuilder.Append("]");

            return enumerableStringBuilder.ToString();
        }

        /// <summary>
        /// Entry point for our application.
        /// </summary>
        private static void Main()
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Press 'enter' to continue...");

            Console.ReadLine();

            // we reveal the window so that the user may interact with our program
            window.SetVisible(true);
            miscTimer.Start();

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

                // increment the fps counter
                framesElapsed++;
            }

            miscTimer.Stop();
            Console.WriteLine("Goodbye, World!");
            Console.WriteLine("Press 'enter' to quit...");
            Console.ReadLine();
        }

        private static void TestHashing(string password)
        {
            // set a shorthand for nicety reasons
            string BytesToString(byte[] contents) => CryptographyHelper.BytesToString(contents);

            Console.WriteLine($"Password to hash: {password}");

            byte[] passwordBytes = CryptographyHelper.StringToBytes(password);
            Console.WriteLine($"Password bytes:\n{BytesToString(passwordBytes)}");

            // generates a salt of the default length
            byte[] saltBytes = CryptographyHelper.GenerateSalt();
            Console.WriteLine($"Generated salt:\n{BytesToString(saltBytes)}");

            // generates a hash of the default length
            byte[] passwordHash = CryptographyHelper.GenerateHash(passwordBytes, saltBytes);
            Console.WriteLine($"Generated valid hash:\n{BytesToString(passwordHash)}");

            byte[] invalidPasswordBytes = CryptographyHelper.StringToBytes("password ");
            Console.WriteLine($"Invalid password bytes:\n{BytesToString(invalidPasswordBytes)}");

            byte[] invalidPasswordHash = CryptographyHelper.GenerateHash(invalidPasswordBytes, saltBytes);
            Console.WriteLine($"Generated invalid hash:\n{BytesToString(invalidPasswordHash)}");

            Console.WriteLine($"Valid hash == invalid hash: {CryptographyHelper.HashesEqual(passwordHash, invalidPasswordHash)}");
        }
    }
}