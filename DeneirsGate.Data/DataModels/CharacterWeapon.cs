using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("CharacterWeapons")]
    public class CharacterWeapon
    {
        [Key]
        public Guid WeaponKey { get; set; }
        public Guid CharacterKey { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public int AttackMod { get; set; }
        [StringLength(10)]
        public string DamageDice { get; set; }
        public int DamageMod { get; set; }
        public Guid DamageType { get; set; }
    }
}
