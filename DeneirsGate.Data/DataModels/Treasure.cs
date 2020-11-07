using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{
    [Table("Treasures")]
    public class Treasure
    {
        [Key]
        public Guid TreasureKey { get; set; }
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
        [Required]
        public int MinChallenge { get; set; }
        [Required]
        public int MaxChallenge { get; set; }
        [Required]
        public int Probability { get; set; }
    }
}
