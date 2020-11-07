using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("TrapNatures")]
    public class TrapNature
    {
        [Key]
        public Guid NatureKey { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
    }
}
