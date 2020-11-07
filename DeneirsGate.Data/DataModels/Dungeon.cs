using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Dungeons")]
    public class Dungeon
    {
        [Key]
        public Guid DungeonKey { get; set; }
        [StringLength(150), Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
