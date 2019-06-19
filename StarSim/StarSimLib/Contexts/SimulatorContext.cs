using Microsoft.EntityFrameworkCore;

namespace StarSimLib.Contexts
{
    /// <summary>
    /// Represents the solar simulation database.
    /// </summary>
    public class SimulatorContext : DbContext
    {
        /// <summary>
        /// The connection string used to access the simulator database.
        /// </summary>
        public const string ConnectionString = @"Data Source=Simulator.db";

        /// <summary>
        /// Constructs a new instance of the <see cref="SimulatorContext"/> class.
        /// </summary>
        public SimulatorContext() : base(new DbContextOptionsBuilder<SimulatorContext>().Options)
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="SimulatorContext"/> class.
        /// </summary>
        /// <param name="options">Any <see cref="DbContextOptions"/> that should be set on this instance.</param>
        public SimulatorContext(DbContextOptions options) : base(options)
        {
        }

        #region Overrides of DbContext

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // we use SQLite due to the low overhead, ease of use, and great tooling (e.g. DB Browser for SQLite).
            optionsBuilder.UseSqlite(ConnectionString);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #endregion Overrides of DbContext
    }
}