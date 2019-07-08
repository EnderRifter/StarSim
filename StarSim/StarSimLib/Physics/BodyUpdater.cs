using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarSimLib.Data_Structures;

namespace StarSimLib.Physics
{
    /// <summary>
    /// Updates the given <see cref="Body"/> collections individual bodies, using the given delta time.
    /// </summary>
    /// <param name="bodies">The body collection.</param>
    /// <param name="deltaTime">The time step.</param>
    public delegate void UpdateDelegate(IEnumerable<Body> bodies, double deltaTime);

    /// <summary>
    /// Provides methods for updating <see cref="Body"/> instance positions.
    /// </summary>
    public static class BodyUpdater
    {
        /// <summary>
        /// Updates the positions of all the given <see cref="Body"/>s with O(n^2) time complexity, with the given time step.
        /// </summary>
        /// <param name="bodies">The collection of <see cref="Body"/>s whose positions to update.</param>
        /// <param name="deltaTime">The time step.</param>
        public static void UpdateBodiesBarnesHut(IEnumerable<Body> bodies, double deltaTime)
        {
            IEnumerable<Body> bodyEnumerable = bodies as Body[] ?? bodies.ToArray();

            // root octant represents the main universe. Anything outside this octant of space is not updated
            Octant universeOctant = new Octant(Constants.UniverseOctant);

            OctantTree barnesHutTree = new OctantTree(universeOctant);

            // we construct our barnes-hut octant tree
            foreach (Body body in bodyEnumerable)
            {
                if (body.IsInOctant(universeOctant))
                {
                    // only update a body's position if it is within the bounds of the universe
                    barnesHutTree.AddBody(body);
                }
            }

            // we update the positions of the bodies in the populated tree in a parallel manner, using the thread pool
            Parallel.ForEach(bodyEnumerable, body =>
            {
                if (body != null)
                {
                    body.ResetForce();

                    if (body.IsInOctant(universeOctant))
                    {
                        barnesHutTree.UpdateForces(body);

                        body.Update(deltaTime);
                    }
                }
            });
        }

        /// <summary>
        /// Updates the positions of all the given <see cref="Body"/>s with O(n^2) time complexity, with the given time step.
        /// </summary>
        /// <param name="bodies">The collection of <see cref="Body"/>s whose positions to update.</param>
        /// <param name="deltaTime">The time step.</param>
        public static void UpdateBodiesBruteForce(IEnumerable<Body> bodies, double deltaTime)
        {
            IEnumerable<Body> bodyEnumerable = bodies as Body[] ?? bodies.ToArray();
            Vector4 forceVector = new Vector4();

            foreach (Body body in bodyEnumerable)
            {
                // resets the force vector to avoid another instantiation and allocation
                forceVector.X = 0;
                forceVector.Y = 0;
                forceVector.Z = 0;

                // use LINQ expression as it is more concise; sum attraction vectors for all other bodies
                forceVector = bodyEnumerable.Where(b => b != body).Aggregate(forceVector, (current, b) => current + Body.GetForceBetween(body, b));

                body.Update(forceVector, deltaTime);
            }
        }
    }
}