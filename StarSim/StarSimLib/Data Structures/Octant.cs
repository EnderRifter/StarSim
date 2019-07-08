using System;

namespace StarSimLib.Data_Structures
{
    /// <summary>
    /// Enumerates the possible sub-octants positions.
    /// </summary>
    public enum PositionSpecifier
    {
        /// <summary>
        /// The child octant in the top layer, north west corner.
        /// </summary>
        TopNorthWest,

        /// <summary>
        /// The child octant in the top layer, north east corner.
        /// </summary>
        TopNorthEast,

        /// <summary>
        /// The child octant in the top layer, south east corner.
        /// </summary>
        TopSouthEast,

        /// <summary>
        /// The child octant in the top layer, south west corner.
        /// </summary>
        TopSouthWest,

        /// <summary>
        /// The child octant in the bottom layer, north west corner.
        /// </summary>
        BottomNorthWest,

        /// <summary>
        /// The child octant in the bottom layer, north east corner.
        /// </summary>
        BottomNorthEast,

        /// <summary>
        /// The child octant in the bottom layer, south east corner.
        /// </summary>
        BottomSouthEast,

        /// <summary>
        /// The child octant in the bottom layer, south west corner.
        /// </summary>
        BottomSouthWest,
    }

    /// <summary>
    /// Represents an octant of 3D space.
    /// </summary>
    public class Octant
    {
        /// <summary>
        /// The length of a half of 1 side of this instance, used to provide the side length of child instances.
        /// </summary>
        private readonly double halfSideLength;

        /// <summary>
        /// The centre of this instance.
        /// </summary>
        private readonly Vector4 midpoint;

        /// <summary>
        /// The length of a quarter of 1 side of this instance, used to provide the midpoint of child instances.
        /// </summary>
        private readonly double quarterSideLength;

        /// <summary>
        /// The length of 1 side of this instance.
        /// </summary>
        private readonly double sideLength;

        /// <summary>
        /// The child octant instances of this instance.
        /// </summary>
        private Octant[] childOctants;

        /// <summary>
        /// Initialises a new instance of the <see cref="Octant"/> class.
        /// </summary>
        /// <param name="midpoint">The central midpoint of this instance.</param>
        /// <param name="length">The length of 1 complete side of this instance.</param>
        public Octant(Vector4 midpoint, double length)
        {
            this.midpoint = midpoint;
            sideLength = length;
            halfSideLength = length / 2;
            quarterSideLength = length / 4;

            childOctants = new Octant[8];
        }

        /// <summary>
        /// The length of 1 complete side of this instance.
        /// </summary>
        public double Length
        {
            get { return sideLength; }
        }

        /// <summary>
        /// The central point for this instance.
        /// </summary>
        public Vector4 Midpoint
        {
            get { return midpoint; }
        }

        /// <summary>
        /// Indexes this instance, shorthand for <see cref="SubOctant(PositionSpecifier)"/>.
        /// </summary>
        /// <param name="specifier">Which child octant instance to return.</param>
        /// <returns>The specified child octant instance.</returns>
        public Octant this[PositionSpecifier specifier]
        {
            get { return SubOctant(specifier); }
        }

        /// <summary>
        /// Indexes this instance, shorthand for <see cref="SubOctant(PositionSpecifier)"/>.
        /// </summary>
        /// <param name="specifier">Which child octant instance to return.</param>
        /// <returns>The specified child octant instance.</returns>
        public Octant this[int specifier]
        {
            get { return SubOctant(specifier); }
        }

        /// <summary>
        /// Returns the specified child octant instance, or instantiates a new octant if the specified child instance
        /// is <c>null</c>. The newly constructed instance will then be returned.
        /// </summary>
        /// <param name="octants">
        /// A reference to the <see cref="Octant"/> array from which to get the specified instance.
        /// </param>
        /// <param name="index">The index in the array whose held instance to get or set.</param>
        /// <param name="newMidpoint">The midpoint of the new child instance, if necessary.</param>
        /// <param name="newSideLength">The side length of the new child instance, if necessary.</param>
        /// <returns>The instance held at the specified index, or the newly created instance.</returns>
        private static Octant GetOrSetChildOctant(ref Octant[] octants, int index, Vector4 newMidpoint, double newSideLength)
        {
            if (octants != null && octants?[index] == null)
            {
                octants[index] = new Octant(newMidpoint, newSideLength);
            }

            return octants?[index];
        }

