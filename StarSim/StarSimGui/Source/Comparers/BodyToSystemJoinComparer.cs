using System.Collections.Generic;
using StarSimLib.Models;

namespace StarSimGui.Source.Comparers
{
    /// <summary>
    /// Provides methods to compare two <see cref="BodyToSystemJoin"/> instances for equality.
    /// </summary>
    public class BodyToSystemJoinComparer : IEqualityComparer<BodyToSystemJoin>
    {
        /// <inheritdoc />
        public bool Equals(BodyToSystemJoin x, BodyToSystemJoin y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Id == y.Id && x.BodyId == y.BodyId && x.SystemId == y.SystemId;
        }

        /// <inheritdoc />
        public int GetHashCode(BodyToSystemJoin obj)
        {
            return obj.GetHashCode();
        }
    }
}