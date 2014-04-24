﻿using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.RegSystem.Services
{
    public class RegHistoryService : EntityService<RegHistory>
    {
        public void AddHistory(int regId, string eventEntry, string eventDetail, int isAdmin)
        {
            var newRegHistory = new RegHistory
                {
                    RegId = regId, 
                    Event = eventEntry,
                    EventDetail = eventDetail,
                    EventTime = DateTime.Now,
                    IsAdmin = isAdmin
                };

            Update(newRegHistory);
        }

        public List<RegHistory> ListRegHistory(int regId)
        {
            var regHistoryQuery = Query<RegHistory>.EQ(g => g.RegId, regId);

            var foundRegHistory = MongoConnectionHandler.MongoCollection.Find(regHistoryQuery)
                .SetSortOrder(SortBy<RegHistory>.Ascending(g => g.EventTime))
                .ToList();

            return foundRegHistory;
        }
    }
}