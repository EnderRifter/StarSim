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
        /// The furthest distance that can be seen on the <see cref="RenderTarget"/>.
        /// </summary>
        private const double farDistance = 1000 * Constants.UniverseSize;

        /// <summary>
        /// The field of view for the drawer.
        /// </summary>
        private const double fieldOfView = 90f;

        /// <summary>
        /// The closest distance that can be seen on the <see cref="RenderTarget"/>.
        /// </summary>
        private const double nearDistance = 0.1;

        /// <summary>
        /// Inverse scale factor used in the projection matrix.
        /// </summary>
        private static readonly double inverseScaleFactor = 1 / Math.Tan(fieldOfView * 0.5 / 180 * Math.PI);

        /// <summary>
        /// The aspect ratio of the render target. Allows for normalisation of the <see cref="RenderTarget"/> space
        /// into a normal plane ([-1, -1] = top left, [1, 1] = bottom right).
        /// </summary>
        private readonly double aspectRatio;

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
        /// Projection matrix used for mapping from 3D to 2D.
        /// </summary>
        private readonly Matrix4x4 projectionMatrix;

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
            originOffset = target.Size / 2;

            aspectRatio = renderTarget.Size.Y / (double)renderTarget.Size.X;

            projectionMatrix = new Matrix4x4(new[]
                                             {
                                                 new [] { aspectRatio * inverseScaleFactor, 0, 0, 0 },
                                                 new [] { 0, inverseScaleFactor, 0, 0 },
                                                 new [] { 0, 0, farDistance / (farDistance - nearDistance), 1 },
                                                 new [] { 0, 0, -farDistance * nearDistance / (farDistance - nearDistance), 0 }
                                             });

            managedBodies = bodies;
            managedBodyShapeMap = bodyShapeMap;
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

                Vector4d worldSpace = body.Position;

                // project the body position
                Vector4d projectedPosition = (worldSpace * projectionMatrix);

                if (!projectedPosition.W.Equals(0))
                {
                    projectedPosition.X /= projectedPosition.W;
                    projectedPosition.Y /= projectedPosition.W;
                    projectedPosition.Z /= projectedPosition.W;
                }

                Vector4d screenPosition = projectedPosition;

                // final position
                shape.Position = new Vector2f(
                    (float)(screenPosition.X * 100 + originOffset.X),
                    (float)(screenPosition.Y * 100 + originOffset.Y)
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