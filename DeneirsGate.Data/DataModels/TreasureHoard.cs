using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("TreasureHoards")]
    public class TreasureHoard
    {
        [Key]
        public Guid TreasureHoardKey { get; set; }
        [StringLength(50)]
        public string CP { get; set; }
        [StringLength(50)]
        public string SP { get; set; }
        [StringLength(50)]
        public string EP { get; set; }
        [StringLength(50)]
        public string GP { get; set; }
        [StringLength(50)]
        public string PP { get; set; }
        [StringLength(50)]
        public string Gemstones { get; set; }
        [StringLength(50)]
        public string ArtObjects { get; set; }
        [Required]
        public int Value { get; set; }
        [Required]
        public int MinChallenge { get; set; }
        [Required]
        public int MaxChallenge { get; set; }
        [Required]
        public int Probability { get; set; }
    }
}
