using System;

namespace StarSimLib.Data_Structures
{
    /// <summary>
    /// Represents a stellar body.
    /// </summary>
    public class Body
    {
        /// <summary>
        /// The orbit tracers for this instance.
        /// </summary>
        private readonly OrbitTracer orbitTracer;

        /// <summary>
        /// The backing field for the <see cref="Force"/> property.
        /// </summary>
        private Vector4 force;

        /// <summary>
        /// Backing field for the <see cref="Mass"/> property.
        /// </summary>
        private double mass;

        /// <summary>
        /// Backing field for the <see cref="Position"/> property.
        /// </summary>
        private Vector4 position;

        /// <summary>
        /// Backing field for the <see cref="Velocity"/> property.
        /// </summary>
        private Vector4 velocity;

        /// <summary>
        /// The generation that this body belongs to.
        /// </summary>
        public readonly uint Generation;

        /// <summary>
        /// The unique id for this body.
        /// </summary>
        public readonly uint Id;

        /// <summary>
        /// Initialises a new instance of the <see cref="Body"/> class.
        /// </summary>
        /// <param name="position">The starting position of the <see cref="Body"/>.</param>
        /// <param name="velocity">The starting velocity of the <see cref="Body"/>.</param>
        /// <param name="mass">The starting mass of the <see cref="Body"/>.</param>
        /// <param name="generation">The generation that this <see cref="Body"/> belongs to.</param>
        /// <param name="id">The unique id for this body.</param>
        public Body(Vector4 position, Vector4 velocity, double mass, uint generation = 1, uint id = 1)
        {
            Generation = generation;
            Id = id;

            this.position = position;
            this.position.W = 1;
            this.velocity = velocity;
            this.mass = mass;

            force = new Vector4();
            orbitTracer = new OrbitTracer();
        }

        /// <summary>
        /// The current force on the <see cref="Body"/> in 3D space.
        /// </summary>
        public Vector4 Force
        {
            get { return force; }
        }

        /// <summary>
        /// The mass of the <see cref="Body"/>.
        /// </summary>
        public double Mass
        {
            get { return mass; }
        }

        /// <summary>
        /// The <see cref="Data_Structures.OrbitTracer"/> for this instance.
        /// </summary>
        public ref readonly OrbitTracer OrbitTracer
        {
            get { return ref orbitTracer; }
        }

        /// <summary>
        /// The current position of the <see cref="Body"/> in 3D space.
        /// </summary>
        public Vector4 Position
        {
            get { return position; }
        }

        /// <summary>
        /// Whether to record previous positions of this instance, to render an orbit tracer behind it.
        /// </summary>
        public bool RecordPreviousPositions { get; set; }

        /// <summary>
        /// The current velocity of the <see cref="Body"/> in 3D space.
        /// </summary>
        public Vector4 Velocity
        {
            get { return velocity; }
        }

        /// <summary>
        /// Computes the force vector for the gravitational attraction between <see cref="Body"/> A and <see cref="Body"/> B.
        /// </summary>
        /// <param name="a">The first <see cref="Body"/> instance.</param>
        /// <param name="b">The second <see cref="Body"/> instance.</param>
        /// <returns>The computed force vector for the gravitational attraction.</returns>
        public static Vector4 GetForceBetween(Body a, Body b)
        {
            // Inlines the Body.DistanceTo(Body) as the position deltas need to be cached for later,
            // as well as to gain a small performance increase
            double dx = b.Position.X - a.Position.X,
                   dy = b.Position.Y - a.Position.Y,
                   dz = b.Position.Z - a.Position.Z;

            // The distance between two bodies can be found via taking the magnitude of their displacements,
            // as shown here via pythagoras
            double distance2 = dx * dx + dy * dy + dz * dz;
            double distance = Math.Sqrt(distance2);

            double numerator = Constants.G * a.Mass * b.Mass;
            double denominator = distance2 + Constants.SofteningFactor2;

            // Using the equation Force = Gravitational Constant * Mass(a) * Mass(b) / distance(a, b)^2
            // with a softening factor, we get the attraction force vector between the 2 bodies
            double force = numerator / denominator;

            return new Vector4(force * dx / distance, force * dy / distance, force * dz / distance);
        }

