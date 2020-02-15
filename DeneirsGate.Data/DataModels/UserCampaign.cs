using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("UserCampaigns")]
    public class UserCampaign
    {
        [Key, Column(Order = 1)]
        public Guid CampaignKey { get; set; }

        [Key, Column(Order = 2)]
        public Guid UserKey { get; set; }

        [Key, Column(Order = 3)]
        public bool IsOwner { get; set; }
    }
}
