using System;
using System.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.WebData.Entities
{
    [BsonIgnoreExtraElements]
    public class PageContent
    {
        [BsonElement("translation_id")]
        public ObjectId TranslationId { get; set; }

        [BsonElement("content")]
        [DisplayName("Content")]
        public string Content { get; set; }

        [BsonElement("isActive")]
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [BsonElement("createdDate")]
        [DisplayName("Date Modified")]
        public DateTime CreatedDate { get; set; }
    }
}
