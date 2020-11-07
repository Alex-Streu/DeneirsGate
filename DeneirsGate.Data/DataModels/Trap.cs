using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Traps")]
    public class Trap
    {
        [Key]
        public Guid TrapKey { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
        [Required]
        public Guid Nature { get; set; }
        [Required]
        public Guid Type { get; set; }
        public string Description { get; set; }
    }
}
