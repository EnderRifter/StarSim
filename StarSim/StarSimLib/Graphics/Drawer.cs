using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using StarSimLib.Data_Structures;
using StarSimLib.Physics;

namespace StarSimLib.Graphics
{
    /// <summary>
    /// Enumerates all possible rotation directions (i.e. north, east, south, west, clockwise, anticlockwise).
    /// </summary>
    public enum RotationDirection
    {
        /// <summary>
        /// Represents an anticlockwise rotation in the x-axis.
        /// </summary>
        North,

        /// <summary>
        /// Represents an anticlockwise rotation in the y-axis.
        /// </summary>
        East,

        /// <summary>
        /// Represents a clockwise rotation in the x-axis.
        /// </summary>
        South,

        /// <summary>
        /// Represents a clockwise rotation in the y-axis.
        /// </summary>
        West,

        /// <summary>
        /// Represents a clockwise rotation in the z-axis.
        /// </summary>
        Clockwise,

        /// <summary>
        /// Represents an anticlockwise rotation in the z-axis.
        /// </summary>
        Anticlockwise
    }

    /// <summary>
    /// Represents a drawer capable of rendering bodies to a <see cref="RenderTarget"/>.
    /// </summary>
    public class Drawer
    {
        /// <summary>
        /// Conversion from degrees to radians, as used by the <see cref="Math"/> functions.
        /// </summary>
        private const double DegToRad = Math.PI / 180;

        /// <summary>
        /// The furthest distance that can be seen on the <see cref="RenderTarget"/>. Any bodies that are further
        /// from the camera than this distance are culled and not rendered.
        /// </summary>
        private const double FarDistance = 1 * Constants.UniverseSize;

        /// <summary>
        /// The default field of view for the drawer in degrees.
        /// </summary>
        private const double FieldOfView = 45;

        /// <summary>
        /// The closest distance that can be seen on the <see cref="RenderTarget"/>. Any bodies that are closer
        /// to the camera than this distance (i.e. behind the camera) are culled and not rendered.
        /// </summary>
        private const double NearDistance = 0;

        /// <summary>
        /// The vector by which all projected points are translated away from the camera and halfway into the view frustum.
        /// </summary>
        private static readonly Vector4d cameraTranslationVector = new Vector4d(0, 0, (FarDistance - NearDistance) / 2);

        /// <summary>
        /// The aspect ratio of the render target. Allows for normalisation of the <see cref="RenderTarget"/> space
        /// into a normal plane ([-1, -1] = top left, [1, 1] = bottom right), instead of having to deal with a
        /// variable render space.
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
        /// Maps a <see cref="Body"/> to the <see cref="VertexArray"/> that holds its orbit tracer vertices, and
        /// is drawn to the screen behind the <see cref="Body"/> instance.
        /// </summary>
        private readonly Dictionary<Body, VertexArray> managedBodyTracerVertexArrayMap;

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
        /// The current value for the field of view, in degrees. Used to zoom the view in and out.
        /// </summary>
        private double currentFieldOfView = FieldOfView;

        /// <summary>
        /// Projection matrix used for mapping from 3D to 2D.
        /// </summary>
        private Matrix4x4 projectionMatrix;

        /// <summary>
        /// The 3D rotation.
        /// </summary>
        private EulerAngles rotation;

        /// <summary>
        /// The rotation matrix for the x axis.
        /// </summary>
        private Matrix4x4 xRotationMatrix;

        /// <summary>
        /// The rotation matrix for the y axis.
        /// </summary>
        private Matrix4x4 yRotationMatrix;

        /// <summary>
        /// The rotation matrix for the z axis.
        /// </summary>
        private Matrix4x4 zRotationMatrix;

