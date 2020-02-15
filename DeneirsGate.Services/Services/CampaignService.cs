using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class CampaignService
    {
        public List<CampaignViewModel> GetCampaigns(Guid userId, bool isOwner)
        {
            var campaigns = new List<CampaignViewModel>();

            using (var db = new DataEntities())
            {
                var keys = db.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner == isOwner).Select(x => x.CampaignKey).ToList();
                campaigns = db.Campaigns.Where(x => keys.Contains(x.CampaignKey)).Select(x => new CampaignViewModel
                {
                    CampaignKey = x.CampaignKey,
                    Description = x.Description,
                    Name = x.Name,
                    Portrait = x.Portrait,
                    LastUpdated = x.LastUpdated
                }).OrderByDescending(x => x.LastUpdated).ToList();
            }

            return campaigns;
        }

        public void GetCampaignDashboard(Guid userId, Guid campaignId)
        {
            //TODO
        }
    }
}
