using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("DungeonTileTraps")]
    public class DungeonTileTrap
    {
        [Key]
        public Guid TrapKey { get; set; }
        [Required]
        public Guid TileKey { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public Guid NatureKey { get; set; }
        [Required]
        public Guid TypeKey { get; set; }
        public int? SaveDC { get; set; }
        public int? AttackBonus { get; set; }
        public string Damage { get; set; }
    }
}
