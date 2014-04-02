using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.RegSystem.Entities
{
    [BsonIgnoreExtraElements]
    public class RegPrice : MongoEntity
    {
        [BsonElement("ageRangeId")]
        [DisplayName("Age Range")]
        public int AgeRangeId { get; set; }

        [BsonElement("roomTypeId")]
        [DisplayName("Registration")]
        public int RoomTypeId { get; set; }

        [BsonElement("price")]
        [DisplayName("Price")]
        public decimal Price { get; set; }
    }

}