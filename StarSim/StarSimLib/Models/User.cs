using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarSimLib.Models
{
    /// <summary>
    /// Enumerates the possible privileges that a <see cref="User"/> can have, as flags to save space.
    /// </summary>
    [Flags]
    public enum UserPrivileges
    {
        /// <summary>
        /// Represents a user with the default privileges.
        /// </summary>
        Default = ViewSimulations | ViewKnownBodies,

        /// <summary>
        /// Represents a user with publishing privileges.
        /// </summary>
        Publisher = Default | PublishSimulations | ManageOwnSimulations | PublishKnownBodies | ManageOwnKnownBodies,

        /// <summary>
        /// Represents a user with administration privileges.
        /// </summary>
        Admin = Publisher | ManageForeignSimulations | ManageForeignKnownBodies | ViewUsers | ManageUsers,

        /// <summary>
        /// Allows a user to start published or random simulations and view them.
        /// </summary>
        ViewSimulations = 0,

        /// <summary>
        /// Allows a user to publish simulations for other users to view.
        /// </summary>
        PublishSimulations = 1,

        /// <summary>
        /// Allows a user to manage other user's published simulations.
        /// </summary>
        ManageForeignSimulations = 2,

        /// <summary>
        /// Allows a user to manage their own published simulations.
        /// </summary>
        ManageOwnSimulations = 4,

        /// <summary>
        /// Allows a user to view known bodies.
        /// </summary>
        ViewKnownBodies = 8,

        /// <summary>
        /// Allows a user to add new known bodies for other users to view.
        /// </summary>
        PublishKnownBodies = 16,

        /// <summary>
        /// Allows a user to edit their own published known bodies.
        /// </summary>
        ManageOwnKnownBodies = 32,

        /// <summary>
        /// Allows a user to edit other user's published known bodies.
        /// </summary>
        ManageForeignKnownBodies = 64,

        /// <summary>
        /// Allows a user to view other user's information.
        /// </summary>
        ViewUsers = 128,

        /// <summary>
        /// Allows a user to manage other user's information.
        /// </summary>
        ManageUsers = 256,
    }

    /// <summary>
    /// Represents a user in the database.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The unique id of this instance.</param>
        /// <param name="username">The username of this instance.</param>
        /// <param name="privileges">The privileges associated with this instance.</param>
        /// <param name="email">The optional email of this instance.</param>
        public User(ulong id, string username, UserPrivileges privileges, string email = null)
        {
            Id = id;
            Username = username;
            Privileges = privileges;
            Email = email;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The unique id of this instance.</param>
        /// <param name="username">The username of this instance.</param>
        /// <param name="privileges">The privileges associated with this instance.</param>
        /// <param name="passwordHash">The hash of the salted password of this instance.</param>
        /// <param name="passwordSalt">The salt used to hash the password.</param>
        /// <param name="email">The optional email of this instance.</param>
        public User(ulong id, string username, UserPrivileges privileges, byte[] passwordHash, byte[] passwordSalt, string email = null)
        {
            Id = id;
            Username = username;
            Privileges = privileges;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Email = email;
        }

        /// <summary>
        /// The <see cref="System"/> entities that this <see cref="User"/> has created.
        /// </summary>
        [InverseProperty(nameof(System.Creator))]
        public virtual ICollection<System> CreatedSystems { get; set; } = new List<System>();

        /// <summary>
        /// The email address of this instance.
        /// </summary>
        [MinLength(0, ErrorMessage = "User's email can not be less than 0 characters long.")]
        [MaxLength(100, ErrorMessage = "User's email can not be more than 100 characters long")]
        public string Email { get; set; }

        /// <summary>
        /// The unique primary key for this instance.
        /// </summary>
        [Key, Required(ErrorMessage = "User must have unique id.")]
        public ulong Id { get; set; }

        /// <summary>
        /// The hash of this instance's salted password.
        /// </summary>
        [Required(ErrorMessage = "User must have a password hash.")]
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// The salt prepended to this instance's password prior to hashing.
        /// </summary>
        [Required(ErrorMessage = "User must have a password salt.")]
        [MinLength(256, ErrorMessage = "User's password salt must be no shorter than 256 bytes long.")]
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// The privileges associated with this instance.
        /// </summary>
        [Required(ErrorMessage = "User account must have at least the default privilege associated with it.")]
        public UserPrivileges Privileges { get; set; }

        /// <summary>
        /// A timestamp updated whenever the entity is handled by the database. Functions as a concurrency token to prevent
        /// multiple access to the same field.
        /// </summary>
        [Timestamp]
        public byte[] Timestamp { get; set; }

        /// <summary>
        /// The displayed username of this instance.
        /// </summary>
        [Required(ErrorMessage = "User must have a username.")]
        [MinLength(0, ErrorMessage = "User's username can not be less than 0 characters long.")]
        [MaxLength(100, ErrorMessage = "User's username can not be more than 100 characters long")]
        public string Username { get; set; }
    }
}