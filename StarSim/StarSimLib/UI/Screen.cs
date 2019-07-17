using SFML.Graphics;

namespace StarSimLib.UI
{
    /// <summary>
    /// Base class for screens that include user interactivity.
    /// </summary>
    public abstract class Screen
    {
        /// <summary>
        /// The input handler to use for this instance.
        /// </summary>
        protected readonly IInputHandler inputHandler;

        /// <summary>
        /// The window to which this instance is drawn.
        /// </summary>
        protected readonly RenderWindow renderWindow;

        /// <summary>
        /// Initialises a new instance of the <see cref="Screen"/> class.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="inputHandler"></param>
        protected Screen(RenderWindow window, IInputHandler inputHandler)
        {
            renderWindow = window;

            this.inputHandler = inputHandler;
            inputHandler.HandledScreen = this;

            // we apply event handlers to allow for interactivity inside the window
            renderWindow.KeyPressed += this.inputHandler.HandleKeyPressed;
            renderWindow.KeyReleased += this.inputHandler.HandleKeyReleased;
            renderWindow.MouseButtonPressed += this.inputHandler.HandleMousePressed;
            renderWindow.MouseButtonReleased += this.inputHandler.HandleMouseReleased;
            renderWindow.MouseMoved += this.inputHandler.HandleMouseMoved;
            renderWindow.MouseWheelScrolled += this.inputHandler.HandleMouseScrolled;
            renderWindow.Closed += this.inputHandler.HandleScreenClosed;

            renderWindow.SetVisible(false);

            ConstructScreen();
        }

        /// <summary>
        /// Cleans up the contents of the screen to the given <see cref="RenderTarget"/> with the given <see cref="RenderStates"/>.
        /// </summary>
        /// <param name="renderTarget">The <see cref="RenderTarget"/> to which to draw this instance.</param>
        /// <param name="renderStates">The <see cref="RenderStates"/> to use for the drawing.</param>
        protected abstract void CleanupScreen(RenderTarget renderTarget, RenderStates renderStates);

        /// <summary>
        /// Called during the constructor to allow for custom setup, before the child constructor is called.
        /// </summary>
        protected abstract void ConstructScreen();

        /// <summary>
        /// Draws the contents of the screen to the given <see cref="RenderTarget"/> with the given <see cref="RenderStates"/>.
        /// </summary>
        /// <param name="renderTarget">The <see cref="RenderTarget"/> to which to draw this instance.</param>
        /// <param name="renderStates">The <see cref="RenderStates"/> to use for the drawing.</param>
        protected abstract void DrawFrame(RenderTarget renderTarget, RenderStates renderStates);

        /// <summary>
        /// Sets up the contents of the screen to the given <see cref="RenderTarget"/> with the given <see cref="RenderStates"/>.
        /// </summary>
        /// <param name="renderTarget">The <see cref="RenderTarget"/> to which to draw this instance.</param>
        /// <param name="renderStates">The <see cref="RenderStates"/> to use for the drawing.</param>
        protected abstract void SetupScreen(RenderTarget renderTarget, RenderStates renderStates);

        /// <summary>
        /// Displays the screen until it is closed.
        /// </summary>
        public void Run()
        {
            // we reveal the window so that the user may interact with our program
            renderWindow.SetVisible(true);

            // allows custom screen setup code
            SetupScreen(renderWindow, RenderStates.Default);

            while (renderWindow.IsOpen)
            {
                renderWindow.Clear();
                renderWindow.DispatchEvents();

                // allows custom screen content
                DrawFrame(renderWindow, RenderStates.Default);

                renderWindow.Display();
            }

            // allows custom screen cleanup code
            CleanupScreen(renderWindow, RenderStates.Default);
        }
    }
}