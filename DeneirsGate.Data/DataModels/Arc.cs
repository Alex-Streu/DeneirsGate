using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Arcs")]
    public class Arc
    {
        [Key]
        public Guid ArcKey { get; set; }
        [Required]
        public Guid CampaignKey { get; set; }
        [StringLength(150), Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [StringLength(150)]
        public string Map { get; set; }
        public bool IsActive { get; set; }
    }
}
