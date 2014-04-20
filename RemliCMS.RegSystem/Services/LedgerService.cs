using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.RegSystem.Services
{
    public class LedgerService : EntityService<Ledger>
    {
        public List<Ledger> GetLedgerList(int regId)
        {
            var ledgerQuery = Query<Ledger>.EQ(g => g.RegId, regId);

            var foundLedgerList = MongoConnectionHandler.MongoCollection.Find(ledgerQuery)
                .SetSortOrder(SortBy<Ledger>.Ascending(g => g.LedgerDate))
                .ToList();

            return foundLedgerList;
        }

        public List<Ledger> GetAllLedgerList()
        {
            // var ledgerQuery = Query<Ledger>.EQ(g => g.RegId, regId);

            var foundLedgerList = MongoConnectionHandler.MongoCollection.FindAll()
                .SetSortOrder(SortBy<Ledger>.Ascending(g => g.LedgerDate))
                .ToList();

            return foundLedgerList;
        }


        public decimal GetRemaining(int regId)
        {
            var ledgerQuery = Query<Ledger>.EQ(g => g.RegId, regId);

            var foundLedgerList = MongoConnectionHandler.MongoCollection.Find(ledgerQuery)
                .SetSortOrder(SortBy<Ledger>.Ascending(g => g.LedgerDate))
                .ToList();

            var totalRemaining = (decimal) 0;
            
            foreach (var item in foundLedgerList)
            {
                if (item.LedgerTypeId == 1)
                {
                    totalRemaining = totalRemaining + item.LedgerAmount;
                }
                else
                {
                    totalRemaining = totalRemaining - item.LedgerAmount;
                }
            }

            return totalRemaining;
        }
    }

}
