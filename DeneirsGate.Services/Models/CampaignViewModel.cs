using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static DeneirsGate.Services.CampaignService;

namespace DeneirsGate.Services
{
    public class CampaignViewModel
    {
        public Guid CampaignKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Portrait { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class CampaignDashboardViewModel
    {
        public Guid CampaignKey { get; set; }
        public ArcViewModel Arc { get; set; }
        public List<PlayerShortViewModel> Players { get; set; }
        public List<ArcCharacterViewModel> NPCs { get; set; }
    }

    public class ArcViewModel
    {
        public Guid ArcKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Map { get; set; }
        public List<ArcMapPinModel> Pins { get; set; }
        public bool IsActive { get; set; }
        public List<QuestViewModel> Quests { get; set; }
    }

    public class ArcMapPinModel
    {
        public Guid QuestKey { get; set; }
        public int Index { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class QuestViewModel
    {
        public Guid QuestKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public QuestStatus Status { get; set; }
        public List<QuestEventViewModel> Events { get; set; }
    }

    public class QuestEventViewModel
    {
        public Guid EventKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsComplete { get; set; }
        public EncounterViewModel Encounter { get; set; }
    }

    public class ArcActivePostModel
    {
        public Guid ArcKey { get; set; }
        public bool IsActive { get; set; }
    }

    public class ArcPostModel
    {
        [NotEmptyGuid]
        public Guid ArcKey { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Map { get; set; }
        public List<ArcMapPinModel> Pins { get; set; } = new List<ArcMapPinModel>();
        public bool IsActive { get; set; }
        public List<QuestPostModel> Quests { get; set; } = new List<QuestPostModel>();
    }

    public class QuestPostModel
    {
        public Guid QuestKey { get; set; }
        [Required, StringLength(150)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int SortOrder { get; set; }
        public QuestStatus Status { get; set; }
        public List<QuestEventPostModel> Events { get; set; } = new List<QuestEventPostModel>();
    }

    public class QuestEventPostModel
    {
        public Guid EventKey { get; set; } = Guid.NewGuid();
        [Required, StringLength(150)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int SortOrder { get; set; }
        public bool IsComplete { get; set; }
        public EncounterPostModel Encounter { get; set; }
    }

    public class ArcCharacterViewModel : CharacterShortViewModel
    {
        public bool IsSelected { get; set; }
    }

    public class ArcCharacterPostModel
    {
        public Guid ArcKey { get; set; }
        public Guid CharacterKey { get; set; }
        public bool Add { get; set; }
    }
}
