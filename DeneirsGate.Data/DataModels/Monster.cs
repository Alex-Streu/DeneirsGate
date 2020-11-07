using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{

    [Table("Monsters")]
    public class Monster
    {
        [Key]
        public Guid MonsterKey { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public Guid Size { get; set; }

        [Required]
        public Guid Type { get; set; }

        [Required]
        public string Alignment { get; set; }

        [Required, StringLength(50)]
        public string Speed { get; set; }

        [Required]
        public Guid ChallengeRating { get; set; }
    }
}
