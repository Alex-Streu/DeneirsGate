using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("QuestEvents")]
    public class QuestEvent
    {
        [Key]
        public Guid EventKey { get; set; }
        [Required]
        public Guid QuestKey { get; set; }
        [StringLength(150), Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int SortOrder { get; set; }
        [Required]
        public bool IsComplete { get; set; }
    }
}
