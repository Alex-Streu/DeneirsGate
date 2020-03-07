using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("FriendRequests")]
    public class FriendRequest
    {
        [Key, Column(Order = 0)]
        public Guid UserSenderKey { get; set; }

        [Key, Column(Order = 1)]
        public Guid UserReceiverKey { get; set; }

        public int Status { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
