using SFML.Graphics;

using StarSimLib.Data_Structures;
using StarSimLib.Graphics;
using StarSimLib.Physics;

using System.Collections.Generic;
using System.Timers;

namespace StarSimLib.UI
{
    /// <summary>
    /// Encapsulates the simulation.
    /// </summary>
    public class SimulationScreen : Screen
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
        /// The current amount of frames per second.
        /// </summary>
        private double fps;

        /// <summary>
        /// Counts the frames elapsed since the last timer pulse, so that the FPS can be tracked.
        /// </summary>
        private uint framesElapsed;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulationScreen"/> class.
        /// </summary>
        /// <param name="renderWindow">The render window to which to display this instance.</param>
        /// <param name="inputHandler">The input handler for this instance.</param>
        /// <param name="bodies">The bodies managed by this instance.</param>
        /// <param name="bodyShapeMap">The shapes for the bodies managed by this instance.</param>
        /// <param name="bodyPositionUpdater">The body position update delegate for this instance.</param>
        public SimulationScreen(RenderWindow renderWindow, IInputHandler inputHandler, ref Body[] bodies,
            ref Dictionary<Body, CircleShape> bodyShapeMap, UpdateDelegate bodyPositionUpdater) : base(renderWindow, inputHandler)
        {
            this.bodies = bodies;
            this.bodyShapeMap = bodyShapeMap;

            this.bodyPositionUpdater = bodyPositionUpdater;

            simulationDrawer = new SimulationDrawer(renderWindow, ref bodies, ref bodyShapeMap);
            simulationInputHandler = (SimulationInputHandler)inputHandler;
            simulationInputHandler.SetSimulationDrawer(simulationDrawer);

            // constructs a new timer and attaches a timer event handler that updates the fps and window title every interval
            miscTimer = new Timer(TimerRefreshIntervalMs) { AutoReset = true, Enabled = true };
            miscTimer.Elapsed += (sender, args) =>
            {
                fps = framesElapsed / (TimerRefreshIntervalMs / 1000);

                framesElapsed = 0;
                renderWindow.SetTitle($"N-Body Simulator: FPS {fps}");
            };
        }

        #region Overrides of Screen

        /// <inheritdoc />
        protected override void CleanupScreen(RenderTarget renderTarget, RenderStates renderStates)
        {
            miscTimer.Stop();
        }

        /// <inheritdoc />
        protected override void ConstructScreen()
        {
            // cap the frame rate of the screen to limit the speed of the simulation
            renderWindow.SetFramerateLimit(Constants.FrameRate);
        }

        /// <inheritdoc />
        protected override void DrawFrame(RenderTarget renderTarget, RenderStates renderStates)
        {
            if (!simulationInputHandler.IsSimulationPaused)
            {
                bodyPositionUpdater(bodies, Constants.TimeStep);
            }

            simulationDrawer.DrawBodies();

            // increment the fps counter
            framesElapsed++;
        }

        /// <inheritdoc />
        protected override void SetupScreen(RenderTarget renderTarget, RenderStates renderStates)
        {
            miscTimer.Start();
        }

        #endregion Overrides of Screen
    }
}