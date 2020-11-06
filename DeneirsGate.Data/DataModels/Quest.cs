using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Quests")]
    public class Quest
    {
        [Key]
        public Guid QuestKey { get; set; }
        [Required]
        public Guid ArcKey { get; set; }
        [StringLength(150), Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int SortOrder { get; set; }
        [Required]
        public int Status { get; set; }
    }
}
