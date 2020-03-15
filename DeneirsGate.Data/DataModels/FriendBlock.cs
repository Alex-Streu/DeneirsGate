using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("FriendBlocks")]
    public class FriendBlock
    {
        [Key, Column(Order = 0)]
        public Guid UserKey { get; set; }

        [Key, Column(Order = 1)]
        public Guid BlockedUserKey { get; set; }
    }
}
