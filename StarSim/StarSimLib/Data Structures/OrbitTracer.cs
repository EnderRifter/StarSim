using System.Collections.Generic;
using StarSimLib.Physics;

namespace StarSimLib.Data_Structures
{
    /// <summary>
    /// Represents a series of points denoting the previous positions of a <see cref="Body"/> instance. Used to render
    /// an orbit tracer behind the <see cref="Body"/>.
    /// </summary>
    public class OrbitTracer
    {
        /// <summary>
        /// Sample rate for the previous position. Used to improve performance and get a longer orbit tracer tail
        /// for less computation. The previous position will be saved once every 20 sampling opportunities.
        /// </summary>
        private const int PositionSampleRate = 20;

        /// <summary>
        /// Backing field for the <see cref="PreviousPositions"/> property.
        /// </summary>
        private readonly Queue<Vector4> previousPositions;

        /// <summary>
        /// Due to the <see cref="Queue{T}"/>s inability to fetch the second-to-last element, we must explicitly
        /// cache it in order to use it in interpolation  calculations later.
        /// </summary>
        private Vector4 dequeuedPosition;

        /// <summary>
        /// Counts the number of sampling opportunities that have gone by since the last position sample. Resets once
        /// it reaches the value of <see cref="PositionSampleRate"/>.
        /// </summary>
        private int positionSampleCounter;

        /// <summary>
        /// Initialises a new instance of the <see cref="OrbitTracer"/> class.
        /// </summary>
        public OrbitTracer()
        {
            previousPositions = new Queue<Vector4>(Constants.StoredPreviousPositionCount);
        }

        /// <summary>
        /// An <see cref="Queue{T}"/> containing previous <see cref="Vector4"/> positions of the body.
        /// </summary>
        public ref readonly Queue<Vector4> PreviousPositions
        {
            get { return ref previousPositions; }
        }

        /// <summary>
        /// Clears the <see cref="Queue{T}"/> holding the previous positions.
        /// </summary>
        public void Clear()
        {
            previousPositions.Clear();
        }

        /// <summary>
        /// Enqueues the current position on the <see cref="previousPositions"/> queue, to save it.
        /// Will dequeue positions from the queue if the number of stored positions exceeds the
        /// value in <see cref="Constants.StoredPreviousPositionCount"/>. Will only enqueue the
        /// current position if the <see cref="PositionSampleRate"/> is met.
        /// </summary>
        public void Enqueue(Vector4 position)
        {
            // if the sample rate limit has not yet been met, don't sample a position but instead edit the last stored
            // position using linear interpolation to give a smooth shrinking motion
            if (++positionSampleCounter < PositionSampleRate)
            {
                /*
                double interpolationPercentage = positionSampleCounter / (float)PositionSampleRate;

                previousPositions[0].X = (1 - interpolationPercentage) * previousPositions[0].X + interpolationPercentage * previousPositions[1].X;
                previousPositions[0].Y = (1 - interpolationPercentage) * previousPositions[0].Y + interpolationPercentage * previousPositions[1].Y;
                previousPositions[0].Z = (1 - interpolationPercentage) * previousPositions[0].Z + interpolationPercentage * previousPositions[1].Z;
                */

                return;
            }

            previousPositions.Enqueue(position);

            if (previousPositions.Count >= Constants.StoredPreviousPositionCount)
            {
                dequeuedPosition = previousPositions.Dequeue();
            }

            // reset the counter
            positionSampleCounter = 0;
        }
    }
}