using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{

    [Table("MonsterTypes")]
    public class MonsterType
    {
        [Key]
        public Guid TypeKey { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }
}
