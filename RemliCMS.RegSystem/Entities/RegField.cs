using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.RegSystem.Entities
{
    [BsonIgnoreExtraElements]
    public class RegField : MongoEntity
    {
        [BsonElement("name")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [BsonElement("key")]
        [DisplayName("Key")]
        public string Key { get; set; }
    }

}
