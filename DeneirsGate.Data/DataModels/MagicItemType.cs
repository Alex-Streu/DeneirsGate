using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("MagicItemTypes")]
    public class MagicItemType
    {
        [Key]
        public Guid TypeKey { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
