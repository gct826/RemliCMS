using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.WebData.Entities;

namespace RemliCMS.WebData.Services
{
    public class ViewHistoryService : EntityService<ViewHistory>
    {
        public void AddHistory(string permalink, string translation, string eventNote)
        {
            var newViewHistory = new ViewHistory
            {
                PagePermalink = permalink,
                PageTranslation = translation,
                PageEvent = eventNote,
                EventTime = DateTime.Now.AddHours(-5),
            };

            Update(newViewHistory);
        }

        public List<ViewHistory> ListAll()
        {
            var foundViewHistory = MongoConnectionHandler.MongoCollection.FindAll()
                .SetSortOrder(SortBy<ViewHistory>.Descending(g => g.EventTime))
                .ToList();

            return foundViewHistory;
        }
    }
}
