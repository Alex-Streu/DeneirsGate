using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("ArtObjects")]
    public class ArtObject
    {
        [Key]
        public Guid ArtObjectKey { get; set; }
        [StringLength(150), Required]
        public string Name { get; set; }
        [Required]
        public int Value { get; set; }
    }
}
