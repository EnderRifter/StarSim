using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using SFML.Graphics;
using SFML.Window;
using StarSimLib.Contexts;
using StarSimLib.Data_Structures;
using StarSimLib.Graphics;
using StarSimLib.Physics;

namespace StarSimLib.UI
{
    /// <summary>
    /// Encapsulates the simulation.
    /// </summary>
    public class SimulationScreen
    {
        /// <summary>
        /// The interval between timer refreshes, in milliseconds.
        /// </summary>
        private const double TimerRefreshIntervalMs = 500;

        /// <summary>
        /// Holds all the <see cref="Body"/> instances that should be simulated.
        /// </summary>
        private readonly Body[] bodies;

        /// <summary>
        /// The body position update algorithm to use.
        /// </summary>
        private readonly UpdateDelegate bodyPositionUpdater;

        /// <summary>
        /// Maps a <see cref="Body"/> to the <see cref="CircleShape"/> that represents it, and is drawn to the
        /// screen at the <see cref="Body"/>s position.
        /// </summary>
        private readonly Dictionary<Body, CircleShape> bodyShapeMap;

        /// <summary>
        /// Timer that manages FPS counter and other miscellaneous counters.
        /// </summary>
        private readonly Timer miscTimer;

        /// <summary>
        /// The renderer used to display the <see cref="Body"/> instances on the screen.
        /// </summary>
        private readonly SimulationDrawer simulationDrawer;

        /// <summary>
        /// The input handler to use to provide interactivity to the simulator.
        /// </summary>
        private readonly SimulationInputHandler simulationInputHandler;

        /// <summary>
        /// The SFML.NET window to which everything is rendered.
        /// </summary>
        private readonly RenderWindow window;

        /// <summary>
        /// The current amount of frames per second.
        /// </summary>
        private double fps;

        /// <summary>
        /// Counts the frames elapsed since the last timer pulse, so that the FPS can be tracked.
        /// </summary>
        private uint framesElapsed;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulationScreen"/> class,
        /// </summary>
        public SimulationScreen(ref Body[] bodies, ref Dictionary<Body, CircleShape> bodyShapeMap, UpdateDelegate bodyPositionUpdater)
        {
            // we construct a new window instance, but immediately hide it so that we can configure the rest of the app
            window = new RenderWindow(VideoMode.DesktopMode, "N-Body Simulation: FPS ", Styles.Default, new ContextSettings());
            window.SetVisible(false);

            this.bodies = bodies;
            this.bodyShapeMap = bodyShapeMap;

            this.bodyPositionUpdater = bodyPositionUpdater;

            simulationDrawer = new SimulationDrawer(window, ref bodies, ref bodyShapeMap);
            simulationInputHandler = new SimulationInputHandler(ref bodies, ref simulationDrawer);

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
        /// Runs the screen until it is closed.
        /// </summary>
        public void Run()
        {
            // we reveal the window so that the user may interact with our program
            window.SetVisible(true);
            miscTimer.Start();

            // we configure the window and its close handler, so that we may close the window once we are done with it
            window.SetFramerateLimit(Constants.FrameRate);
            window.Closed += (sender, eventArgs) => ((RenderWindow)sender).Close();

            // we apply event handlers to allow for interactivity inside the window
            window.KeyPressed += simulationInputHandler.HandleKeyPressed;
            window.KeyReleased += simulationInputHandler.HandleKeyReleased;
            window.MouseButtonPressed += simulationInputHandler.HandleMousePressed;
            window.MouseButtonReleased += simulationInputHandler.HandleMouseReleased;
            window.MouseMoved += simulationInputHandler.HandleMouseMoved;
            window.MouseWheelScrolled += simulationInputHandler.HandleMouseScrolled;

            simulationDrawer.DrawBodies();

            while (window.IsOpen)
            {
                window.Clear();
                window.DispatchEvents();

                if (!simulationInputHandler.IsSimulationPaused)
                {
                    bodyPositionUpdater(bodies, Constants.TimeStep);
                }

                simulationDrawer.DrawBodies();

                window.Display();

                // increment the fps counter
                framesElapsed++;
            }

            miscTimer.Stop();
        }
    }
}