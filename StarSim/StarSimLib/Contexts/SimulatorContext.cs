using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StarSimLib.Cryptography;
using StarSimLib.Data_Structures;
using StarSimLib.Models;
using Body = StarSimLib.Models.Body;

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
        /// Initialises a new instance of the <see cref="SimulatorContext"/> class.
        /// </summary>
        public SimulatorContext() : base(new DbContextOptionsBuilder<SimulatorContext>().Options)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="SimulatorContext"/> class.
        /// </summary>
        /// <param name="options">Any <see cref="DbContextOptions"/> that should be set on this instance.</param>
        public SimulatorContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Holds the <see cref="Models.Body"/> entities in the database. Also allows querying the database via LINQ.
        /// </summary>
        public DbSet<Body> Bodies { get; set; }

        /// <summary>
        /// Holds the <see cref="BodyToSystemJoin"/> entities in the database. Also allows querying the database via LINQ.
        /// </summary>
        public DbSet<BodyToSystemJoin> BodyToSystemJoins { get; set; }

        /// <summary>
        /// Holds the <see cref="PublishedSystem"/> entities in the database. Also allows querying the database via LINQ.
        /// </summary>
        public DbSet<PublishedSystem> PublishedSystems { get; set; }

        /// <summary>
        /// Holds the <see cref="Models.System"/> entities in the database. Also allows querying the database via LINQ.
        /// </summary>
        public DbSet<Models.System> Systems { get; set; }

        /// <summary>
        /// Holds the <see cref="User"/> entities in the database. Also allows querying the database via LINQ.
        /// </summary>
        public DbSet<User> Users { get; set; }

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
            modelBuilder.Entity<Body>().HasIndex(body => body.Id).IsUnique();

            modelBuilder.Entity<Body>().Property(body => body.Name).HasDefaultValue("UNNAMED");

            modelBuilder.Entity<Body>().HasData(
                new Body(1, "Sagittarius A*", Constants.CentralBodyMass),
                new Body(2, "Sol", Constants.SolarMass),
                new Body(3, "Earth", Constants.SolarMass)
            );

            modelBuilder.Entity<Models.System>().HasIndex(system => system.Id).IsUnique();

            modelBuilder.Entity<Models.System>().Property(system => system.BodyPositionData)
                        .HasConversion(
                            deserialisedDict => JsonConvert.SerializeObject(deserialisedDict),
                            serialisedDict => JsonConvert.DeserializeObject<Dictionary<ulong, (Vector4, Vector4)>>(serialisedDict));

            modelBuilder.Entity<Models.System>().HasData(
                new Models.System(1, "Test", new Dictionary<ulong, (Vector4 pos, Vector4 vel)>
                                             {
                                                 { 1, (new Vector4(), new Vector4()) },
                                                 { 2, (new Vector4(-8.85E15, 7.46E15, -4.86E15), new Vector4(7.96E4, 6.93E-9, 6.63E4)) },
                                                 { 3, (new Vector4(-8.23E15, 7.89E15, 9.38E15), new Vector4(-6.84E4, -4.91E-9, -6.56E4)) }
                                             })
            );

            modelBuilder.Entity<BodyToSystemJoin>().HasIndex(join => new { join.BodyId, join.SystemId }).IsUnique();

            modelBuilder.Entity<BodyToSystemJoin>().HasData(
                new BodyToSystemJoin(1, 1, 1),
                new BodyToSystemJoin(2, 2, 1),
                new BodyToSystemJoin(3, 3, 1)
            );

            modelBuilder.Entity<PublishedSystem>().HasIndex(system => new { system.PublisherId, system.SystemId }).IsUnique();

            modelBuilder.Entity<PublishedSystem>().HasData(
                new PublishedSystem(1, 1, 1, DateTime.Now)
                );

            modelBuilder.Entity<User>().HasIndex(user => user.Id).IsUnique();

            modelBuilder.Entity<User>().Property(user => user.Email).HasDefaultValue("john.doe@gmail.com");

            byte[] defaultUserSalt = CryptographyHelper.GenerateSalt(), defaultUserPasswordBytes = CryptographyHelper.StringToBytes("Default");
            byte[] publisherSalt = CryptographyHelper.GenerateSalt(), publisherPasswordBytes = CryptographyHelper.StringToBytes("Publish");
            byte[] adminSalt = CryptographyHelper.GenerateSalt(), adminPasswordBytes = CryptographyHelper.StringToBytes("Admin");
            byte[] testUserSalt = CryptographyHelper.GenerateSalt(), testUserPasswordBytes = CryptographyHelper.StringToBytes("Test123");

            modelBuilder.Entity<User>().HasData(
                new User(1, "User", UserPrivileges.Default, CryptographyHelper.GenerateHash(defaultUserPasswordBytes, defaultUserSalt), defaultUserSalt),
                new User(2, "Publisher", UserPrivileges.Publisher, CryptographyHelper.GenerateHash(publisherPasswordBytes, publisherSalt), publisherSalt),
                new User(3, "Administrator", UserPrivileges.Admin, CryptographyHelper.GenerateHash(adminPasswordBytes, adminSalt), adminSalt),
                new User(4, "John Doe", UserPrivileges.Default, CryptographyHelper.GenerateHash(testUserPasswordBytes, testUserSalt), testUserSalt)
            );
        }

        #endregion Overrides of DbContext
    }
}