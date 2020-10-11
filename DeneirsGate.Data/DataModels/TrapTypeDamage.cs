using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("TrapTypeDamages")]
    public class TrapTypeDamage
    {
        [Key]
        public Guid DamageKey { get; set; }
        [Required]
        public Guid TypeKey { get; set; }
        [Required]
        public int MinLevel { get; set; }
        [Required]
        public int MaxLevel { get; set; }
        [StringLength(10), Required]
        public string Damage { get; set; }
    }
}
