using System;
using System.Linq;

namespace StarSimLib.Data_Structures
{
    /// <summary>
    /// A tree of <see cref="Octant"/> instances, where each node has 8 children.
    /// </summary>
    public class OctantTree
    {
        /// <summary>
        /// The <see cref="Octant"/> of space managed by this instance.
        /// </summary>
        private readonly Octant octant;

        /// <summary>
        /// The aggregate mass of all the bodies held in this instance.
        /// </summary>
        private double aggregateMass;

        /// <summary>
        /// The <see cref="Body"/> instance that is held in this instance, be it real or an aggregate.
        /// </summary>
        private Body body;

        /// <summary>
        /// The point at which all the mass seems to be concentrated.
        /// </summary>
        private Vector4 centreOfAggregateMass;

        /// <summary>
        /// The child octant tree instances of this instance.
        /// </summary>
        private OctantTree[] childTrees;

        /// <summary>
        /// Initialises a new instance of the <see cref="OctantTree"/> class.
        /// </summary>
        /// <param name="octant">The <see cref="Octant"/> instance representing the space managed by this instance.</param>
        public OctantTree(Octant octant)
        {
            this.octant = octant;

            childTrees = new OctantTree[8];
        }

        /// <summary>
        /// Indexes this instance, shorthand for <see cref="SubTree(PositionSpecifier)"/>.
        /// </summary>
        /// <param name="specifier">Which child octant tree instance to return.</param>
        /// <returns>The specified child octant tree instance.</returns>
        public OctantTree this[PositionSpecifier specifier]
        {
            get { return SubTree(specifier); }
        }

        /// <summary>
        /// Indexes this instance, shorthand for <see cref="SubTree(PositionSpecifier)"/>.
        /// </summary>
        /// <param name="specifier">Which child octant tree instance to return.</param>
        /// <returns>The specified child octant tree instance.</returns>
        public OctantTree this[int specifier]
        {
            get { return SubTree(specifier); }
        }

        /// <summary>
        /// Adds the given body to one of the child trees of this instance.
        /// </summary>
        /// <param name="newBody">The body to add.</param>
        private void AddToChildTree(Body newBody)
        {
            // don't create subtrees if it violates the minimum width limit, to prevent infinitely deep trees and thus
            // preventing excessive resource allocation
            if (octant.Length / 2 < Constants.MinimumTreeWidth)
            {
                return;
            }

            for (int childTreeIndex = 0; childTreeIndex < 8; childTreeIndex++)
            {
                OctantTree childTree = SubTree(childTreeIndex);

                // if the child tree under consideration contains the given body, then add it to that tree
                if (childTree.octant.ContainsPoint(newBody.Position))
                {
                    childTree.AddBody(newBody);
                    return;
                }
            }
        }

        /// <summary>
        /// Returns the specified child octant tree instance, or instantiates a new octant tree if the specified child
        /// instance is <c>null</c>. The newly constructed instance will then be returned.
        /// </summary>
        /// <param name="trees">
        /// A reference to the <see cref="OctantTree"/> array from which to get the specified instance.
        /// </param>
        /// <param name="index">The index in the array whose held instance to get or set.</param>
        /// <param name="newOctant">
        /// The octant instance representing the space managed by the new child tree instance, if necessary.
        /// </param>
        /// <returns>The instance held at the specified index, or the newly created instance.</returns>
        public static OctantTree GetOrSetSubTree(ref OctantTree[] trees, int index, Octant newOctant)
        {
            if (trees != null && trees?[index] == null)
            {
                trees[index] = new OctantTree(newOctant);
            }

            return trees?[index];
        }

        /// <summary>
        /// Adds the given <see cref="Body"/> instance to this tree instance or a child tree instance.
        /// </summary>
        /// <param name="newBody">The <see cref="Body"/> instance to add.</param>
        public void AddBody(Body newBody)
        {
            centreOfAggregateMass = (aggregateMass * centreOfAggregateMass + newBody.Mass * newBody.Position) / (aggregateMass + newBody.Mass);
            aggregateMass += newBody.Mass;

            if (body == null)
            {
                // this is an empty instance that has not yet had any bodies added to it.
                body = newBody;
            }
            else if (IsExternal())
            {
                // this instance is 'external' and contains another body. figure out where the new body should go and
                // create a new octant tree instance to hold the new body
                AddToChildTree(newBody);
                AddToChildTree(body);
            }
            else if (!IsExternal())
            {
                // this instance already has a body to represent it, and it is not an 'external' tree instance, that is
                // it has child trees of its own. figure out in which child tree the new body should be stored and update
                // any further child nodes
                AddToChildTree(newBody);
            }
        }

