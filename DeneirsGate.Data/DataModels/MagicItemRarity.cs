using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("MagicItemRarities")]
    public class MagicItemRarity
    {
        [Key]
        public Guid RarityKey { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Rarity { get; set; }
    }
}
