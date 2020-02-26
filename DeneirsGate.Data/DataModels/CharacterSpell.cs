using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("CharacterSpells")]
    public class CharacterSpell
    {
        [Key]
        public Guid SpellKey { get; set; }
        
        public Guid CharacterKey { get; set; }

        public int Level { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }
    }
}
