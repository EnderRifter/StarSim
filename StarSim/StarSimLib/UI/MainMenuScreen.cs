using SFML.Graphics;
using StarSimLib.GUI;

namespace StarSimLib.UI
{
    /// <summary>
    /// Encapsulates the main menu screen.
    /// </summary>
    public class MainMenuScreen : Screen
    {
        /// <inheritdoc />
        public MainMenuScreen(RenderWindow window, IInputHandler inputHandler) : base(window, inputHandler)
        {
        }

        #region Overrides of Screen

        /// <inheritdoc />
        protected override void CleanupScreen(RenderTarget renderTarget, RenderStates renderStates)
        {
        }

        /// <inheritdoc />
        protected override void ConstructScreen()
        {
        }

        /// <inheritdoc />
        protected override void DrawFrame(RenderTarget renderTarget, RenderStates renderStates)
        {
        }

        /// <inheritdoc />
        protected override void SetupScreen(RenderTarget renderTarget, RenderStates renderStates)
        {
        }

        #endregion Overrides of Screen
    }
}