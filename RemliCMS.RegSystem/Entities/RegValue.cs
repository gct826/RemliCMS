using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.RegSystem.Entities
{

    [BsonIgnoreExtraElements]
    public class RegValue : MongoEntity
    {
        public RegValue()
        {
            Translation = new List<RegText>();
        }

        [BsonElement("value")]
        [DisplayName("Value")]
        public int Value { get; set; }

        [BsonElement("regFieldId")]
        [DisplayName("Reg Field Id")]
        public ObjectId RegFieldObjectId { get; set; }

        [BsonElement("translation")]
        [DisplayName("Translation")]
        public List<RegText> Translation { get; set; }

        [BsonElement("isActive")]
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        [BsonElement("isDeleted")]
        [DisplayName("Deleted")]
        public bool IsDeleted { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class RegText
    {
        [BsonElement("translation_id")]
        public ObjectId TranslationId { get; set; }

        [BsonElement("text")]
        [DisplayName("Text")]
        public string Text { get; set; }
    }

    public class ValueText
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
