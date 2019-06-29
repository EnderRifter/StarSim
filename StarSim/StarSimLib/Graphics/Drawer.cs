using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using StarSimLib.Physics;

namespace StarSimLib.Graphics
{
    /// <summary>
    /// Represents a drawer capable of rendering bodies to a <see cref="RenderTarget"/>.
    /// </summary>
    public class Drawer
    {
        /// <summary>
        /// The aspect ratio of the render target. Allows for normalisation of the <see cref="RenderTarget"/> space
        /// into a normal plane ([-1, -1] = top left, [1, 1] = bottom right).
        /// </summary>
        private readonly float aspectRatio;

        /// <summary>
        /// The field of view for the drawer.
        /// </summary>
        private readonly float fieldOfView;

        /// <summary>
        /// Holds all the <see cref="Body"/> instances that should be simulated.
        /// </summary>
        private readonly Body[] managedBodies;

        /// <summary>
        /// Maps a <see cref="Body"/> to the <see cref="CircleShape"/> that represents it, and is drawn to the
        /// screen at the <see cref="Body"/>s position.
        /// </summary>
        private readonly Dictionary<Body, CircleShape> managedBodyShapeMap;

        /// <summary>
        /// The offset that has to be applied to the positions of a <see cref="CircleShape"/>, so that they appear
        /// to be in the centre of the render target and not the top left corner.
        /// </summary>
        private readonly Vector2u originOffset;

        /// <summary>
        /// The target to which the managed bodies will be drawn.
        /// </summary>
        private readonly RenderTarget renderTarget;

        /// <summary>
        /// Initialises a new instance of the <see cref="Drawer"/> class.
        /// </summary>
        /// <param name="target">The target to which the managed bodies should be rendered.</param>
        /// <param name="bodies">The <see cref="Body"/> instances which should be managed by this instance.</param>
        /// <param name="bodyShapeMap">
        /// Maps a <see cref="Body"/> instance to the <see cref="CircleShape"/> that represents it, and is drawn to the
        /// screen at the <see cref="Body"/> instances position.
        /// </param>
        public Drawer(RenderTarget target, ref Body[] bodies, ref Dictionary<Body, CircleShape> bodyShapeMap)
        {
            renderTarget = target;
            aspectRatio = renderTarget.Size.Y / (float)renderTarget.Size.X;
            originOffset = target.Size / 2;

            managedBodies = bodies;

            managedBodyShapeMap = bodyShapeMap;
        }

        private double InverseScaleFactor
        {
            get { return 1 / Math.Tan(fieldOfView / 2); }
        }

        /// <summary>
        /// Draws each <see cref="Body"/> instance that is managed by this drawer to the <see cref="RenderTarget"/> specified
        /// in the constructor.
        /// </summary>
        public void DrawBodies()
        {
            foreach (Body body in managedBodies)
            {
                // get the cached shape and projects the current bodies 3D position down to a 2D position,
                // which is used as the new position of the shape.
                CircleShape shape = managedBodyShapeMap[body];

                shape.Position = new Vector2f(
                    (float)(body.Position.X * Constants.UniverseScalingFactor + originOffset.X),
                    (float)(body.Position.Y * Constants.UniverseScalingFactor + originOffset.Y)
                );

                /*
                shape.Position = new Vector2f(
                    (float)(body.Position.X - shape.Radius / 2 + originOffset.X),
                    (float)(body.Position.Y - shape.Radius / 2 + originOffset.Y)
                );
                */

                shape.Draw(renderTarget, RenderStates.Default);
            }
        }

        /// <summary>
        /// Scales this instances view by the given amount.
        /// </summary>
        /// <param name="scaleMultiplier">The multiplier by which to scale the viewport of this instances render target.</param>
        public void Scale(float scaleMultiplier)
        {
            View renderView = renderTarget.GetView();

            renderView.Zoom(scaleMultiplier);

            renderTarget.SetView(renderView);
        }
    }
}