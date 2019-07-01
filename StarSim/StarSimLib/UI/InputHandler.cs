using System;
using SFML.Window;
using StarSimLib.Graphics;
using StarSimLib.Physics;

namespace StarSimLib.UI
{
    /// <summary>
    /// Provides user input handling functions.
    /// </summary>
    public class InputHandler
    {
        /// <summary>
        /// The renderer used to display the <see cref="Body"/> instances on the screen.
        /// </summary>
        private readonly Drawer bodyDrawer;

        /// <summary>
        /// Initialises a new instance of the <see cref="InputHandler"/> class.
        /// </summary>
        /// <param name="drawer">
        /// A reference to the renderer used to display the <see cref="Body"/> instances on the screen.
        /// </param>
        public InputHandler(ref Drawer drawer)
        {
            bodyDrawer = drawer;
        }

        /// <summary>
        /// Whether the simulation is paused at any given time.
        /// </summary>
        public bool IsSimulationPaused { get; set; }

        /// <summary>
        /// Handles key presses.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="KeyEventArgs"/> associated with the key press.</param>
        public void HandleKeyPressed(object sender, KeyEventArgs eventArgs)
        {
            string msg = "";

            switch (eventArgs.Code)
            {
                case Keyboard.Key.Space:
                    // toggle the paused state
                    IsSimulationPaused = !IsSimulationPaused;
                    break;

                case Keyboard.Key.W:
                    // rotate the view north
                    bodyDrawer.Rotate(RotationDirection.North, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees anticlockwise in the x axis. " +
                          $"View Rotation: ({bodyDrawer.XAngle},{bodyDrawer.YAngle},{bodyDrawer.ZAngle})";
                    break;

                case Keyboard.Key.A:
                    // rotate the view west
                    bodyDrawer.Rotate(RotationDirection.West, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees clockwise in the y axis. " +
                          $"View Rotation: ({bodyDrawer.XAngle},{bodyDrawer.YAngle},{bodyDrawer.ZAngle})";
                    break;

                case Keyboard.Key.S:
                    // rotate the view south
                    bodyDrawer.Rotate(RotationDirection.South, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees clockwise in the x axis. " +
                          $"View Rotation: ({bodyDrawer.XAngle},{bodyDrawer.YAngle},{bodyDrawer.ZAngle})";
                    break;

                case Keyboard.Key.D:
                    // rotate the view east
                    bodyDrawer.Rotate(RotationDirection.East, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees anticlockwise in the y axis. " +
                          $"View Rotation: ({bodyDrawer.XAngle},{bodyDrawer.YAngle},{bodyDrawer.ZAngle})";
                    break;

                case Keyboard.Key.Q:
                    // rotate the view anti-clockwise
                    bodyDrawer.Rotate(RotationDirection.Anticlockwise, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees anticlockwise in the z axis. " +
                          $"View Rotation: ({bodyDrawer.XAngle},{bodyDrawer.YAngle},{bodyDrawer.ZAngle})";
                    break;

                case Keyboard.Key.E:
                    // rotate the view clockwise
                    bodyDrawer.Rotate(RotationDirection.Clockwise, Constants.EulerRotationStep);
                    msg = $"Rotated by {Constants.EulerRotationStep} degrees anticlockwise in the z axis. " +
                          $"View Rotation: ({bodyDrawer.XAngle},{bodyDrawer.YAngle},{bodyDrawer.ZAngle})";
                    break;

                default:
                    break;
            }

            if (!msg.Equals(""))
            {
                Console.WriteLine(msg);
            }
        }

        /// <summary>
        /// Handles key releases.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="KeyEventArgs"/> associated with the key release.</param>
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

        /// <summary>
        /// Handles motions of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseMoveEventArgs"/> associated with the mouse movement.</param>
        public void HandleMouseMoved(object sender, MouseMoveEventArgs eventArgs)
        {
            Console.WriteLine($"Mouse moved: {eventArgs.X} {eventArgs.Y}");
        }

        /// <summary>
        /// Handles key presses of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseButtonEventArgs"/> associated with the mouse press.</param>
        public void HandleMousePressed(object sender, MouseButtonEventArgs eventArgs)
        {
            Console.WriteLine($"Mouse pressed: {eventArgs.X} {eventArgs.Y}, {eventArgs.Button}");
        }

        /// <summary>
        /// Handles key releases of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseButtonEventArgs"/> associated with the mouse release.</param>
        public void HandleMouseReleased(object sender, MouseButtonEventArgs eventArgs)
        {
            Console.WriteLine($"Mouse released: {eventArgs.X} {eventArgs.Y}, {eventArgs.Button}");
        }

        /// <summary>
        /// Handles scrolling of the mouse wheel.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseWheelScrollEventArgs"/> associated with the mouse scroll.</param>
        public void HandleMouseScrolled(object sender, MouseWheelScrollEventArgs eventArgs)
        {
            if (eventArgs.Delta > 0)
            {
                bodyDrawer.Scale(1 + Constants.ZoomStep);
            }
            else if (eventArgs.Delta < 0)
            {
                bodyDrawer.Scale(1 - Constants.ZoomStep);
            }

            Console.WriteLine($"Current field of view (zoom level): {bodyDrawer.FOV} ({bodyDrawer.ZoomLevel})");
        }
    }
}