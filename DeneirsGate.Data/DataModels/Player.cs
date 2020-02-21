using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Players")]
    public class Player
    {
        [Key]
        public Guid PlayerKey { get; set; }
        
        public int Level { get; set; }
        
        public int MaxHP { get; set; }
        
        public int Strength { get; set; }
        
        public int Dexterity { get; set; }
        
        public int Constitution { get; set; }
        
        public int Intelligence { get; set; }
        
        public int Wisdom { get; set; }
        
        public int Charisma { get; set; }

        public string Abilities { get; set; }
        
        public string Status { get; set; }
    }
}
