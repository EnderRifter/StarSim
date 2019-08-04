using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using StarSimLib.Contexts;

namespace StarSimGui.Contexts
{
    /// <summary>
    /// Factory class for creating <see cref="SimulatorContext"/> instances.
    /// </summary>
    /// <remarks>
    /// This solves the problem of having the database context class in a common library, and trying to push migrations
    /// to a specific user of the library.
    /// SO question: https://stackoverflow.com/questions/46394494/ef-core-dbcontext-in-net-standard-2-0-library
    /// Solution 1 : https://docs.microsoft.com/en-gb/ef/core/miscellaneous/cli/dbcontext-creation
    /// Solution 2 : https://codingblast.com/entityframework-core-idesigntimedbcontextfactory/
    /// </remarks>
    public class SimulatorContextFactory : IDesignTimeDbContextFactory<SimulatorContext>
    {
        #region Implementation of IDesignTimeDbContextFactory<out TaxiContext>

        /// <inheritdoc />
        public SimulatorContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<SimulatorContext> optionsBuilder = new DbContextOptionsBuilder<SimulatorContext>();

            // we use SQLite due to the low overhead, ease of use, and great tooling (e.g. DB Browser for SQLite).
            optionsBuilder.UseSqlite(SimulatorContext.ConnectionString,
                builder => builder.MigrationsAssembly(nameof(StarSimGui)));

            return new SimulatorContext(optionsBuilder.Options);
        }

        #endregion Implementation of IDesignTimeDbContextFactory<out TaxiContext>
    }
}