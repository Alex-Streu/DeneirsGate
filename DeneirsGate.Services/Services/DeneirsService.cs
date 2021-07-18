using DeneirsGate.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeneirsGate.Services
{
    public class DeneirsService
    {
        protected DataEntities db;

        //protected DataEntities DB
        //{
        //    get
        //    {
        //        if (db == null) { db = new DataEntities(); }
        //        return db;
        //    }
        //}

        //protected DataEntities DBReset()
        //{
        //    db = new DataEntities();
        //    return db;
        //}

        protected virtual void UserHasAccess(Guid userId, Guid campaignId)
        {
            var hasAccess = false;

            if (db.UserCampaigns.FirstOrDefault(x => x.UserKey == userId && x.CampaignKey == campaignId && x.IsOwner) != null || db.Campaigns.FirstOrDefault(x => x.CampaignKey == campaignId) == null)
            {
                hasAccess = true;
            }

            if (!hasAccess)
            {
                throw new Exception("You do not have access to this content!");
            }
        }
    }
}

namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }
    }
}

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static void Move<T>(this List<T> list, T item, int index)
        {
            list.Remove(item);
            list.Insert(index, item);
        }

        public static void MoveToTop<T>(this List<T> list, T item)
        {
            list.Move(item, 0);
        }

        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.Count == 0;
        }
    }
}
