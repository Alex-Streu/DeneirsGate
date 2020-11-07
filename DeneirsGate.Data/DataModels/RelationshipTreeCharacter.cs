using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("RelationshipTreeCharacters")]
    public class RelationshipTreeCharacter
    {
        [Key, Column(Order = 0)]
        public Guid TreeKey { get; set; }

        [Key, Column(Order = 1)]
        public Guid CharacterKey { get; set; }

        public Guid TierKey { get; set; }

        public bool IsShallow { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public string Backstory { get; set; }
    }
}
