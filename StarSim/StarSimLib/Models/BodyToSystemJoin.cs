using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarSimLib.Models
{
    /// <summary>
    /// Maps a <see cref="Models.Body"/> entity and <see cref="Models.System"/> entity in a many-to-many relationship in the database.
    /// </summary>
    public class BodyToSystemJoin
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="BodyToSystemJoin"/> class.
        /// </summary>
        /// <param name="id">The unique id for this instance.</param>
        /// <param name="bodyId">The id of the body mapped by this instance.</param>
        /// <param name="systemId">The id of the system mapped by this instance.</param>
        public BodyToSystemJoin(ulong id, ulong bodyId, ulong systemId)
        {
            Id = id;
            BodyId = bodyId;
            SystemId = systemId;
        }

        /// <summary>
        /// The <see cref="Models.Body"/> instance mapped by this join instance.
        /// </summary>
        [ForeignKey(nameof(BodyId))]
        [Required(ErrorMessage = "Body-to-System join must map a body.")]
        public Body Body { get; set; }

        /// <summary>
        /// The Id of the <see cref="Models.Body"/> instance mapped by this join instance.
        /// </summary>
        [Required(ErrorMessage = "Body-to-System join must have the mapped body's id.")]
        public ulong BodyId { get; set; }

        /// <summary>
        /// The unique primary key for this instance.
        /// </summary>
        [Key, Required(ErrorMessage = "Body-to-System join must have unique id.")]
        public ulong Id { get; set; }

        /// <summary>
        /// The <see cref="Models.System"/> instance mapped by this join instance.
        /// </summary>
        [ForeignKey(nameof(SystemId))]
        [Required(ErrorMessage = "Body-to-System join must map a system.")]
        public System System { get; set; }

        /// <summary>
        /// The Id of the <see cref="Models.System"/> instance mapped by this join instance.
        /// </summary>
        [Required(ErrorMessage = "Body-to-System join must have the mapped system's id.")]
        public ulong SystemId { get; set; }

        /// <summary>
        /// A timestamp updated whenever the entity is handled by the database. Functions as a concurrency token to prevent
        /// multiple access to the same field.
        /// </summary>
        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}