using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarSimLib.Models
{
    /// <summary>
    /// Represents a published system in the database.
    /// </summary>
    public class PublishedSystem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PublishedSystem"/> class.
        /// </summary>
        /// <param name="id">The unique id of this instance.</param>
        /// <param name="publisherId">The unique id of the publisher of this instance.</param>
        /// <param name="systemId">The unique id of the system that was published in this instance.</param>
        /// <param name="publishDate">The date this instance was published on.</param>
        public PublishedSystem(ulong id, ulong publisherId, ulong systemId, DateTime publishDate)
        {
            Id = id;
            PublisherId = publisherId;
            SystemId = systemId;
            PublishDate = publishDate;
        }

        /// <summary>
        /// The unique primary key for this instance.
        /// </summary>
        [Key, Required(ErrorMessage = "Published system must have unique id.")]
        public ulong Id { get; set; }

        /// <summary>
        /// The date on which this instance was published.
        /// </summary>
        [Required(ErrorMessage = "Published system must have a publish date.")]
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// The <see cref="User"/> who created this instance.
        /// </summary>
        [ForeignKey(nameof(PublisherId))]
        [Required(ErrorMessage = "Published system must have creator.")]
        public User Publisher { get; set; }

        /// <summary>
        /// The Id of the <see cref="User"/> who created this instance.
        /// </summary>
        [Required(ErrorMessage = "Published system must have id of creator.")]
        public ulong PublisherId { get; set; }

        /// <summary>
        /// The <see cref="Models.System"/> instance that was published..
        /// </summary>
        [ForeignKey(nameof(SystemId))]
        [Required(ErrorMessage = "Published system must have published system.")]
        public System System { get; set; }

        /// <summary>
        /// The Id of the <see cref="Models.System"/> instance which was published..
        /// </summary>
        [Required(ErrorMessage = "Published system must have id of published system.")]
        public ulong SystemId { get; set; }

        /// <summary>
        /// A timestamp updated whenever the entity is handled by the database. Functions as a concurrency token to prevent
        /// multiple access to the same field.
        /// </summary>
        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}