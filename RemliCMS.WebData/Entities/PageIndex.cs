using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.WebData.Entities
{
    [BsonIgnoreExtraElements]
    public class PageIndex : MongoEntity
    {
        public PageIndex()
        {
            PageContents = new List<PageContent>();
        }

        [BsonElement("pageheader_id")]
        public ObjectId PageHeaderId { get; set; }

        [BsonElement("createdDate")]
        [DisplayName("Date Created")]
        public DateTime CreatedDate { get; set; }

        [BsonElement("order")]
        [DisplayName("Sort Order")]
        public int Order { get; set; }

        [BsonElement("width")]
        [DisplayName("Content Width")]
        public int Width { get; set; }

        [BsonElement("rowbreak")]
        [DisplayName("Row Break")]
        public bool RowBreak { get; set; }

        [BsonElement("pageContent")]
        [DisplayName("Contents")]
        public List<PageContent> PageContents { get; set; } 
    }
}