using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("UserMonsters")]
    public class UserMonster
    {
        [Key, Column(Order = 1)]
        public Guid UserKey { get; set; }
        [Key, Column(Order = 2)]
        public Guid MonsterKey { get; set; }
        public bool IsPublic { get; set; }
    }
}
