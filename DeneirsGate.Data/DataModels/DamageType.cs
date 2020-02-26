using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("DamageTypes")]
    public class DamageType
    {
        [Key]
        public Guid TypeKey { get; set; }
        public string Name { get; set; }
    }
}
