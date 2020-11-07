using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeneirsGate.Data
{

    [Table("MonsterChallengeRatings")]
    public class MonsterChallengeRating
    {
        [Key]
        public Guid RatingKey { get; set; }

        [StringLength(50)]
        public string Challenge { get; set; }

        [Required]
        public int Proficiency { get; set; }

        [Required]
        public int XP { get; set; }

        [Required]
        public int Difficulty { get; set; }
    }
}
