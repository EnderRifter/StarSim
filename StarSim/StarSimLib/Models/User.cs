using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StarSimLib.Cryptography;

namespace StarSimLib.Models
{
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
        /// <param name="email">The optional email of this instance.</param>
        public User(ulong id, string username, string email = null)
        {
            Id = id;
            Username = username;
            Email = email;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The unique id of this instance.</param>
        /// <param name="username">The username of this instance.</param>
        /// <param name="passwordHash">The hash of the salted password of this instance.</param>
        /// <param name="passwordSalt">The salt used to hash the password.</param>
        /// <param name="email">The optional email of this instance.</param>
        public User(ulong id, string username, byte[] passwordHash, byte[] passwordSalt, string email = null)
        {
            Id = id;
            Username = username;
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
        /// A timestamp updated whenever the entity is handled by the database. Functions as a concurrency token to prevent
        /// multiple access to the same field.
        /// </summary>
        [Timestamp, Required(ErrorMessage = "User must have a timestamp associated with it.")]
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