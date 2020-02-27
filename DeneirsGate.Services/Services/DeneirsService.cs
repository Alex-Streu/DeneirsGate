﻿using DeneirsGate.Data;

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
    }
}
