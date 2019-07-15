using SFML.Graphics;
using SFML.System;

namespace StarSimLib.GUI.Components
{
    /// <summary>
    /// Represents an image.
    /// </summary>
    public class Image : Component
    {
        /// <inheritdoc />
        public Image(Vector2f position, Sprite sprite) : base(position, sprite)
        {
        }
    }
}