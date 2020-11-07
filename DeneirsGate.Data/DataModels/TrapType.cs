using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("TrapTypes")]
    public class TrapType
    {
        [Key]
        public Guid TypeKey { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
        [StringLength(10), Required]
        public string SaveDC { get; set; }
        [StringLength(10), Required]
        public string AttackBonus { get; set; }
    }
}
