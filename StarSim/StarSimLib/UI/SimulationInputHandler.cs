using SFML.Window;

using StarSimLib.Data_Structures;
using StarSimLib.Graphics;

using System;

namespace StarSimLib.UI
{
    /// <summary>
    /// Provides user input handling functions for the simulation.
    /// </summary>
    public class SimulationInputHandler : IInputHandler
    {
        /// <summary>
        /// Holds all the <see cref="Body"/> instances that should be simulated.
        /// </summary>
        private readonly Body[] managedBodies;

        /// <summary>
        /// The renderer used to display the <see cref="Body"/> instances on the screen.
        /// </summary>
        private SimulationDrawer simulationDrawer;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulationInputHandler"/> class.
        /// </summary>
        /// <param name="bodies">
        /// A reference to the <see cref="Body"/> instances which should be managed by this instance.
        /// </param>
        public SimulationInputHandler(ref Body[] bodies)
        {
            managedBodies = bodies;
        }

        /// <inheritdoc />
        public Screen HandledScreen { get; set; }

        /// <summary>
        /// Whether the simulation is paused at any given time.
        /// </summary>
        public bool IsSimulationPaused { get; set; }

        /// <summary>
        /// Whether to record previous <see cref="Body"/> instance positions, in order to render an orbit tracer
        /// behind each body instance.
        /// </summary>
        public bool RecordOrbitTracers { get; set; }

        /// <inheritdoc />
        public void HandleKeyPressed(object sender, KeyEventArgs eventArgs)
        {
            string msg = "";

            switch (eventArgs.Code)
            {
                case Keyboard.Key.Space:
                    // toggle the paused state
                    IsSimulationPaused = !IsSimulationPaused;
                    break;

                case Keyboard.Key.T:
                    // toggle orbit tracer recording
                    RecordOrbitTracers = !RecordOrbitTracers;

                    // every simulated body must be individually set to record tracers
                    foreach (Body body in managedBodies)
                    {
                        // we want to clear the screen of any orbit tracers that still exist when we turn orbit tracing
                        // off, so we must clear the previous position queues of every managed object. this has the
                        // effect of clearing the vertex arrays holding the orbit tracer vertices during the next draw step
                        body.RecordPreviousPositions = RecordOrbitTracers;
                        body.OrbitTracer.Clear();
                    }

                    break;

                case Keyboard.Key.W:
                    // rotate the view north
                    simulationDrawer.Rotate(RotationDirection.North, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees anticlockwise in the x axis. " +
                          $"View Rotation: ({simulationDrawer.XAngle},{simulationDrawer.YAngle},{simulationDrawer.ZAngle})";
                    break;

                case Keyboard.Key.A:
                    // rotate the view west
                    simulationDrawer.Rotate(RotationDirection.West, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees clockwise in the y axis. " +
                          $"View Rotation: ({simulationDrawer.XAngle},{simulationDrawer.YAngle},{simulationDrawer.ZAngle})";
                    break;

                case Keyboard.Key.S:
                    // rotate the view south
                    simulationDrawer.Rotate(RotationDirection.South, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees clockwise in the x axis. " +
                          $"View Rotation: ({simulationDrawer.XAngle},{simulationDrawer.YAngle},{simulationDrawer.ZAngle})";
                    break;

                case Keyboard.Key.D:
                    // rotate the view east
                    simulationDrawer.Rotate(RotationDirection.East, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees anticlockwise in the y axis. " +
                          $"View Rotation: ({simulationDrawer.XAngle},{simulationDrawer.YAngle},{simulationDrawer.ZAngle})";
                    break;

                case Keyboard.Key.Q:
                    // rotate the view anti-clockwise
                    simulationDrawer.Rotate(RotationDirection.Anticlockwise, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees anticlockwise in the z axis. " +
                          $"View Rotation: ({simulationDrawer.XAngle},{simulationDrawer.YAngle},{simulationDrawer.ZAngle})";
                    break;

                case Keyboard.Key.E:
                    // rotate the view clockwise
                    simulationDrawer.Rotate(RotationDirection.Clockwise, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees anticlockwise in the z axis. " +
                          $"View Rotation: ({simulationDrawer.XAngle},{simulationDrawer.YAngle},{simulationDrawer.ZAngle})";
                    break;

                case Keyboard.Key.Comma:
                    // decrease the simulation speed
                    break;

                case Keyboard.Key.Period:
                    // increase the simulation speed
                    break;

                default:
                    break;
            }

            if (!msg.Equals(""))
            {
                Console.WriteLine(msg);
            }
        }

        /// <inheritdoc />
        public void HandleKeyReleased(object sender, KeyEventArgs eventArgs)
        {
            string msg = "";

            switch (eventArgs.Code)
            {
                default:
                    break;
            }

            if (!msg.Equals(""))
            {
                Console.WriteLine(msg);
            }
        }

        /// <inheritdoc />
        public void HandleMouseMoved(object sender, MouseMoveEventArgs eventArgs)
        {
            //Console.WriteLine($"Mouse moved: {eventArgs.X} {eventArgs.Y}");
        }

        /// <inheritdoc />
        public void HandleMousePressed(object sender, MouseButtonEventArgs eventArgs)
        {
            //Console.WriteLine($"Mouse pressed: {eventArgs.X} {eventArgs.Y}, {eventArgs.Button}");
        }

        /// <inheritdoc />
        public void HandleMouseReleased(object sender, MouseButtonEventArgs eventArgs)
        {
            //Console.WriteLine($"Mouse released: {eventArgs.X} {eventArgs.Y}, {eventArgs.Button}");
        }

        /// <inheritdoc />
        public void HandleMouseScrolled(object sender, MouseWheelScrollEventArgs eventArgs)
        {
            if (eventArgs.Delta > 0)
            {
                simulationDrawer.Scale(1 + Constants.ZoomStep);
            }
            else if (eventArgs.Delta < 0)
            {
                simulationDrawer.Scale(1 - Constants.ZoomStep);
            }

            Console.WriteLine($"Current field of view (zoom level): {simulationDrawer.FOV} ({simulationDrawer.ZoomLevel})");
        }

        /// <inheritdoc />
        public void HandleScreenClosed(object sender, EventArgs eventArgs)
        {
            ((Window)sender).Close();
        }

        /// <summary>
        /// Sets the simulation drawer.
        /// </summary>
        /// <param name="drawer"></param>
        public void SetSimulationDrawer(SimulationDrawer drawer)
        {
            simulationDrawer = drawer;
        }
    }
}