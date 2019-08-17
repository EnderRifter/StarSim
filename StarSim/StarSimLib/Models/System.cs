using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StarSimLib.Data_Structures;

namespace StarSimLib.Models
{
    /// <summary>
    /// Represents a star system in the database, with a collection of child <see cref="Body"/> instances.
    /// </summary>
    public class System
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="System"/> class.
        /// </summary>
        /// <param name="id">The unique id of this instance.</param>
        /// <param name="name">The name of this instance.</param>
        /// <param name="bodyPositionData">Optional</param>
        public System(ulong id, string name, IReadOnlyDictionary<ulong, (Vector4 pos, Vector4 vel)> bodyPositionData = null)
        {
            Id = id;
            Name = name;
            BodyPositionData = bodyPositionData ?? new Dictionary<ulong, (Vector4 pos, Vector4 vel)>();
        }

        /// <summary>
        /// Stores the positions and velocities of every <see cref="Body"/> entity tracked by this instance. Will be
        /// serialised to a JSON string and then deserialised from a JSON string to be stored and retrieved from the
        /// database.
        /// </summary>
        [Required]
        public IReadOnlyDictionary<ulong, (Vector4 pos, Vector4 vel)> BodyPositionData { get; private set; }

        /// <summary>
        /// The <see cref="BodyToSystemJoin"/> entities that map this <see cref="System"/> to the <see cref="Body"/>
        /// entities that it holds.
        /// </summary>
        [InverseProperty(nameof(BodyToSystemJoin.System))]
        public virtual ICollection<BodyToSystemJoin> BodyToSystemJoins { get; set; } = new List<BodyToSystemJoin>();

        /// <summary>
        /// The unique primary key for this instance.
        /// </summary>
        [Key, Required(ErrorMessage = "System must have unique id.")]
        public ulong Id { get; set; }

        /// <summary>
        /// The displayed name of this instance.
        /// </summary>
        [Required(ErrorMessage = "System must have a name.")]
        [MinLength(0, ErrorMessage = "System's name can not be less than 0 characters long.")]
        [MaxLength(100, ErrorMessage = "System's name can not be more than 100 characters long")]
        public string Name { get; set; }

        /// <summary>
        /// A timestamp updated whenever the entity is handled by the database. Functions as a concurrency token to prevent
        /// multiple access to the same field.
        /// </summary>
        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}