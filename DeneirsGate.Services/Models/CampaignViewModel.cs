using System;

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
}
