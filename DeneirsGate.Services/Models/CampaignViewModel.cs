using System;
using System.Collections.Generic;

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
        public List<PlayerShortViewModel> Players { get; set; }
    }
}
