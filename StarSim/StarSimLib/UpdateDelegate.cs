using System.Collections.Generic;
using StarSimLib.Physics;

namespace StarSimLib
{
    /// <summary>
    /// Updates the given <see cref="Body"/> collections individual bodies, using the given delta time.
    /// </summary>
    /// <param name="bodies">The body collection.</param>
    /// <param name="deltaTime">The time step.</param>
    public delegate void UpdateDelegate(IEnumerable<Body> bodies, double deltaTime);
}