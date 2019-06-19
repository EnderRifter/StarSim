using SFML.System;
using System;
using StarSimLib.Extensions;

namespace StarSimLib
{
    /// <summary>
    /// Represents a stellar body.
    /// </summary>
    public class Body
    {
        #region Variables

        /// <summary>
        /// Describes how the <see cref="Body"/> instance should be formatted as a <see cref="string"/>.
        /// </summary>
        private const string BodyFormatString =
            "Body {0,2}.{1,-4}: Pos-({2:D,-3}, {3:D,-3}, {4:D,-3}), Vel-({5:D,-3}, {6:D,-3}, {7:D,-3}), Mass-{8:D,3}";

        /// <summary>
        /// The generation that this body belongs to.
        /// </summary>
        public readonly uint Generation;

        /// <summary>
        /// The unique id for this body.
        /// </summary>
        public readonly uint Id;

        #endregion Variables

        #region Properties

        /// <summary>
        /// The current position of the <see cref="Body"/> in 3D space.
        /// </summary>
        public Vector3d Position { get; private set; }

        /// <summary>
        /// The current velocity of the <see cref="Body"/> in 3D space.
        /// </summary>
        public Vector3d Velocity { get; private set; }

        /// <summary>
        /// The mass of the <see cref="Body"/>.
        /// </summary>
        public double Mass { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Body"/> class.
        /// </summary>
        /// <param name="position">The starting position of the <see cref="Body"/>.</param>
        /// <param name="velocity">The starting velocity of the <see cref="Body"/>.</param>
        /// <param name="mass">The starting mass of the <see cref="Body"/>.</param>
        /// <param name="generation">The generation that this <see cref="Body"/> belongs to.</param>
        /// <param name="id">The unique id for this body.</param>
        public Body(Vector3d position, Vector3d velocity, double mass, uint generation = 1, uint id = 1)
        {
            Generation = generation;
            Id = id;

            Position = position;
            Velocity = velocity;
            Mass = mass;
        }

        #endregion Constructors

        #region Methods

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(BodyFormatString, Generation, Id, Position.X, Position.Y, Position.Z, Velocity.X, Velocity.Y, Velocity.Z, Mass);
        }

        #endregion Overrides of Object

        /// <summary>
        /// Updates the <see cref="Position"/> of the current body using the given force vector and time step.
        /// </summary>
        /// <param name="forceVector">The sum vector of all forces due to other <see cref="Body"/>s.</param>
        /// <param name="deltaTime">The time step.</param>
        public void Update(Vector3d forceVector, double deltaTime)
        {
            Velocity += deltaTime * forceVector / Mass;
            Position += deltaTime * Velocity;
        }

        /// <summary>
        /// Finds and returns the distance between the current <see cref="Body"/> instance and the given <see cref="Body"/>.
        /// In other words, it finds the magnitude of the translation vector between the two <see cref="Body"/> instances.
        /// </summary>
        /// <param name="body">The other <see cref="Body"/> instance to which to calculate the distance.</param>
        /// <param name="displacement">The vector showing the displacement of the current body from the given one.</param>
        /// <returns>The distance to the other <see cref="Body"/> as a <see cref="float"/>.</returns>
        public double DistanceTo(Body body, out Vector3d displacement)
        {
            double dpx = body.Position.X - Position.X,
                   dpy = body.Position.Y - Position.Y,
                   dpz = body.Position.Z - Position.Z;

            displacement = new Vector3d(dpx, dpy, dpz);

            return Math.Sqrt(dpx * dpx + dpy * dpy + dpz * dpz);
        }

        /// <summary>
        /// Gets the force vector for the attraction between <see cref="Body"/> A and <see cref="Body"/> B.
        /// </summary>
        /// <param name="a">The first <see cref="Body"/> instance.</param>
        /// <param name="b">The second <see cref="Body"/> instance.</param>
        /// <returns></returns>
        public static Vector3d GetForceBetween(Body a, Body b)
        {
            // Inlines the Body.DistanceTo(Body) as the position deltas need to be cached for later,
            // as well as to gain a small performance increase
            double dx = b.Position.X - a.Position.X,
                   dy = b.Position.Y - a.Position.Y,
                   dz = b.Position.Z - a.Position.Z;

            // The distance between two bodies can be found via taking the magnitude of their displacements,
            // as shown here via pythagoras
            double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);

            double numerator = Constants.G * a.Mass * b.Mass;
            double denominator = distance * distance + Constants.SofteningFactor2;

            // Using the equation Force = Gravitational Constant * Mass(a) * Mass(b) / distance(a, b)^2
            // with a softening factor, we get the attraction force vector between the 2 bodies
            double force = numerator / denominator;

            return new Vector3d(force * dx / distance, force * dy / distance, force * dz / distance);
        }

        #endregion Methods
    }
}