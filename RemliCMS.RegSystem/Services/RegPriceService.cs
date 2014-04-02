using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.RegSystem.Services
{
    public class RegPriceService : EntityService<RegPrice>
    {
        
        public decimal GetPrice(int roomTypeId, int ageRangeId)
        {
            // returns Price

            var regPriceQuery = Query.And(
                    Query<RegPrice>.EQ(g => g.AgeRangeId, ageRangeId),
                    Query<RegPrice>.EQ(g => g.RoomTypeId, roomTypeId)
                    );

            var foundRegPrice = MongoConnectionHandler.MongoCollection.Find(regPriceQuery).ToList();


            if (foundRegPrice.Count == 0)
            {
                return (decimal) 0.00;
            }

            return foundRegPrice.Last().Price;
        }

        public RegPrice GetRegPrice(int roomTypeId, int ageRangeId)
        {
            // returns Price

            var regPriceQuery = Query.And(
                    Query<RegPrice>.EQ(g => g.AgeRangeId, ageRangeId),
                    Query<RegPrice>.EQ(g => g.RoomTypeId, roomTypeId)
                    );

            var foundRegPrice = MongoConnectionHandler.MongoCollection.FindOne(regPriceQuery);

            return foundRegPrice;
        }

    }
}
