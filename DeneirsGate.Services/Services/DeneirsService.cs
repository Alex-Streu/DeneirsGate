using DeneirsGate.Data;
using System;
using System.Linq;

namespace DeneirsGate.Services
{
    public class DeneirsService
    {
        private DataEntities db;

        protected DataEntities DB
        {
            get
            {
                if (db == null) { db = new DataEntities(); }
                return db;
            }
        }

        protected DataEntities DBReset()
        {
            db = new DataEntities();
            return db;
        }

        protected virtual void UserHasAccess(Guid userId, Guid campaignId)
        {
            var hasAccess = false;

            using (DBReset())
            {
                if (DB.UserCampaigns.FirstOrDefault(x => x.UserKey == userId && x.CampaignKey == campaignId && x.IsOwner) != null || DB.Campaigns.FirstOrDefault(x => x.CampaignKey == campaignId) == null)
                {
                    hasAccess = true;
                }
            }

            if (!hasAccess)
            {
                throw new Exception("You do not have access to this content!");
            }
        }
    }
}
