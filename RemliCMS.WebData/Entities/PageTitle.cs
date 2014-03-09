using System;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.WebData.Entities
{
    [BsonIgnoreExtraElements]
    public class PageTitle
    {
        [BsonElement("translation_id")]
        public ObjectId TranslationId { get; set; }

        [BsonElement("title")]
        [DisplayName("Title")]
        public string Title { get; set; }

        [BsonElement("isActive")]
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [BsonElement("createdDate")]
        [DisplayName("Date Modified")]
        public DateTime CreatedDate { get; set; }
    }
}
