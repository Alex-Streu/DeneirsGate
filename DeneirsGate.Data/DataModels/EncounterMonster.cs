using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("EncounterMonsters")]
    public class EncounterMonster
    {
        [Key, Column(Order = 0)]
        public Guid EncounterKey { get; set; }
        [Key, Column(Order = 1)]
        public Guid MonsterKey { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
