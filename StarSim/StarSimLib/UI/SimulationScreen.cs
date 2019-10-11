using SFML.Graphics;

using StarSimLib.Configuration;
using StarSimLib.Data_Structures;
using StarSimLib.Graphics;
using StarSimLib.Physics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        /// Allows for writing out the state of the current frame to a file.
        /// </summary>
        private readonly StreamWriter fileWriter;

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
        /// Counts the number of frames elapsed since the beginning of the simulation.
        /// </summary>
        private ulong simulationFramesElapsed;

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

            simulationDrawer = new SimulationDrawer(renderWindow, ref this.bodies, ref this.bodyShapeMap);
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

            fileWriter = CreateFileWriter();
        }

        /// <summary>
        /// The current configuration of the simulation.
        /// </summary>
        public Config Configuration { get; set; }

        /// <summary>
        /// Creates a <see cref="StreamWriter"/> to allow for logging the frame-by-frame state of the simulation to a file.
        /// </summary>
        private StreamWriter CreateFileWriter()
        {
            try
            {
                string dirPath = Path.GetFullPath(Constants.OutputFileDirectory);

                if (!Directory.Exists(dirPath))
                {
                    // ensure that the chosen output directory exists
                    Directory.CreateDirectory(dirPath);
                }

                string fullPath = Path.Combine(dirPath, GetSimulationOutputFileName(dirPath));

                return new StreamWriter(File.Create(fullPath), Encoding.UTF8);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Selected output directory could not be found: {ex.Message}");
                Console.WriteLine(
                    "Could not create new simulation output file. Frame-by-frame state of simulation will not be logged.");
            }

            return null;
        }

        /// <summary>
        /// Returns the formatted name of the simulation file to be created.
        /// </summary>
        /// <param name="dirPath">The path to the output directory.</param>
        /// <returns>The simulation output file name, including a timestamp.</returns>
        private string GetSimulationOutputFileName(string dirPath)
        {
            StringBuilder fileNameBuilder = new StringBuilder("simulation-");

            string timestamp = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}";
            fileNameBuilder.Append(timestamp);

            uint previousFileCount = 0;

            foreach (string fileName in Directory.EnumerateFiles(dirPath))
            {
                if (fileName.Contains(timestamp))
                {
                    previousFileCount++;
                }
            }

            if (previousFileCount > 0)
            {
                fileNameBuilder.Append($"-{previousFileCount}");
            }

            fileNameBuilder.Append(".txt");

            return fileNameBuilder.ToString();
        }

        /// <inheritdoc />
        protected override void CleanupScreen(RenderTarget renderTarget, RenderStates renderStates)
        {
            miscTimer.Stop();

            if (fileWriter != null)
            {
                fileWriter.Flush();
                fileWriter.Close();
            }
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
                if (fileWriter != null)
                {
                    fileWriter.WriteLine($"FRAME: {simulationFramesElapsed}, ELAPSED TIME: {simulationFramesElapsed * Constants.TimeStep}");

                    // write out the unique id of the body and its state to the currently open file
                    foreach (Body body in bodies)
                    {
                        fileWriter.WriteLine($"{body.Generation,2:D}.{body.Id,-4:D}\t" +
                                             $"{body.Mass,21}\t" +
                                             $"[{body.Position.X,21}, {body.Position.Y,21}, {body.Position.Z,21}]\t" +
                                             $"[{body.Velocity.X,21}, {body.Velocity.Y,21}, {body.Velocity.Z,21}]\t" +
                                             $"[{body.Force.X,21}, {body.Force.Y,21}, {body.Force.Z,21}]");
                    }
                }

                bodyPositionUpdater(bodies, Constants.TimeStep);

                simulationFramesElapsed++;
            }

            simulationDrawer.DrawBodies();

            // increment the fps counter and global frame counter
            framesElapsed++;
        }

        /// <inheritdoc />
        protected override void SetupScreen(RenderTarget renderTarget, RenderStates renderStates)
        {
            miscTimer.Start();
        }
    }
}