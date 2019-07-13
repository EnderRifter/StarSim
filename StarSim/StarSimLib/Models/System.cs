using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public System(ulong id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// The <see cref="BodyToSystemJoin"/> entities that map this <see cref="System"/> to the <see cref="Body"/>
        /// entities that it holds.
        /// </summary>
        [InverseProperty(nameof(BodyToSystemJoin.System))]
        public virtual ICollection<BodyToSystemJoin> BodyToSystemJoins { get; set; } = new List<BodyToSystemJoin>();

        /// <summary>
        /// The <see cref="User"/> who created this instance.
        /// </summary>
        [ForeignKey(nameof(CreatorId))]
        [Required(ErrorMessage = "System must have creator.")]
        public User Creator { get; set; }

        /// <summary>
        /// The Id of the <see cref="User"/> who created this instance.
        /// </summary>
        [Required(ErrorMessage = "System must have id of creator.")]
        public ulong CreatorId { get; set; }

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
        [Timestamp, Required(ErrorMessage = "System must have a timestamp associated with it.")]
        public byte[] Timestamp { get; set; }
    }
}