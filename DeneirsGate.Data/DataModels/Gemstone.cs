using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Gemstones")]
    public class Gemstone
    {
        [Key]
        public Guid GemstoneKey { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
        [StringLength(100), Required]
        public string Description { get; set; }
        [Required]
        public int Value { get; set; }
    }
}
