using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Transactions;

namespace StarSimLib
{
    /// <summary>
    /// A 3D vector with <see cref="double"/> components.
    /// </summary>
    public struct Vector3d
    {
        /// <summary>
        /// The x component of the vector.
        /// </summary>
        public double X;

        /// <summary>
        /// The y component of the vector.
        /// </summary>
        public double Y;

        /// <summary>
        /// The z component of the vector.
        /// </summary>
        public double Z;

        /// <summary>
        /// Initialises a new instance of the <see cref="Vector3d"/> struct. Sets Z to 0.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        public Vector3d(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Vector3d"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public Vector3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        #region Operator Overloads

        /// <summary>
        /// Implements the subtraction operator for 2 <see cref="Vector3d"/>s.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The other <see cref="Vector3d"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator -(Vector3d vector, Vector3d vector2)
        {
            vector.X -= vector2.X;
            vector.Y -= vector2.Y;
            vector.Z -= vector2.Z;

            return vector;
        }

        /// <summary>
        /// Implements the subtraction operator for a <see cref="Vector3d"/> and a (X, Y, Z) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The 3D tuple.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator -(Vector3d vector, (double X, double Y, double Z) vector2)
        {
            (double x, double y, double z) = vector2;

            vector.X -= x;
            vector.Y -= y;
            vector.Z -= z;

            return vector;
        }

        /// <summary>
        /// Implements the subtraction operator for a <see cref="Vector3d"/> and a (X, Y, Z) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The 3D tuple.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator -((double X, double Y, double Z) vector2, Vector3d vector) => vector - vector2;

        /// <summary>
        /// Implements the subtraction operator for a <see cref="Vector3d"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator -(Vector3d vector, double scalar)
        {
            vector.X -= scalar;
            vector.Y -= scalar;
            vector.Z -= scalar;

            return vector;
        }

        /// <summary>
        /// Implements the subtraction operator for a <see cref="Vector3d"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator -(double scalar, Vector3d vector) => vector - scalar;

        /// <summary>
        /// Implements the multiplication operator for 2 <see cref="Vector3d"/>s.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The other <see cref="Vector3d"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator *(Vector3d vector, Vector3d vector2)
        {
            vector.X *= vector2.X;
            vector.Y *= vector2.Y;
            vector.Z *= vector2.Z;

            return vector;
        }

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Vector3d"/> and a (X, Y, Z) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The 3D tuple.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator *(Vector3d vector, (double X, double Y, double Z) vector2)
        {
            (double x, double y, double z) = vector2;

            vector.X *= x;
            vector.Y *= y;
            vector.Z *= z;

            return vector;
        }

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Vector3d"/> and a (X, Y, Z) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The 3D tuple.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator *((double X, double Y, double Z) vector2, Vector3d vector) => vector * vector2;

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Vector3d"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator *(Vector3d vector, double scalar)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;

            return vector;
        }

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Vector3d"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator *(double scalar, Vector3d vector) => vector * scalar;

        /// <summary>
        /// Implements the division operator for 2 <see cref="Vector3d"/>s.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The other <see cref="Vector3d"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator /(Vector3d vector, Vector3d vector2)
        {
            vector.X /= vector2.X;
            vector.Y /= vector2.Y;
            vector.Z /= vector2.Z;

            return vector;
        }

        /// <summary>
        /// Implements the division operator for a <see cref="Vector3d"/> and a (X, Y, Z) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The 3D tuple.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator /(Vector3d vector, (double X, double Y, double Z) vector2)
        {
            (double x, double y, double z) = vector2;

            vector.X /= x;
            vector.Y /= y;
            vector.Z /= z;

            return vector;
        }

        /// <summary>
        /// Implements the division operator for a <see cref="Vector3d"/> and a (X, Y, Z) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The 3D tuple.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator /((double X, double Y, double Z) vector2, Vector3d vector) => vector / vector2;

        /// <summary>
        /// Implements the division operator for a <see cref="Vector3d"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator /(Vector3d vector, double scalar)
        {
            vector.X /= scalar;
            vector.Y /= scalar;
            vector.Z /= scalar;

            return vector;
        }

        /// <summary>
        /// Implements the division operator for a <see cref="Vector3d"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator /(double scalar, Vector3d vector) => vector / scalar;

        /// <summary>
        /// Implements the addition operator for 2 <see cref="Vector3d"/>s.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The other <see cref="Vector3d"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator +(Vector3d vector, Vector3d vector2)
        {
            vector.X += vector2.X;
            vector.Y += vector2.Y;
            vector.Z += vector2.Z;

            return vector;
        }

        /// <summary>
        /// Implements the addition operator for a <see cref="Vector3d"/> and a (X, Y, Z) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The 3D tuple.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator +(Vector3d vector, (double X, double Y, double Z) vector2)
        {
            (double x, double y, double z) = vector2;

            vector.X += x;
            vector.Y += y;
            vector.Z += z;

            return vector;
        }

        /// <summary>
        /// Implements the addition operator for a <see cref="Vector3d"/> and a (X, Y, Z) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="vector2">The 3D tuple.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator +((double X, double Y, double Z) vector2, Vector3d vector) => vector + vector2;

        /// <summary>
        /// Implements the addition operator for a <see cref="Vector3d"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator +(Vector3d vector, double scalar)
        {
            vector.X += scalar;
            vector.Y += scalar;
            vector.Z += scalar;

            return vector;
        }

        /// <summary>
        /// Implements the addition operator for a <see cref="Vector3d"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector3d"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector3d"/>.</returns>
        public static Vector3d operator +(double scalar, Vector3d vector) => vector + scalar;

        #endregion Operator Overloads

        /// <summary>
        /// Returns the absolute value of this <see cref="Vector3d"/>.
        /// </summary>
        /// <returns>The absolute value (magnitude) of this <see cref="Vector3d"/>, as a <see cref="double"/>.</returns>
        [Pure]
        public double Abs()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Returns the magnitude of this <see cref="Vector3d"/>.
        /// </summary>
        /// <returns>The magnitude (absolute value) of this <see cref="Vector3d"/>, as a <see cref="double"/>.</returns>
        [Pure]
        public double Magnitude()
        {
            return Abs();
        }

        /// <summary>
        /// Holds unit vectors.
        /// </summary>
        public static class UnitVectors
        {
            /// <summary>
            /// The negative unit vector for the Z axis.
            /// </summary>
            public static readonly Vector3d Backwards = new Vector3d(0, 0, -1);

            /// <summary>
            /// The negative unit vector for the Y axis.
            /// </summary>
            public static readonly Vector3d Down = new Vector3d(0, -1, 0);

            /// <summary>
            /// The positive unit vector for the Z axis.
            /// </summary>
            public static readonly Vector3d Forwards = new Vector3d(0, 0, 1);

            /// <summary>
            /// The positive unit vector for the X axis.
            /// </summary>
            public static readonly Vector3d Left = new Vector3d(1, 0, 0);

            /// <summary>
            /// The negative unit vector for the X axis.
            /// </summary>
            public static readonly Vector3d Right = new Vector3d(-1, 0, 0);

            /// <summary>
            /// The positive unit vector for the Y axis.
            /// </summary>
            public static readonly Vector3d Up = new Vector3d(0, 1, 0);
        }

        #region Overrides of ValueType

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X}, {Y}, {Z}";
        }

        #endregion Overrides of ValueType
    }
}