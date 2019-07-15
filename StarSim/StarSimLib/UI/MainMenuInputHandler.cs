using System;
using SFML.Window;
using StarSimLib.GUI;

namespace StarSimLib.UI
{
    /// <summary>
    /// Provides user input handling functions for the main menu.
    /// </summary>
    public class MainMenuInputHandler : IInputHandler
    {
        /// <inheritdoc />
        public Screen HandledScreen { get; set; }

        /// <inheritdoc />
        public void HandleKeyPressed(object sender, KeyEventArgs eventArgs)
        {
        }

        /// <inheritdoc />
        public void HandleKeyReleased(object sender, KeyEventArgs eventArgs)
        {
        }

        /// <inheritdoc />
        public void HandleMouseMoved(object sender, MouseMoveEventArgs eventArgs)
        {
        }

        /// <inheritdoc />
        public void HandleMousePressed(object sender, MouseButtonEventArgs eventArgs)
        {
        }

        /// <inheritdoc />
        public void HandleMouseReleased(object sender, MouseButtonEventArgs eventArgs)
        {
        }

        /// <inheritdoc />
        public void HandleMouseScrolled(object sender, MouseWheelScrollEventArgs eventArgs)
        {
        }

        /// <inheritdoc />
        public void HandleScreenClosed(object sender, EventArgs eventArgs)
        {
            ((Window)sender).Close();
        }
    }
}