        /// <summary>
        /// Initialises a new instance of the <see cref="Drawer"/> class.
        /// </summary>
        /// <param name="target">The target to which the managed bodies should be rendered.</param>
        /// <param name="bodies">
        /// A reference to the <see cref="Body"/> instances which should be managed by this instance.
        /// </param>
        /// <param name="bodyShapeMap">
        /// A reference to the dictionary mapping a <see cref="Body"/> instance to the <see cref="CircleShape"/> that
        /// represents it, and is drawn to the screen at the <see cref="Body"/> instance's position.
        /// </param>
        public Drawer(RenderTarget target, ref Body[] bodies, ref Dictionary<Body, CircleShape> bodyShapeMap)
        {
            renderTarget = target;
            originOffset = target.Size / 2;

            aspectRatio = renderTarget.Size.Y / (double)renderTarget.Size.X;

            // projection and rotation matrices
            projectionMatrix = new Matrix4x4(new[]
                                             {
                                                 new [] { aspectRatio * InverseScaleFactor, 0, 0, 0 },
                                                 new [] { 0, InverseScaleFactor, 0, 0 },
                                                 new [] { 0, 0, FarDistance / (FarDistance - NearDistance), 1 },
                                                 new [] { 0, 0, -FarDistance * NearDistance / (FarDistance - NearDistance), 0 }
                                             });

            xRotationMatrix = new Matrix4x4(new[]
                                            {
                                                new [] { 1d, 0, 0, 0 },
                                                new [] { 0d, 0, 0, 0 },
                                                new [] { 0d, 0, 0, 0 },
                                                new [] { 0d, 0, 0, 1 }
                                            });

            yRotationMatrix = new Matrix4x4(new[]
                                            {
                                                new [] { 0d, 0, 0, 0 },
                                                new [] { 0d, 1, 0, 0 },
                                                new [] { 0d, 0, 0, 0 },
                                                new [] { 0d, 0, 0, 1 }
                                            });

            zRotationMatrix = new Matrix4x4(new[]
                                            {
                                                new [] { 0d, 0, 0, 0 },
                                                new [] { 0d, 0, 0, 0 },
                                                new [] { 0d, 0, 1, 0 },
                                                new [] { 0d, 0, 0, 1 }
                                            });

            // set initial values for the rotation matrices
            UpdateRotationMatrices();

            managedBodies = bodies;
            managedBodyShapeMap = bodyShapeMap;
            managedBodyTracerVertexArrayMap = new Dictionary<Body, VertexArray>();

            foreach (Body body in bodies)
            {
                // create a vertex array to store orbit tracer points for this body
                managedBodyTracerVertexArrayMap.Add(body,
                    new VertexArray(PrimitiveType.LineStrip, Constants.StoredPreviousPositionCount));
            }
        }

        /// <summary>
        /// Inverse scale factor used in the projection matrix.
        /// </summary>
        private double InverseScaleFactor { get { return 1 / Math.Tan(currentFieldOfView * 0.5 * DegToRad); } }

        /// <summary>
        /// Current field-of-view.
        /// </summary>
        public double FOV
        {
            get
            {
                return currentFieldOfView;
            }
        }

        /// <summary>
        /// Current angle of rotation in the x axis.
        /// </summary>
        public double XAngle
        {
            get { return rotation.X; }
        }

        /// <summary>
        /// Current angle of rotation in the x axis.
        /// </summary>
        public double YAngle
        {
            get { return rotation.Y; }
        }

        /// <summary>
        /// Current angle of rotation in the x axis.
        /// </summary>
        public double ZAngle
        {
            get { return rotation.Z; }
        }

        /// <summary>
        /// Current zoom level of the drawer.
        /// </summary>
        public double ZoomLevel
        {
            get { return FieldOfView / currentFieldOfView; }
        }

        /// <summary>
        /// Projects the given <see cref="Vector4d"/> point from world space into screen space.
        /// </summary>
        /// <param name="point">The point to project.</param>
        /// <returns>The projected point.</returns>
        private Vector4d ProjectPoint(Vector4d point)
        {
            // rotations should happen before the point is translated into camera space
            Vector4d worldSpace = point;

            // rotations of point in the x, y and z axes
            worldSpace *= zRotationMatrix;
            worldSpace *= yRotationMatrix;
            worldSpace *= xRotationMatrix;

            // points must be translated into the camera space, as the camera must be some distance away from the
            // world space origin (0,0,0) or else rendering breaks. the point is translated into the middle of the
            // camera space (the view frustum)
            Vector4d cameraSpace = worldSpace + cameraTranslationVector;

            // project the point position from camera space into screen space (without any special transformations)
            Vector4d projectedPosition = cameraSpace * projectionMatrix;

            if (!projectedPosition.W.Equals(0))
            {
                projectedPosition.X /= projectedPosition.W;
                projectedPosition.Y /= projectedPosition.W;
                projectedPosition.Z /= projectedPosition.W;
            }

            // any transformations can be applied now
            Vector4d screenPosition = projectedPosition;

            return screenPosition;
        }

