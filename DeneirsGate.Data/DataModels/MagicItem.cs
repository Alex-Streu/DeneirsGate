using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("MagicItems")]
    public class MagicItem
    {
        [Key]
        public Guid ItemKey { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [Required]
        public Guid Type { get; set; }
        [Required]
        public Guid Rarity { get; set; }
        [Required]
        public bool HasAttunement { get; set; }
        public string Description { get; set; }
    }
}