        /// <summary>
        /// Whether the given point is within the bounds of this instance.
        /// </summary>
        /// <param name="point">
        /// The point whose position to check. Only the first 3 dimensions are used, the 4th is ignored.
        /// </param>
        /// <returns>Whether the given point is within the bounds of this instance.</returns>
        public bool ContainsPoint(Vector4 point)
        {
            return point.X <= midpoint.X + halfSideLength && point.X >= midpoint.X - halfSideLength &&
                   point.Y <= midpoint.Y + halfSideLength && point.Y >= midpoint.Y - halfSideLength &&
                   point.Z <= midpoint.Z + halfSideLength && point.Z >= midpoint.Z - halfSideLength;
        }

        /// <summary>
        /// Returns the specified child octant instance.
        /// </summary>
        /// <param name="specifier">Which child octant instance to return.</param>
        /// <returns>The specified child octant instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the given specifier does not equate to one of the values in the <see cref="PositionSpecifier"/> enum.
        /// </exception>
        public Octant SubOctant(int specifier) => SubOctant((PositionSpecifier)specifier);

        /// <summary>
        /// Returns the specified child octant instance.
        /// </summary>
        /// <param name="specifier">Which child octant instance to return.</param>
        /// <returns>The specified child octant instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the given specifier does not equate to one of the values in the <see cref="PositionSpecifier"/> enum.
        /// </exception>
        public Octant SubOctant(PositionSpecifier specifier)
        {
            switch (specifier)
            {
                case PositionSpecifier.TopNorthWest:
                    return GetOrSetChildOctant(ref childOctants, 0,
                        midpoint + new Vector4(-quarterSideLength, +quarterSideLength, +quarterSideLength),
                        halfSideLength);

                case PositionSpecifier.TopNorthEast:
                    return GetOrSetChildOctant(ref childOctants, 1,
                        midpoint + new Vector4(+quarterSideLength, +quarterSideLength, +quarterSideLength),
                        halfSideLength);

                case PositionSpecifier.TopSouthEast:
                    return GetOrSetChildOctant(ref childOctants, 2,
                        midpoint + new Vector4(+quarterSideLength, +quarterSideLength, -quarterSideLength),
                        halfSideLength);

                case PositionSpecifier.TopSouthWest:
                    return GetOrSetChildOctant(ref childOctants, 3,
                        midpoint + new Vector4(-quarterSideLength, +quarterSideLength, -quarterSideLength),
                        halfSideLength);

                case PositionSpecifier.BottomNorthWest:
                    return GetOrSetChildOctant(ref childOctants, 4,
                        midpoint + new Vector4(-quarterSideLength, -quarterSideLength, +quarterSideLength),
                        halfSideLength);

                case PositionSpecifier.BottomNorthEast:
                    return GetOrSetChildOctant(ref childOctants, 5,
                        midpoint + new Vector4(+quarterSideLength, -quarterSideLength, +quarterSideLength),
                        halfSideLength);

                case PositionSpecifier.BottomSouthEast:
                    return GetOrSetChildOctant(ref childOctants, 6,
                        midpoint + new Vector4(+quarterSideLength, -quarterSideLength, -quarterSideLength),
                        halfSideLength);

                case PositionSpecifier.BottomSouthWest:
                    return GetOrSetChildOctant(ref childOctants, 7,
                        midpoint + new Vector4(-quarterSideLength, -quarterSideLength, -quarterSideLength),
                        halfSideLength);

                default:
                    throw new ArgumentOutOfRangeException(nameof(specifier), specifier,
                        "The given specifier is outside of the valid range.");
            }
        }

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Octant - Width: {sideLength}, Midpoint: {midpoint} " +
                   $"TNW: {childOctants[0]?.ToString() ?? "null"}, TNE: {childOctants[1]?.ToString() ?? "null"}, " +
                   $"TSE: {childOctants[2]?.ToString() ?? "null"}, TSW: {childOctants[3]?.ToString() ?? "null"}, " +
                   $"BNW: {childOctants[4]?.ToString() ?? "null"}, BNE: {childOctants[5]?.ToString() ?? "null"}, " +
                   $"BSE: {childOctants[6]?.ToString() ?? "null"}, BSW: {childOctants[7]?.ToString() ?? "null"}";
        }

        #endregion Overrides of Object
    }
}