        /// <summary>
        /// Whether this instance does not have any child instances and is thus an 'external' octant tree, or not.
        /// </summary>
        /// <returns>Whether this instance has no non-null child instance.</returns>
        public bool IsExternal() => childTrees.All(tree => tree == null) && body != null;

        /// <summary>
        /// Returns the specified child octant tree instance.
        /// </summary>
        /// <param name="specifier">Which child octant tree instance to return.</param>
        /// <returns>The specified child octant tree instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the given specifier does not equate to one of the values in the <see cref="PositionSpecifier"/> enum.
        /// </exception>
        public OctantTree SubTree(int specifier) => SubTree((PositionSpecifier)specifier);

        /// <summary>
        /// Returns the specified child octant tree instance.
        /// </summary>
        /// <param name="specifier">Which child octant tree instance to return.</param>
        /// <returns>The specified child octant tree instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the given specifier does not equate to one of the values in the <see cref="PositionSpecifier"/> enum.
        /// </exception>
        public OctantTree SubTree(PositionSpecifier specifier)
        {
            switch (specifier)
            {
                case PositionSpecifier.TopNorthWest:
                    return GetOrSetSubTree(ref childTrees, 0, octant?[0]);

                case PositionSpecifier.TopNorthEast:
                    return GetOrSetSubTree(ref childTrees, 1, octant?[1]);

                case PositionSpecifier.TopSouthEast:
                    return GetOrSetSubTree(ref childTrees, 2, octant?[2]);

                case PositionSpecifier.TopSouthWest:
                    return GetOrSetSubTree(ref childTrees, 3, octant?[3]);

                case PositionSpecifier.BottomNorthWest:
                    return GetOrSetSubTree(ref childTrees, 4, octant?[4]);

                case PositionSpecifier.BottomNorthEast:
                    return GetOrSetSubTree(ref childTrees, 5, octant?[5]);

                case PositionSpecifier.BottomSouthEast:
                    return GetOrSetSubTree(ref childTrees, 6, octant?[6]);

                case PositionSpecifier.BottomSouthWest:
                    return GetOrSetSubTree(ref childTrees, 7, octant?[7]);

                default:
                    throw new ArgumentOutOfRangeException(nameof(specifier), specifier,
                        "The given specifier was outside of the valid range.");
            }
        }

        /// <summary>
        /// Recursively updates the forces on each <see cref="Body"/> instance held in this tree, with respect to the
        /// given reference <see cref="Body"/> instance.
        /// </summary>
        /// <param name="referenceBody">The body instance against which force updates are made.</param>
        public void UpdateForces(Body referenceBody)
        {
            // cache the distance between the centre of mass of this tree instance and the reference body
            double dx = centreOfAggregateMass.X - referenceBody.Position.X;
            double dy = centreOfAggregateMass.Y - referenceBody.Position.Y;
            double dz = centreOfAggregateMass.Z - referenceBody.Position.Z;
            double distance2 = dx * dx + dy * dy + dz * dz;

            if (IsExternal() && referenceBody != body || octant.Length * octant.Length < Constants.TreeTheta * Constants.TreeTheta * distance2)
            {
                // since this tree instance is 'external' it has no children, we can treat it as a single body
                // we can also treat it as a single body if the width of the octant squared is less that the predefined
                // 'theta' (grouping tolerance) value times the distance between the centre of mass and the reference body
                referenceBody.AddForce(Body.GetForceBetween(referenceBody, centreOfAggregateMass, aggregateMass));
            }
            else if (childTrees != null)
            {
                // otherwise we need to fall back to a more 'granular' approach, and get the force between individual
                // child trees, as they are not far enough away to be considered as a single body
                foreach (OctantTree subtree in childTrees)
                {
                    subtree?.UpdateForces(referenceBody);
                }
            }
        }

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Tree - Octant: {octant?.ToString() ?? "null"}, " +
                   $"TNW: {childTrees[0]?.ToString() ?? "null"}, TNE: {childTrees[1]?.ToString() ?? "null"}, " +
                   $"TSE: {childTrees[2]?.ToString() ?? "null"}, TSW: {childTrees[3]?.ToString() ?? "null"}, " +
                   $"BNW: {childTrees[4]?.ToString() ?? "null"}, BNE: {childTrees[5]?.ToString() ?? "null"}, " +
                   $"BSE: {childTrees[6]?.ToString() ?? "null"}, BSW: {childTrees[7]?.ToString() ?? "null"}";
        }

        #endregion Overrides of Object
    }
}