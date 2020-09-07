using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("RelationshipTreeTiers")]
    public class RelationshipTreeTier
    {
        [Key]
        public Guid TierKey { get; set; }

        public Guid TreeKey { get; set; }

        public int SortOrder { get; set; }
    }
}
