using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("DungeonTiles")]
    public class DungeonTile
    {
        [Key]
        public Guid TileKey { get; set; }
        [Required]
        public Guid DungeonKey { get; set; }
        [Required]
        public int Row { get; set; }
        [Required]
        public int Column { get; set; }
        public string Description { get; set; }
        [Required, StringLength(150)]
        public string Image { get; set; }
        public int? Index { get; set; }
    }
}
