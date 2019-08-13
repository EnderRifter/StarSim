using System;
using System.Diagnostics.Contracts;

namespace StarSimLib.Data_Structures
{
    /// <summary>
    /// A 4D vector with <see cref="double"/> components.
    /// </summary>
    public struct Vector4
    {
        /// <summary>
        /// The w component of the vector.
        /// </summary>
        public double W;

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
        /// Initialises a new instance of the <see cref="Vector4"/> struct. Sets Z and W to 0.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        public Vector4(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
            W = 0;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Vector4"/> struct. Sets W to 0.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public Vector4(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 0;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Vector4"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        /// <param name="w">The w component.</param>
        public Vector4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #region Operator Overloads

        /// <summary>
        /// Implements the subtraction operator for 2 <see cref="Vector4"/>s.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The other <see cref="Vector4"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator -(Vector4 vector, Vector4 vector2)
        {
            vector.X -= vector2.X;
            vector.Y -= vector2.Y;
            vector.Z -= vector2.Z;
            vector.W -= vector2.W;

            return vector;
        }

        /// <summary>
        /// Implements the subtraction operator for a <see cref="Vector4"/> and a (X, Y, Z, W) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The 4D tuple.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator -(Vector4 vector, (double X, double Y, double Z, double W) vector2)
        {
            (double x, double y, double z, double w) = vector2;

            vector.X -= x;
            vector.Y -= y;
            vector.Z -= z;
            vector.W -= w;

            return vector;
        }

        /// <summary>
        /// Implements the subtraction operator for a <see cref="Vector4"/> and a (X, Y, Z, W) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The 4D tuple.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator -((double X, double Y, double Z, double W) vector2, Vector4 vector) => vector - vector2;

        /// <summary>
        /// Implements the subtraction operator for a <see cref="Vector4"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator -(Vector4 vector, double scalar)
        {
            vector.X -= scalar;
            vector.Y -= scalar;
            vector.Z -= scalar;
            vector.W -= scalar;

            return vector;
        }

        /// <summary>
        /// Implements the subtraction operator for a <see cref="Vector4"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator -(double scalar, Vector4 vector) => vector - scalar;

        /// <summary>
        /// Implements the multiplication operator for 2 <see cref="Vector4"/>s.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The other <see cref="Vector4"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator *(Vector4 vector, Vector4 vector2)
        {
            vector.X *= vector2.X;
            vector.Y *= vector2.Y;
            vector.Z *= vector2.Z;
            vector.W *= vector2.W;

            return vector;
        }

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Vector4"/> and a (X, Y, Z, W) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The 4D tuple.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator *(Vector4 vector, (double X, double Y, double Z, double W) vector2)
        {
            (double x, double y, double z, double w) = vector2;

            vector.X *= x;
            vector.Y *= y;
            vector.Z *= z;
            vector.W *= w;

            return vector;
        }

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Vector4"/> and a (X, Y, Z, W) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The 4D tuple.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator *((double X, double Y, double Z, double W) vector2, Vector4 vector) => vector * vector2;

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Vector4"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator *(Vector4 vector, double scalar)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            vector.W *= scalar;

            return vector;
        }

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Vector4"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator *(double scalar, Vector4 vector) => vector * scalar;

        /// <summary>
        /// Implements the division operator for 2 <see cref="Vector4"/>s.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The other <see cref="Vector4"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator /(Vector4 vector, Vector4 vector2)
        {
            vector.X /= vector2.X;
            vector.Y /= vector2.Y;
            vector.Z /= vector2.Z;
            vector.W /= vector2.W;

            return vector;
        }

        /// <summary>
        /// Implements the division operator for a <see cref="Vector4"/> and a (X, Y, Z, W) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The 4D tuple.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator /(Vector4 vector, (double X, double Y, double Z, double W) vector2)
        {
            (double x, double y, double z, double w) = vector2;

            vector.X /= x;
            vector.Y /= y;
            vector.Z /= z;
            vector.W /= w;

            return vector;
        }

        /// <summary>
        /// Implements the division operator for a <see cref="Vector4"/> and a (X, Y, Z, W) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The 4D tuple.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator /((double X, double Y, double Z, double W) vector2, Vector4 vector) => vector / vector2;

