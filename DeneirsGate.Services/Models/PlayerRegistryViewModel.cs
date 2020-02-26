using System;

namespace DeneirsGate.Services
{
    public class PlayerRegistryViewModel
    {
        public string Name { get; set; }
        public Guid CharacterKey { get; set; }
        public Guid CampaignKey { get; set; }
    }

    public class PlayerRegistryPostModel
    {
        public Guid CharacterKey { get; set; }
    }
}
