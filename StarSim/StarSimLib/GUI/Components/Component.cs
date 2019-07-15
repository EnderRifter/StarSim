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
        /// Initialises a new instance of the <see cref="Component"/> class.
        /// </summary>
        /// <param name="position">The position of this instance.</param>
        /// <param name="sprite">The sprite that should represent this instance.</param>
        protected Component(Vector2f position, Sprite sprite)
        {
            this.sprite = sprite;
            this.sprite.Position = position;
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