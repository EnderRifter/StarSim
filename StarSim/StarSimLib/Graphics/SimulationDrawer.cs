﻿using SFML.Graphics;
using SFML.System;

using StarSimLib.Data_Structures;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StarSimLib.Graphics
{
    /// <summary>
    /// Enumerates all possible rotation directions (i.e. north, east, south, west, clockwise, anticlockwise).
    /// Here, camera forward is north.
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
    public class SimulationDrawer
    {
        /// <summary>
        /// Conversion from degrees to radians, as used by the <see cref="Math"/> functions.
        /// </summary>
        private const double DegToRad = Math.PI / 180;

        /// <summary>
        /// The furthest distance that can be seen on the <see cref="RenderTarget"/>. Any bodies that are further
        /// from the camera than this distance are culled and not rendered.
        /// </summary>
        private const double FarDistance = Constants.UniverseSize;

        /// <summary>
        /// The default field of view for the drawer in degrees.
        /// </summary>
        private const double FieldOfView = 45;

        /// <summary>
        /// The maximum speed that is mapped and rendered using the HSV colour map.
        /// </summary>
        private const double MaximumRenderedSpeed = 100_000;

        /// <summary>
        /// The minimum speed that is mapped and rendered using the HSV colour map.
        /// </summary>
        private const double MinimumRenderedSpeed = 0;

        /// <summary>
        /// The closest distance that can be seen on the <see cref="RenderTarget"/>. Any bodies that are closer
        /// to the camera than this distance (i.e. behind the camera) are culled and not rendered.
        /// </summary>
        private const double NearDistance = 0;

        /// <summary>
        /// The vector by which all projected points are translated away from the camera and halfway into the view frustum.
        /// </summary>
        private static readonly Vector4 cameraTranslationVector = new Vector4(0, 0, (FarDistance - NearDistance) / 2);

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

#pragma warning disable IDE0044  // Set field to readonly

        /// <summary>
        /// Projection matrix used for mapping from 3D to 2D.
        /// </summary>
        private Matrix4x4 projectionMatrix;

        /// <summary>
        /// The 3D rotation.
        /// </summary>
        private EulerAngle rotation;

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

#pragma warning restore IDE0044

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulationDrawer"/> class.
        /// </summary>
        /// <param name="target">The target to which the managed bodies should be rendered.</param>
        /// <param name="bodies">
        /// A reference to the <see cref="Body"/> instances which should be managed by this instance.
        /// </param>
        /// <param name="bodyShapeMap">
        /// A reference to the dictionary mapping a <see cref="Body"/> instance to the <see cref="CircleShape"/> that
        /// represents it, and is drawn to the screen at the <see cref="Body"/> instance's position.
        /// </param>
        public SimulationDrawer(RenderTarget target, ref Body[] bodies, ref Dictionary<Body, CircleShape> bodyShapeMap)
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
        /// Creates a new ARGB colour from a HSV colour.
        /// </summary>
        /// <param name="hue">The hue value for the new colour, between 0-360.</param>
        /// <param name="saturation">The saturation value for the new colour, between 0-1.</param>
        /// <param name="value">The value for the new colour, between 0-1.</param>
        /// <returns>The created ARGB colour.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Color ColorFromHsv(double hue, double saturation, double value)
        {
            double c = value * saturation;
            double k = hue / 60;
            double x = c * (1 - Math.Abs(k % 2 - 1));
            double m = value - c;

            double baseRed = 0, baseGreen = 0, baseBlue = 0;

            if (0 <= k && k <= 1)
            {
                baseRed = c;
                baseGreen = x;
            }
            else if (1 < k && k <= 2)
            {
                baseRed = x;
                baseGreen = c;
            }
            else if (2 < k && k <= 3)
            {
                baseGreen = c;
                baseBlue = x;
            }
            else if (3 < k && k <= 4)
            {
                baseGreen = x;
                baseBlue = c;
            }
            else if (4 < k && k <= 5)
            {
                baseRed = x;
                baseBlue = c;
            }
            else if (5 < k && k <= 6)
            {
                baseRed = c;
                baseBlue = x;
            }

            return new Color((byte)((baseRed + m) * 255), (byte)((baseGreen + m) * 255), (byte)((baseBlue + m) * 255), 255);
        }

        /// <summary>
        /// Linearly interpolates between a and b by the given percentage dt (0 = 100% a, 1 = 100% b).
        /// </summary>
        /// <param name="a">One of the starting values between which to interpolate.</param>
        /// <param name="b">One of the starting values between which to interpolate.</param>
        /// <param name="dt">The percentage by which to interpolate, capped between 0 and 1.</param>
        /// <returns>The interpolated value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector4 LinearInterpolate(Vector4 a, Vector4 b, double dt)
        {
            // caps the percentage first between 1 and -Infinity, and then between 1 and 0, and reverses it such
            // that 0 is 100% a and 0% b, and that 1 is 0% a and 100% b.
            dt = 1 - dt;
            dt = dt > 1 ? 1 : dt;
            dt = dt < 0 ? 0 : dt;

            /* formula: C = A + dt(B - A) / distance */
            Vector4 c = a + dt * (b - a) / 1;

            return c;
        }

        /// <summary>
        /// Maps a value in the given input range to a new value in the gieen output range.
        /// </summary>
        /// <param name="value">The value that should be mapped.</param>
        /// <param name="minimumInputRange">The lower bound of the input range.</param>
        /// <param name="maximumInputRange">The upper bound of the input range.</param>
        /// <param name="minimumOutputRange">The lower bound of the output range.</param>
        /// <param name="maximumOutputRange">The upper bound of the output range.</param>
        /// <returns>The projected value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Map(double value, double minimumInputRange, double maximumInputRange, double minimumOutputRange, double maximumOutputRange)
        {
            double clampedInput = value < maximumInputRange ? value :
                value < minimumInputRange ? minimumInputRange : maximumInputRange;

            double inputRange = maximumInputRange - minimumInputRange,
                outputRange = maximumOutputRange - minimumOutputRange;

            double percentIntoRange = clampedInput / inputRange;

            double output = outputRange * percentIntoRange;

            return output < maximumOutputRange ? output :
                output < minimumOutputRange ? minimumOutputRange : maximumOutputRange;
        }

        /// <summary>
        /// Projects the given <see cref="Vector4"/> point from world space into screen space.
        /// </summary>
        /// <param name="point">The point to project.</param>
        /// <returns>The projected point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector4 ProjectPoint(Vector4 point)
        {
            // rotations should happen before the point is translated into camera space
            Vector4 worldSpace = point;

            // rotations of point in the x, y and z axes
            worldSpace *= zRotationMatrix;
            worldSpace *= yRotationMatrix;
            worldSpace *= xRotationMatrix;

            // points must be translated into the camera space, as the camera must be some distance away from the
            // world space origin (0,0,0) or else rendering breaks. the point is translated into the middle of the
            // camera space (the view frustum)
            Vector4 cameraSpace = worldSpace + cameraTranslationVector;

            // project the point position from camera space into screen space (without any special transformations)
            Vector4 projectedPosition = cameraSpace * projectionMatrix;

            if (!projectedPosition.W.Equals(0))
            {
                projectedPosition.X /= projectedPosition.W;
                projectedPosition.Y /= projectedPosition.W;
                projectedPosition.Z /= projectedPosition.W;
            }

            // any transformations can be applied now
            Vector4 screenPosition = projectedPosition;

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
        /// in the constructor, using the current view settings (rotation, zoom, etc.) to project the positions from 3D to 2D.
        /// This method should be called every frame, as without it the view isn't updated and neither are rotation or zoom.
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
                    Vector4[] orbitTracerPositions = body.OrbitTracer.PreviousPositions.ToArray();

                    double clampedSpeed = body.Velocity.Abs() < MaximumRenderedSpeed ? body.Velocity.Abs() :
                        body.Velocity.Abs() < MinimumRenderedSpeed ? MinimumRenderedSpeed : MaximumRenderedSpeed;
                    double clampedRainbowColour = Map(clampedSpeed, MinimumRenderedSpeed, MaximumRenderedSpeed, 0, 360);

                    // we cache the colours used for the orbit tracers and the background, and pack them into a 4D vector
                    Color tracerColour = ColorFromHsv(clampedRainbowColour, 1, 1), bgColour = Color.Transparent;
                    Vector4 tracerVector = new Vector4(tracerColour.R, tracerColour.G, tracerColour.B, tracerColour.A);
                    Vector4 bgVector = new Vector4(bgColour.R, bgColour.G, bgColour.B, bgColour.A);

                    for (uint i = 0; i < orbitTracerPositions.Length; i++)
                    {
                        Vector4 previousPosition = orbitTracerPositions[i];

                        // project previous position onto the screen
                        Vector4 pointScreenPosition = ProjectPoint(previousPosition);

                        // convert the final position into a Vector2f, useable by the SFML.NET Vertex struct
                        Vector2f finalOrbitTracerPosition = new Vector2f(
                            (float)(pointScreenPosition.X * renderTarget.Size.X / 2 + originOffset.X),
                            (float)(pointScreenPosition.Y * renderTarget.Size.Y / 2 + originOffset.Y));

                        // use linear interpolation to make the tail colour transition look smooth
                        Vector4 interpolatedColourVector =
                            LinearInterpolate(tracerVector, bgVector, i / (double)orbitTracerPositions.Length);

                        // unpack the colour from a vector into a colour
                        Color interpolatedColour = new Color(
                            (byte)interpolatedColourVector.X,
                            (byte)interpolatedColourVector.Y,
                            (byte)interpolatedColourVector.Z,
                            (byte)interpolatedColourVector.W
                            );

                        // append the new vertex to the orbit tracer array
                        orbitTracerVertexArray.Append(new Vertex(finalOrbitTracerPosition, interpolatedColour));
                    }

                    // single call to VertexArray.Draw() makes use of hardware acceleration without causing delays,
                    // as modern graphics processing units are optimised to render many vertices simultaneously, instead
                    // of rendering many vertices sequentially
                    orbitTracerVertexArray.Draw(renderTarget, RenderStates.Default);
                }

                Vector4 screenPosition = ProjectPoint(body.Position);

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
                    rotation.X += 360 - angle % 360;
                    break;

                case RotationDirection.West:
                    rotation.Y += 360 - angle % 360;
                    break;

                case RotationDirection.Clockwise:
                    rotation.Z += 360 - angle % 360;
                    break;

                case RotationDirection.Anticlockwise:
                    rotation.Z += angle % 360;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction,
                        "The given rotation direction was not within the valid range.");
            }

            // once we hit 360 degrees of rotation, we wrap back around to 0 degrees
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