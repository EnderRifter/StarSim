using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using StarSimLib;
using StarSimLib.Data_Structures;

namespace StarSimGui.Source
{
    /// <summary>
    /// Represents the editable fields in a <see cref="Body"/> instance.
    /// </summary>
    public class BodyDummy : INotifyPropertyChanged
    {
        /// <summary>
        /// The underlying mass.
        /// </summary>
        private double mass;

        /// <summary>
        /// The underlying position vector.
        /// </summary>
        private Vector4 positionVector;

        /// <summary>
        /// The underlying velocity vector.
        /// </summary>
        private Vector4 velocityVector;

        /// <summary>
        /// Initialises a new instance of the <see cref="BodyDummy"/> class.
        /// </summary>
        public BodyDummy()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="BodyDummy"/> class.
        /// </summary>
        /// <param name="body">The <see cref="Body"/> instance around which to construct a dummy.</param>
        public BodyDummy(Body body)
        {
            Generation = body.Generation;
            Id = body.Id;
            mass = body.Mass;
            positionVector = body.Position;
            velocityVector = body.Velocity;
        }

        /// <summary>
        /// Signals that a property on this instance has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The generation of this instance.
        /// </summary>
        public uint Generation { get; set; }

        /// <summary>
        /// The id of this instance.
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// The starting mass of this instance.
        /// </summary>
        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        /// <summary>
        /// The starting position X component of this instance.
        /// </summary>
        public double PosX
        {
            get { return positionVector.X; }
            set { positionVector.X = value; }
        }

        /// <summary>
        /// The starting position Y component of this instance.
        /// </summary>
        public double PosY
        {
            get { return positionVector.Y; }
            set { positionVector.Y = value; }
        }

        /// <summary>
        /// The starting position Z component of this instance.
        /// </summary>
        public double PosZ
        {
            get { return positionVector.Z; }
            set { positionVector.Z = value; }
        }

        /// <summary>
        /// The starting velocity X component of this instance.
        /// </summary>
        public double VelX
        {
            get { return velocityVector.X; }
            set { velocityVector.X = value; }
        }

        /// <summary>
        /// The starting velocity Y component of this instance.
        /// </summary>
        public double VelY
        {
            get { return velocityVector.Y; }
            set { velocityVector.Y = value; }
        }

        /// <summary>
        /// The starting velocity Z component of this instance.
        /// </summary>
        public double VelZ
        {
            get { return velocityVector.Z; }
            set { velocityVector.Z = value; }
        }

        /// <summary>
        /// Invokes the <see cref="PropertyChanged"/> event with the name of the caller property that changed.
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Implements an explicit conversion between a <see cref="BodyDummy"/> instance to a <see cref="Body"/> instance.
        /// </summary>
        /// <param name="instance">The <see cref="BodyDummy"/> instance to convert.</param>
        public static explicit operator Body(BodyDummy instance)
        {
            return new Body(instance.positionVector, instance.velocityVector, instance.Mass, instance.Generation, instance.Id);
        }
    }
}