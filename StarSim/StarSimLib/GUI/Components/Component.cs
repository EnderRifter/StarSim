using SFML.Graphics;
using SFML.System;

namespace StarSimLib.GUI.Components
{
    /// <summary>
    /// Base class for all GUI components, such as text boxes, buttons, etc.
    /// </summary>
    public abstract class Component : Drawable
    {
        /// <summary>
        /// The sprite representing this component.
        /// </summary>
        private readonly Sprite sprite;

        /// <summary>
        /// The rectangular entity used for this instances sprite.
        /// </summary>
        private IntRect intRect;

        /// <summary>
        /// The texture used for this instances sprite.
        /// </summary>
        private Texture texture;

        /// <summary>
        /// Initialises a new instance of the <see cref="Component"/> class.
        /// </summary>
        /// <param name="position">The position of this instance.</param>
        /// <param name="size">The size of this instance.</param>
        /// <param name="texture">The texture of this instance.</param>
        protected Component(Vector2i position, Vector2i size, Texture texture)
        {
            intRect = new IntRect(position, size);
            this.texture = texture;
            sprite = new Sprite(this.texture, intRect);
        }

        /// <summary>
        /// The position of this component.
        /// </summary>
        public Vector2f Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        /// <summary>
        /// Draws this component to the given <see cref="RenderTarget"/>, with the given <see cref="RenderStates"/>.
        /// </summary>
        /// <param name="renderTarget">The <see cref="RenderTarget"/> to which to draw this instance.</param>
        /// <param name="renderStates">The <see cref="RenderStates"/> for the drawing action.</param>
        protected virtual void DrawInternal(RenderTarget renderTarget, RenderStates renderStates)
        {
            sprite.Draw(renderTarget, renderStates);
        }

        #region Implementation of Drawable

        /// <inheritdoc />
        public void Draw(RenderTarget target, RenderStates states)
        {
            DrawInternal(target, states);
        }

        #endregion Implementation of Drawable
    }
}