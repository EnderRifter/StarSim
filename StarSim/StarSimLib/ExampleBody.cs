using System;

namespace StarSimLib
{
    public class ExampleBody
    {
        private static readonly double G = 6.673e-11;   // gravitational constant
        private static readonly double solarmass = 1.98892e30;

        public double pos_x, pos_y;
        public double vel_x, vel_y;
        public double force_x, force_y;
        public double mass;

        public ExampleBody(double px, double py, double vx, double vy, double mass)
        {
            pos_x = px;
            pos_y = py;
            vel_x = vx;
            vel_y = vy;
            this.mass = mass;
        }

        public void update(double delta_time)
        {
            vel_x += delta_time * force_x / mass;
            vel_y += delta_time * force_y / mass;
            pos_x += delta_time * vel_x;
            pos_y += delta_time * vel_y;
        }

        public double distanceTo(ExampleBody b)
        {
            double dx = pos_x - b.pos_x;
            double dy = pos_y - b.pos_y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public void resetForce()
        {
            force_x = 0.0;
            force_y = 0.0;
        }

        public void addForce(ExampleBody b)
        {
            ExampleBody a = this;
            double softening_factor = 3E4;
            double dx = b.pos_x - a.pos_x;
            double dy = b.pos_y - a.pos_y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            double force = (G * a.mass * b.mass) / (dist * dist + softening_factor * softening_factor);

            a.force_x += force * dx / dist;
            a.force_y += force * dy / dist;
        }

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Body @ ({pos_x}, {pos_y}) with velocity ({vel_x}, {vel_y})";
        }

        #endregion Overrides of Object
    }
}