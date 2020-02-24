using System;
using System.ComponentModel;

namespace DeneirsGate.Services
{
    public class CharacterViewModel
    {
        public Guid CharacterKey { get; set; }
        public int Level { get; set; } = 1;
        [DisplayName("Max HP")]
        public int MaxHP { get; set; } = 1;
        public int Strength { get; set; } = 10;
        public int Dexterity { get; set; } = 10;
        public int Constitution { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Wisdom { get; set; } = 10;
        public int Charisma { get; set; } = 10;
        public string Abilities { get; set; }
        public string Status { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Character Portrait")]
        public string Portrait { get; set; }
        [DisplayName("Race")]
        public Guid RaceKey { get; set; }
        [DisplayName("Class")]
        public Guid ClassKey { get; set; }
        [DisplayName("Background")]
        public Guid BackgroundKey { get; set; }
        public string Fears { get; set; }
        public string Ideals { get; set; }
        public string Backstory { get; set; }
        public Guid CampaignKey { get; set; }
        public string Languages { get; set; }
        public string Alignment { get; set; }
    }

    public class CharacterShortViewModel
    {
        public Guid CharacterKey { get; set; }
        public Guid CampaignKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Level { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public string Portrait { get; set; }
    }

    public class CharacterPostModel
    {
        [NotEmptyGuid]
        public Guid CharacterKey { get; set; }
        public int Level { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public string Abilities { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Portrait { get; set; }
        public Guid RaceKey { get; set; }
        public Guid ClassKey { get; set; }
        public Guid BackgroundKey { get; set; }
        public string Fears { get; set; }
        public string Ideals { get; set; }
        public string Backstory { get; set; }
        [NotEmptyGuid]
        public Guid CampaignKey { get; set; }
        public string Languages { get; set; }
        public string Alignment { get; set; }
    }

    public class PlayerViewModel : CharacterViewModel
    {
        public Guid UserKey { get; set; }
    }

    public class PlayerShortViewModel : CharacterShortViewModel
    {
        public Guid UserKey { get; set; }
    }

    public class PlayerPostModel : CharacterPostModel
    {
        public Guid UserKey { get; set; }
    }
}
