using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("MonsterEnvironmentLinkers")]
    public class MonsterEnvironmentLinker
    {
        [Key, Column(Order = 0)]
        public Guid MonsterKey { get; set; }

        [Key, Column(Order = 1)]
        public Guid EnvironmentKey { get; set; }
    }
}
