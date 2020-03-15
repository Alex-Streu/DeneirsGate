using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public Guid NotificationKey { get; set; }

        public Guid UserKey { get; set; }

        [StringLength(100)]
        public string Message { get; set; }

        [StringLength(250)]
        public string Link { get; set; }

        public DateTime DateAdded { get; set; }

        public bool IsRead { get; set; }
    }
}
