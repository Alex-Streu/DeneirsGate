using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("DungeonTileEncounters")]
    public class DungeonTileEncounter
    {
        [Key, Column(Order = 1)]
        public Guid TileKey { get; set; }
        [Key, Column(Order = 2)]
        public Guid EncounterKey { get; set; }
    }
}
