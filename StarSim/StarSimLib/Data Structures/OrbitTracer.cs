using System.Collections.Generic;

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
        /// for less computation. The previous position will be saved once every 15 sampling opportunities.
        /// </summary>
        private const int PositionSampleRate = 15;

        /// <summary>
        /// Backing field for the <see cref="PreviousPositions"/> property.
        /// </summary>
        private readonly Queue<Vector4> previousPositions;

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
                return;
            }

            previousPositions.Enqueue(position);

            if (previousPositions.Count >= Constants.StoredPreviousPositionCount)
            {
                previousPositions.Dequeue();
            }

            // reset the counter
            positionSampleCounter = 0;
        }
    }
}