using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{

    [Table("RelationshipTrees")]
    public class RelationshipTree
    {
        [Key]
        public Guid TreeKey { get; set; }

        public Guid CampaignKey { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }
}
