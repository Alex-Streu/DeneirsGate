using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{

    [Table("Environments")]
    public class Environment
    {
        [Key]
        public Guid EnvironmentKey { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }
}