        /// <summary>
        /// Computes the force vector for the gravitational attraction between the <see cref="Body"/> A and the mass at
        /// the given position.
        /// </summary>
        /// <param name="a">The <see cref="Body"/> instance.</param>
        /// <param name="positionB">The position at which the given mass can be thought of as acting.</param>
        /// <param name="massB">The mass for which to compute the gravitational attraction.</param>
        /// <returns>The computed force vector for the gravitational attraction.</returns>
        public static Vector4 GetForceBetween(Body a, Vector4 positionB, double massB)
        {
            // Inlines the Body.DistanceTo(Body) as the position deltas need to be cached for later,
            // as well as to gain a small performance increase
            double dx = positionB.X - a.Position.X,
                   dy = positionB.Y - a.Position.Y,
                   dz = positionB.Z - a.Position.Z;

            // The distance between two bodies can be found via taking the magnitude of their displacements,
            // as shown here via pythagoras
            double distance2 = dx * dx + dy * dy + dz * dz;
            double distance = Math.Sqrt(distance2);

            double numerator = Constants.G * a.Mass * massB;
            double denominator = distance2 + Constants.SofteningFactor2;

            // Using the equation Force = Gravitational Constant * Mass(a) * Mass(b) / distance(a, b)^2
            // with a softening factor, we get the attraction force vector between the 2 bodies
            double force = numerator / denominator;

            return new Vector4(force * dx / distance, force * dy / distance, force * dz / distance);
        }

        /// <summary>
        /// Calculates the force between this instance and the given other body, and adds the resultant vector to the
        /// internal total force vector.
        /// </summary>
        /// <param name="otherBody">The other body to calculate the force between.</param>
        public void AddForce(Body otherBody)
        {
            force += GetForceBetween(this, otherBody);
        }

        /// <summary>
        /// Updates the current force vector for this instance, by adding the given vector to it.
        /// </summary>
        /// <param name="forceVector">The force vector to add to this instances internal force vector.</param>
        public void AddForce(Vector4 forceVector)
        {
            force += forceVector;
        }

        /// <summary>
        /// Collides this instance with the given <see cref="Body"/> instance.
        /// </summary>
        /// <param name="otherBody">The other instance with which to collide.</param>
        public void Collide(Body otherBody)
        {
            mass += otherBody.mass;
            velocity += otherBody.velocity;
        }

        /// <summary>
        /// Finds and returns the distance between the current <see cref="Body"/> instance and the given <see cref="Body"/>.
        /// In other words, it finds the magnitude of the translation vector between the two <see cref="Body"/> instances.
        /// </summary>
        /// <param name="body">The other <see cref="Body"/> instance to which to calculate the distance.</param>
        /// <param name="displacement">The vector showing the displacement of the current body from the given one.</param>
        /// <returns>The distance to the other <see cref="Body"/> as a <see cref="float"/>.</returns>
        public double DistanceTo(Body body, out Vector4 displacement)
        {
            double dpx = body.Position.X - Position.X,
                   dpy = body.Position.Y - Position.Y,
                   dpz = body.Position.Z - Position.Z;

            displacement = new Vector4(dpx, dpy, dpz);

            return Math.Sqrt(dpx * dpx + dpy * dpy + dpz * dpz);
        }

        /// <summary>
        /// Whether this instance is currently inside the bounds of the given <see cref="Octant"/> instance.
        /// </summary>
        /// <param name="octant">The octant to check.</param>
        /// <returns>Whether this <see cref="Body"/> instance is inside the given <see cref="Octant"/> instance.</returns>
        public bool IsInOctant(Octant octant)
        {
            return octant.ContainsPoint(position);
        }

        /// <summary>
        /// Resets the internal force vector.
        /// </summary>
        public void ResetForce()
        {
            force.X = 0;
            force.Y = 0;
            force.Z = 0;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Body {Generation,2:D}.{Id,-4:D}: " +
                   $"Pos-[{Position.X:E2}, {Position.Y:E2}, {Position.Z:E2}], " +
                   $"Vel-[{Velocity.X:E2}, {Velocity.Y:E2}, {Velocity.Z:E2}], " +
                   $"Mass-{Mass:E2}";
        }

        /// <summary>
        /// Updates the <see cref="Position"/> of the current body using the internal force vector and the given time step.
        /// </summary>
        /// <param name="deltaTime">The time step.</param>
        public void Update(double deltaTime)
        {
            velocity += deltaTime * force / mass;

            if (RecordPreviousPositions)
            {
                orbitTracer.Enqueue(position);
            }

            position += deltaTime * velocity;
        }

        /// <summary>
        /// Updates the <see cref="Position"/> of the current body using the given force vector and time step.
        /// </summary>
        /// <param name="forceVector">The sum vector of all forces due to other <see cref="Body"/>s.</param>
        /// <param name="deltaTime">The time step.</param>
        public void Update(Vector4 forceVector, double deltaTime)
        {
            velocity += deltaTime * forceVector / mass;

            if (RecordPreviousPositions)
            {
                orbitTracer.Enqueue(position);
            }

            position += deltaTime * velocity;
        }
    }
}