        /// <summary>
        /// Updates the rotation matrices for the view.
        /// </summary>
        private void UpdateRotationMatrices()
        {
            (double xAngle, double yAngle, double zAngle) = (rotation.X, rotation.Y, rotation.Z);

            // update the rotation matrix values for the x axis
            xRotationMatrix[1, 1] = Math.Cos(xAngle * DegToRad);
            xRotationMatrix[1, 2] = -Math.Sin(xAngle * DegToRad);
            xRotationMatrix[2, 1] = Math.Sin(xAngle * DegToRad);
            xRotationMatrix[2, 2] = Math.Cos(xAngle * DegToRad);

            // update the rotation matrix values for the y axis
            yRotationMatrix[0, 0] = Math.Cos(yAngle * DegToRad);
            yRotationMatrix[0, 2] = Math.Sin(yAngle * DegToRad);
            yRotationMatrix[2, 0] = -Math.Sin(yAngle * DegToRad);
            yRotationMatrix[2, 2] = Math.Cos(yAngle * DegToRad);

            // update the rotation matrix values for the z axis
            zRotationMatrix[0, 0] = Math.Cos(zAngle * DegToRad);
            zRotationMatrix[0, 1] = -Math.Sin(zAngle * DegToRad);
            zRotationMatrix[1, 0] = Math.Sin(zAngle * DegToRad);
            zRotationMatrix[1, 1] = Math.Cos(zAngle * DegToRad);
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
                // which is used as the new position of the shape
                CircleShape shape = managedBodyShapeMap[body];

                // get the vertex array used to store orbit tracer points for this body
                VertexArray orbitTracerVertexArray = managedBodyTracerVertexArrayMap[body];

                // we need to clear the vertex array, to ensure that no garbage values are present. otherwise
                // there are lines from the origin to the first actually desired position
                orbitTracerVertexArray.Clear();

                // we only attempt to draw tracers if the body instance in question should record its previous position
                // otherwise, we skip the relatively expensive rendering code
                if (body.RecordPreviousPositions)
                {
                    Vector4d[] orbitTracerPositions = body.PreviousPositions.ToArray();

                    for (uint i = 0; i < orbitTracerPositions.Length; i++)
                    {
                        Vector4d previousPosition = orbitTracerPositions[i];

                        // project previous position onto the screen
                        Vector4d pointScreenPosition = ProjectPoint(previousPosition);

                        // convert the final position into a Vector2f, useable by the SFML.NET Vertex struct
                        Vector2f finalOrbitTracerPosition = new Vector2f(
                            (float)(pointScreenPosition.X * renderTarget.Size.X / 2 + originOffset.X),
                            (float)(pointScreenPosition.Y * renderTarget.Size.Y / 2 + originOffset.Y));

                        // append the new vertex to the orbit tracer array
                        orbitTracerVertexArray.Append(new Vertex(finalOrbitTracerPosition, Color.Cyan));
                    }

                    // single call to VertexArray.Draw() makes use of hardware acceleration without causing delays,
                    // as modern graphics processing units are optimised to render many vertices simultaneously, instead
                    // of rendering many vertices sequentially
                    orbitTracerVertexArray.Draw(renderTarget, RenderStates.Default);
                }

                Vector4d screenPosition = ProjectPoint(body.Position);

                // final position
                shape.Position = new Vector2f(
                    (float)(screenPosition.X * renderTarget.Size.X / 2 + originOffset.X),
                    (float)(screenPosition.Y * renderTarget.Size.Y / 2 + originOffset.Y)
                );

                // the shape is drawn onto the render target at its final screen position
                shape.Draw(renderTarget, RenderStates.Default);
            }
        }

        /// <summary>
        /// Rotates the view in the given direction, by the specified angle (in degrees).
        /// </summary>
        /// <param name="direction">The direction in which to rotate the view.</param>
        /// <param name="angle">The angle by which to rotate in the given direction.</param>
        public void Rotate(RotationDirection direction, double angle)
        {
            switch (direction)
            {
                case RotationDirection.North:
                    rotation.X += angle % 360;
                    break;

                case RotationDirection.East:
                    rotation.Y += angle % 360;
                    break;

                case RotationDirection.South:
                    rotation.X -= angle % 360;
                    break;

                case RotationDirection.West:
                    rotation.Y -= angle % 360;
                    break;

                case RotationDirection.Clockwise:
                    rotation.Z -= angle % 360;
                    break;

                case RotationDirection.Anticlockwise:
                    rotation.Z += angle % 360;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction,
                        "The given rotation direction was not within the valid range.");
            }

            // once we hit 360 (or -360) degrees of rotation, we wrap back around to 0 degrees
            rotation.X %= 360;
            rotation.Y %= 360;
            rotation.Z %= 360;

            UpdateRotationMatrices();
        }

        /// <summary>
        /// Scales this instances view by the given amount.
        /// </summary>
        /// <param name="scaleMultiplier">The multiplier by which to scale the viewport of this instances render target.</param>
        public void Scale(double scaleMultiplier)
        {
            currentFieldOfView /= scaleMultiplier;

            // limits field of view to a lower bound and an upper bound
            currentFieldOfView = currentFieldOfView < 0.0001 ? 0.0001 : currentFieldOfView;
            currentFieldOfView = currentFieldOfView > 179d ? 179 : currentFieldOfView;

            // reassign projection matrix fields as the field of view (used by InverseScaleFactor) has been
            // modified to zoom in or out
            projectionMatrix[0, 0] = aspectRatio * InverseScaleFactor;
            projectionMatrix[1, 1] = InverseScaleFactor;
        }
    }
}