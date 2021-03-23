using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("ActivityLogs")]
    public class ActivityLog
    {
        [Key]
        public Guid LogKey { get; set; }
        [Required]
        public Guid ArcKey { get; set; }
        [Required]
        public DateTime DateLogged { get; set; }
        [Required]
        public string Log { get; set; }
        [Required]
        public int Type { get; set; }
    }
}
