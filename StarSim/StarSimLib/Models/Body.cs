using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarSimLib.Models
{
    /// <summary>
    /// Represents a known stellar body in the database. Maps to a <see cref="Data_Structures.Body"/> instance.
    /// </summary>
    public class Body
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="Body"/> class.
        /// </summary>
        /// <param name="id">The unique id for this instance.</param>
        /// <param name="name">The name for this instance.</param>
        /// <param name="mass">The mass for this instance.</param>
        public Body(ulong id, string name, double mass)
        {
            Id = id;
            Name = name;
            Mass = mass;
        }

        /// <summary>
        /// The <see cref="BodyToSystemJoin"/> entities that map this <see cref="Body"/> to the <see cref="System"/>
        /// entities that hold it.
        /// </summary>
        [InverseProperty(nameof(BodyToSystemJoin.Body))]
        public virtual ICollection<BodyToSystemJoin> BodyToSystemJoins { get; set; } = new List<BodyToSystemJoin>();

        /// <summary>
        /// The unique primary key for this instance.
        /// </summary>
        [Key, Required(ErrorMessage = "Body must have unique id.")]
        public ulong Id { get; set; }

        /// <summary>
        /// The mass of this instance.
        /// </summary>
        [Required(ErrorMessage = "Body must have a mass.")]
        public double Mass { get; set; }

        /// <summary>
        /// The displayed name of this instance.
        /// </summary>
        [Required(ErrorMessage = "Body must have a name.")]
        [MinLength(0, ErrorMessage = "Body's name can not be less than 0 characters long.")]
        [MaxLength(100, ErrorMessage = "Body's name can not be more than 100 characters long")]
        public string Name { get; set; }

        /// <summary>
        /// A timestamp updated whenever the entity is handled by the database. Functions as a concurrency token to prevent
        /// multiple access to the same field.
        /// </summary>
        [Timestamp, Required(ErrorMessage = "Body must have a timestamp associated with it.")]
        public byte[] Timestamp { get; set; }
    }
}