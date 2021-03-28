using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class SettlementService : DeneirsService
    {
        public void UserHasSettlementAccess(Guid userId, Guid contentKey)
        {
            DBReset();

            //Check if exists
            if (DB.Settlements.FirstOrDefault(x => x.SettlementKey == contentKey) == null) { return; }

            //Check campaign
            var campaignKeys = DB.UserCampaigns.Where(x => x.UserKey == userId && x.IsOwner).Select(x => x.CampaignKey).ToList();
            if (DB.Settlements.FirstOrDefault(x => campaignKeys.Contains(x.CampaignKey) && x.SettlementKey == contentKey) != null)
            {
                return;
            }

            throw new Exception("You do not have access to this content!");
        }

        public List<SettlementViewModel> GetSettlements(Guid userId, Guid campaignId)
        {
            UserHasAccess(userId, campaignId);

            DBReset();

            var settlements = DB.Settlements.Where(x => x.CampaignKey == campaignId).Select(x => new SettlementViewModel
            {
                Description = x.Description,
                Map = x.Map,
                Name = x.Name,
                SettlementKey = x.SettlementKey
            }).OrderBy(x => x.Name).ToList();

            return settlements;
        }

        public SettlementViewModel GetSettlement(Guid userId, Guid campaignId, Guid settlementId)
        {
            UserHasAccess(userId, campaignId);
            UserHasSettlementAccess(userId, settlementId);

            var settlement = CreateSettlement(campaignId, settlementId);
            DBReset();

            if (settlement == null)
            {
                settlement = DB.Settlements.Where(x => x.SettlementKey == settlementId).Select(x => new SettlementViewModel
                {
                    Description = x.Description,
                    Map = x.Map,
                    Name = x.Name,
                    SettlementKey = x.SettlementKey,
                    SettlementLocations = x.SettlementLocations.Select(y => new SettlementLocationViewModel
                    {
                        Description = y.Description,
                        LocationKey = y.LocationKey,
                        Name = y.Name,
                        SettlementKey = y.SettlementKey,
                        SortOrder = y.SortOrder,
                        X = y.X,
                        Y = y.Y
                    }).OrderBy(y => y.SortOrder).ToList()
                }).FirstOrDefault();
            }

            return settlement;
        }

        public SettlementViewModel CreateSettlement(Guid campaignId, Guid settlementId)
        {
            var exists = false;
            DBReset();

            if (DB.Settlements.FirstOrDefault(x => x.CampaignKey == campaignId && x.SettlementKey == settlementId) != null)
            {
                exists = true;
            }

            if (exists) { return null; }

            return new SettlementViewModel
            {
                SettlementKey = settlementId,
                SettlementLocations = new List<SettlementLocationViewModel>()
            };
        }

        public void UpdateSettlement(Guid userId, Guid campaignId, SettlementViewModel model)
        {
            UserHasAccess(userId, campaignId);
            UserHasSettlementAccess(userId, model.SettlementKey);

            DBReset();

            var add = false;
            var settlement = DB.Settlements.FirstOrDefault(x => x.SettlementKey == model.SettlementKey);

            if (settlement == null)
            {
                settlement = new Settlement();
                settlement.SettlementKey = model.SettlementKey;
                settlement.CampaignKey = campaignId;
                add = true;
            }

            settlement.Description = model.Description;
            settlement.Map = model.Map;
            settlement.Name = model.Name;

            //Locations
            DB.SettlementLocations.RemoveRange(x => x.SettlementKey == model.SettlementKey);
            foreach (var item in model.SettlementLocations)
            {
                DB.SettlementLocations.Add(new SettlementLocation
                {
                    Description = item.Description,
                    LocationKey = Guid.NewGuid(),
                    Name = item.Name,
                    SettlementKey = model.SettlementKey,
                    SortOrder = item.SortOrder,
                    X = item.X,
                    Y = item.Y
                });
            }

            if (add)
            {
                DB.Settlements.Add(settlement);
            }

            DB.SaveChanges();
        }

        public void DeleteSettlement(Guid userId, Guid campaignId, Guid settlementId)
        {
            UserHasAccess(userId, campaignId);
            UserHasSettlementAccess(userId, settlementId);

            DBReset();
            DB.Settlements.RemoveRange(x => x.SettlementKey == settlementId);
            DB.SettlementLocations.RemoveRange(x => x.SettlementKey == settlementId);

            DB.SaveChanges();
        }
    }
}
