using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("SettlementLocations")]
    public class SettlementLocation
    {
        [Key]
        public Guid LocationKey { get; set; }
        public Guid SettlementKey { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int SortOrder { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
    }
}
