using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeneirsGate.Services
{
    public class MonsterViewModel
    {
        public Guid MonsterKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Alignment { get; set; }
        public List<string> Environments { get; set; }
        public string Speed { get; set; }
        public string ChallengeRating { get; set; }
        public int Difficulty { get; set; }
        public int XP { get; set; }
    }

    public class MonsterEditModel
    {
        [NotEmptyGuid]
        public Guid MonsterKey { get; set; }
        public Guid CampaignKey { get; set; }
        [Required(ErrorMessage = "Name is required!"), StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid Size { get; set; }
        public Guid Type { get; set; }
        public string Alignment { get; set; }
        public List<Guid> Environments { get; set; } = new List<Guid>();
        [Required(ErrorMessage = "Speed is required!"), StringLength(50)]
        public string Speed { get; set; }
        public Guid ChallengeRating { get; set; }
    }

    public class MonsterPostModel : MonsterEditModel
    {
        //public bool IsPublic { get; set; }
    }

    public class MonsterSizeViewModel
    {
        public Guid SizeKey { get; set; }
        public string Name { get; set; }
    }

    public class MonsterTypeViewModel
    {
        public Guid TypeKey { get; set; }
        public string Name { get; set; }
    }

    public class MonsterChallengeRatingViewModel
    {
        public Guid RatingKey { get; set; }
        public string Challenge { get; set; }
        public int Proficiency { get; set; }
        public int XP { get; set; }
        public int Difficulty { get; set; }
    }
}
