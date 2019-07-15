using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using StarSimLib.GUI;
using StarSimLib.GUI.Components;

namespace StarSimLib.UI
{
    /// <summary>
    /// Encapsulates the main menu screen.
    /// </summary>
    public class MainMenuScreen : Screen
    {
        private readonly List<Component> components;

        /// <inheritdoc />
        public MainMenuScreen(RenderWindow window, IInputHandler inputHandler) : base(window, inputHandler)
        {
            components = new List<Component>
                         {
                             new GUI.Components.Image(new Vector2f(10, 10), new Sprite(
                                 new Texture(@"resources/chessboard.jpg",
                                     new IntRect(new Vector2i(100, 100), new Vector2i(800, 800))),
                                 new IntRect(new Vector2i(10, 10), new Vector2i(80, 80)))),
                             new GUI.Components.Image(new Vector2f(200, 10), new Sprite(
                                 new Texture(@"resources/chessboard.jpg",
                                     new IntRect(new Vector2i(100, 100), new Vector2i(800, 800))),
                                 new IntRect(new Vector2i(10, 10), new Vector2i(80, 80)))),
                             new GUI.Components.Image(new Vector2f(10, 200), new Sprite(
                                 new Texture(@"resources/chessboard.jpg",
                                     new IntRect(new Vector2i(100, 100), new Vector2i(800, 800))),
                                 new IntRect(new Vector2i(10, 10), new Vector2i(80, 80)))),
                             new GUI.Components.Image(new Vector2f(200, 200), new Sprite(
                                 new Texture(@"resources/chessboard.jpg",
                                     new IntRect(new Vector2i(100, 100), new Vector2i(800, 800))),
                                 new IntRect(new Vector2i(10, 10), new Vector2i(80, 80)))),
                             new GUI.Components.Image(new Vector2f(400, 400), new Sprite(
                                 new Texture(@"resources/chessboard.jpg",
                                     new IntRect(new Vector2i(100, 100), new Vector2i(800, 800))),
                                 new IntRect(new Vector2i(10, 10), new Vector2i(80, 80))))
                         };
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
            foreach (Component component in components)
            {
                component.Draw(renderTarget, renderStates);
            }
        }

        /// <inheritdoc />
        protected override void SetupScreen(RenderTarget renderTarget, RenderStates renderStates)
        {
        }

        #endregion Overrides of Screen
    }
}