        /// <summary>
        /// Implements the division operator for a <see cref="Vector4"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator /(Vector4 vector, double scalar)
        {
            vector.X /= scalar;
            vector.Y /= scalar;
            vector.Z /= scalar;
            vector.W /= scalar;

            return vector;
        }

        /// <summary>
        /// Implements the division operator for a <see cref="Vector4"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator /(double scalar, Vector4 vector) => vector / scalar;

        /// <summary>
        /// Implements the addition operator for 2 <see cref="Vector4"/>s.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The other <see cref="Vector4"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator +(Vector4 vector, Vector4 vector2)
        {
            vector.X += vector2.X;
            vector.Y += vector2.Y;
            vector.Z += vector2.Z;
            vector.W += vector2.W;

            return vector;
        }

        /// <summary>
        /// Implements the addition operator for a <see cref="Vector4"/> and a (X, Y, Z, W) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The 4D tuple.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator +(Vector4 vector, (double X, double Y, double Z, double W) vector2)
        {
            (double x, double y, double z, double w) = vector2;

            vector.X += x;
            vector.Y += y;
            vector.Z += z;
            vector.W += w;

            return vector;
        }

        /// <summary>
        /// Implements the addition operator for a <see cref="Vector4"/> and a (X, Y, Z, W) tuple.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="vector2">The 4D tuple.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator +((double X, double Y, double Z, double W) vector2, Vector4 vector) => vector + vector2;

        /// <summary>
        /// Implements the addition operator for a <see cref="Vector4"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator +(Vector4 vector, double scalar)
        {
            vector.X += scalar;
            vector.Y += scalar;
            vector.Z += scalar;
            vector.W += scalar;

            return vector;
        }

        /// <summary>
        /// Implements the addition operator for a <see cref="Vector4"/> and a scalar <see cref="double"/> value.
        /// </summary>
        /// <param name="vector">The original <see cref="Vector4"/>.</param>
        /// <param name="scalar">The scalar <see cref="double"/>.</param>
        /// <returns>The new <see cref="Vector4"/>.</returns>
        public static Vector4 operator +(double scalar, Vector4 vector) => vector + scalar;

        #endregion Operator Overloads

        /// <summary>
        /// Indexes the vector as a 1D array.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>The value at the specified field.</returns>
        public double this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return X;

                    case 1:
                        return Y;

                    case 2:
                        return Z;

                    case 3:
                        return W;

                    default:
                        return 0;
                }
            }

            set
            {
                switch (i)
                {
                    case 0:
                        X = value;
                        break;

                    case 1:
                        Y = value;
                        break;

                    case 2:
                        Z = value;
                        break;

                    case 3:
                        W = value;
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the absolute value of this <see cref="Vector4"/>.
        /// </summary>
        /// <returns>The absolute value (magnitude) of this <see cref="Vector4"/>, as a <see cref="double"/>.</returns>
        [Pure]
        public double Abs()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        /// <summary>
        /// Returns the magnitude of this <see cref="Vector4"/>.
        /// </summary>
        /// <returns>The magnitude (absolute value) of this <see cref="Vector4"/>, as a <see cref="double"/>.</returns>
        [Pure]
        public double Magnitude()
        {
            return Abs();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}, {W}]";
        }

        /// <summary>
        /// Holds unit vectors.
        /// </summary>
        public static class UnitVectors
        {
            /// <summary>
            /// The negative unit vector for the Z axis.
            /// </summary>
            public static readonly Vector4 Backwards = new Vector4(0, 0, -1);

            /// <summary>
            /// The negative unit vector for the Y axis.
            /// </summary>
            public static readonly Vector4 Down = new Vector4(0, -1, 0);

            /// <summary>
            /// The positive unit vector for the Z axis.
            /// </summary>
            public static readonly Vector4 Forwards = new Vector4(0, 0, 1);

            /// <summary>
            /// The positive unit vector for the X axis.
            /// </summary>
            public static readonly Vector4 Left = new Vector4(1, 0, 0);

            /// <summary>
            /// The negative unit vector for the X axis.
            /// </summary>
            public static readonly Vector4 Right = new Vector4(-1, 0, 0);

            /// <summary>
            /// The positive unit vector for the Y axis.
            /// </summary>
            public static readonly Vector4 Up = new Vector4(0, 1, 0);
        }
    }
}