using System;

namespace StarSimLib
{
    /// <summary>
    /// A 4-by-4 matrix of <see cref="double"/> values.
    /// </summary>
    public struct Matrix4x4
    {
        /// <summary>
        /// The internal matrix representation.
        /// </summary>
        private readonly double[][] matrix;

        /// <summary>
        /// The default <see cref="MatrixValueGeneratorDelegate"/> to use when constructing a <see cref="Matrix4x4"/>
        /// from a given 2D <see cref="double"/> array. Simply copies over the values from the source matrix to the
        /// output matrix. Should the source matrix contain null fields, a 0 will be placed at the relevant field in
        /// the target matrix.
        /// </summary>
        public static readonly MatrixValueGeneratorDelegate CopyMatrixValuesGeneratorDelegate =
            (sourceMatrix, i, j) => sourceMatrix?[i]?[j] ?? 0;

        /// <summary>
        /// Initialises a new instance of the <see cref="Matrix4x4"/> struct.
        /// </summary>
        /// <param name="defaultValue">The value to which to initialise all fields in the matrix.</param>
        public Matrix4x4(double defaultValue) : this(null, (sourceMatrix, i, j) => defaultValue)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Matrix4x4"/> struct.
        /// </summary>
        /// <param name="sourceMatrix">The source matrix from which to copy over values.</param>
        public Matrix4x4(double[][] sourceMatrix) : this(sourceMatrix, CopyMatrixValuesGeneratorDelegate)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Matrix4x4"/> struct.
        /// </summary>
        /// <param name="sourceMatrix">The source matrix from which to copy over values.</param>
        public Matrix4x4(Matrix4x4 sourceMatrix) : this(sourceMatrix, CopyMatrixValuesGeneratorDelegate)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Matrix4x4"/> struct.
        /// </summary>
        /// <param name="sourceMatrix">The source matrix.</param>
        /// <param name="valueGenerator">
        /// The <see cref="MatrixValueGeneratorDelegate"/> to use to generate new matrix values. Can use the source matrix
        /// or can ignore it.
        /// </param>
        public Matrix4x4(double[][] sourceMatrix, MatrixValueGeneratorDelegate valueGenerator)
        {
            // construct the columns
            matrix = new double[4][];

            try
            {
                // for each column in the target matrix
                for (int i = 0; i < 4; i++)
                {
                    // construct the rows
                    matrix[i] = new double[4];

                    // for each row in the target matrix
                    for (int j = 0; j < 4; j++)
                    {
                        // set the value at [i, j] according to the given generator function
                        this[i, j] = valueGenerator(sourceMatrix, i, j);
                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new ArgumentException("Source array is not a minimum of 4x4 in size.", nameof(sourceMatrix), ex);
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Matrix4x4"/> struct.
        /// </summary>
        /// <param name="sourceMatrix">The source matrix.</param>
        /// <param name="valueGenerator">
        /// The <see cref="MatrixValueGeneratorDelegate"/> to use to generate new matrix values. Can use the source matrix
        /// or can ignore it.
        /// </param>
        public Matrix4x4(Matrix4x4 sourceMatrix, MatrixValueGeneratorDelegate valueGenerator) :
            this((double[][])sourceMatrix, valueGenerator)
        {
        }

        /// <summary>
        /// Generates a value to place in the target matrix (at [i, j]), using a source matrix and i and j coordinates.
        /// </summary>
        /// <param name="sourceMatrix">The source matrix to use.</param>
        /// <param name="i">The column index.</param>
        /// <param name="j">The row index</param>
        /// <returns>The value to place at the (i, j) position in the new matrix.</returns>
        public delegate double MatrixValueGeneratorDelegate(double[][] sourceMatrix, int i, int j);

        /// <summary>
        /// Indexes the matrix as a 2D array.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <param name="j">The row index.</param>
        /// <returns>The value at the specified field.</returns>
        public double this[int i, int j]
        {
            get { return matrix[i][j]; }
            set { matrix[i][j] = value; }
        }

        /// <summary>
        /// Implicitly converts a <see cref="Matrix4x4"/> to a 2D <see cref="double"/> array.
        /// </summary>
        /// <param name="matrix4x4">The matrix to convert.</param>
        public static implicit operator double[][](Matrix4x4 matrix4x4)
        {
            return matrix4x4.matrix;
        }

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Matrix4x4"/> and a <see cref="Vector4d"/>. This is
        /// the 'dot' operation for a vector and a matrix.
        /// </summary>
        /// <param name="matrix4x4">The matrix.</param>
        /// <param name="vector">The 3D vector.</param>
        /// <returns>The dot product of the <see cref="Matrix4x4"/> and the <see cref="Vector4d"/>.</returns>
        public static Vector4d operator *(Matrix4x4 matrix4x4, Vector4d vector)
        {
            return new Vector4d
            {
                X = vector.X * matrix4x4[0, 0] +
                           vector.Y * matrix4x4[1, 0] +
                           vector.Z * matrix4x4[2, 0] +
                           vector.W * matrix4x4[3, 0],
                Y = vector.X * matrix4x4[0, 1] +
                           vector.Y * matrix4x4[1, 1] +
                           vector.Z * matrix4x4[2, 1] +
                           vector.W * matrix4x4[3, 1],
                Z = vector.X * matrix4x4[0, 2] +
                           vector.Y * matrix4x4[1, 2] +
                           vector.Z * matrix4x4[2, 2] +
                           vector.W * matrix4x4[3, 2],
                W = vector.X * matrix4x4[0, 3] +
                           vector.Y * matrix4x4[1, 3] +
                           vector.Z * matrix4x4[2, 3] +
                           vector.W * matrix4x4[3, 3]
            };
        }

        /// <summary>
        /// Implements the multiplication operator for a <see cref="Matrix4x4"/> and a <see cref="Vector4d"/>. This is
        /// the 'dot' operation for a vector and a matrix.
        /// </summary>
        /// <param name="vector">The 3D vector.</param>
        /// <param name="matrix4x4">The matrix.</param>
        /// <returns>The dot product of the <see cref="Matrix4x4"/> and the <see cref="Vector4d"/>.</returns>
        public static Vector4d operator *(Vector4d vector, Matrix4x4 matrix4x4) => matrix4x4 * vector;

        #region Overrides of ValueType

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[" +
                   $"[{this[0, 0]}, {this[1, 0]}, {this[2, 0]}, {this[3, 0]}]," +
                   $"[{this[0, 1]}, {this[1, 1]}, {this[2, 1]}, {this[3, 1]}]," +
                   $"[{this[0, 2]}, {this[1, 2]}, {this[2, 2]}, {this[3, 2]}]," +
                   $"[{this[0, 3]}, {this[1, 3]}, {this[2, 3]}, {this[3, 3]}]" +
                   $"]";
        }

        #endregion Overrides of ValueType
    }
}