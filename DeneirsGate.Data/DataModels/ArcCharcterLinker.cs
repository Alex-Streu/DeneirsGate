using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("ArcCharacterLinkers")]
    public class ArcCharacterLinker
    {
        [Key, Column(Order = 1)]
        public Guid ArcKey { get; set; }
        [Key, Column(Order = 2)]
        public Guid CharacterKey { get; set; }
    }
}
