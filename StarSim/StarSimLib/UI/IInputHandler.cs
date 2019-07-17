using System;
using SFML.Window;

namespace StarSimLib.UI
{
    /// <summary>
    /// Describes an input handler that allows screen interactivity.
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// The <see cref="Screen"/> instance whose input to manage.
        /// </summary>
        Screen HandledScreen { get; set; }

        /// <summary>
        /// Handles key presses.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="KeyEventArgs"/> associated with the key press.</param>
        void HandleKeyPressed(object sender, KeyEventArgs eventArgs);

        /// <summary>
        /// Handles key releases.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="KeyEventArgs"/> associated with the key release.</param>
        void HandleKeyReleased(object sender, KeyEventArgs eventArgs);

        /// <summary>
        /// Handles motions of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseMoveEventArgs"/> associated with the mouse movement.</param>
        void HandleMouseMoved(object sender, MouseMoveEventArgs eventArgs);

        /// <summary>
        /// Handles key presses of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseButtonEventArgs"/> associated with the mouse press.</param>
        void HandleMousePressed(object sender, MouseButtonEventArgs eventArgs);

        /// <summary>
        /// Handles key releases of the mouse.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseButtonEventArgs"/> associated with the mouse release.</param>
        void HandleMouseReleased(object sender, MouseButtonEventArgs eventArgs);

        /// <summary>
        /// Handles scrolling of the mouse wheel.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="MouseWheelScrollEventArgs"/> associated with the mouse scroll.</param>
        void HandleMouseScrolled(object sender, MouseWheelScrollEventArgs eventArgs);

        /// <summary>
        /// Handles the screen closing.
        /// </summary>
        /// <param name="sender">The <see cref="Window"/> that sent the event.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> associated with the screen close.</param>
        void HandleScreenClosed(object sender, EventArgs eventArgs);
    }
}