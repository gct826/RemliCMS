using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.RegSystem.Entities
{
    public class RegHistory : MongoEntity
    {
        [BsonElement("regId")]
        [DisplayName("Registration Id")]
        public int RegId { get; set; }

        [BsonElement("event")]
        [DisplayName("Event")]
        public string Event { get; set; }

        [BsonElement("eventDetail")]
        [DisplayName("Event Detail")]
        public string EventDetail { get; set; }

        [BsonElement("eventTime")]
        [DisplayName("Event Time")]
        public DateTime EventTime { get; set; }

        [BsonElement("isAdmin")]
        [DisplayName("Admin Action")]
        public int IsAdmin { get; set; }
    }
}
