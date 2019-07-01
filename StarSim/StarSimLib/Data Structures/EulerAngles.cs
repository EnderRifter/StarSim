namespace StarSimLib.Data_Structures
{
    /// <summary>
    /// Represents a set of 3 angles, that together describe a 3D rotation.
    /// </summary>
    public struct EulerAngles
    {
        /// <summary>
        /// Angle of rotation in the x axis.
        /// </summary>
        public double X;

        /// <summary>
        /// Angle of rotation in the y axis.
        /// </summary>
        public double Y;

        /// <summary>
        /// Angle of rotation in the z axis.
        /// </summary>
        public double Z;

        /// <summary>
        /// Initialises a new instance of the <see cref="EulerAngles"/> struct.
        /// </summary>
        /// <param name="x">The angle of rotation in the x axis.</param>
        /// <param name="y">The angle of rotation in the y axis.</param>
        /// <param name="z">The angle of rotation in the z axis.</param>
        public EulerAngles(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}