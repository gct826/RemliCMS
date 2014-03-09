using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.WebData.Entities
{
    [BsonIgnoreExtraElements]
    public class PageHeader : MongoEntity
    {
        public PageHeader()
        {
            PageTitles = new List<PageTitle>();
        }

        [BsonElement("name")]
        [DisplayName("Page Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [BsonElement("permalink")]
        [DisplayName("Permalink")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Required(ErrorMessage = "Permalink is required")]
        public string Permalink { get; set; }

        [BsonElement("isDefault")]
        [DisplayName("Default")]
        public bool IsDefault { get; set; }

        [BsonElement("parent_id")]
        [DisplayName("Parent")]
        public ObjectId ParentId { get; set; }

        [BsonElement("order")]
        [DisplayName("Menu Order")]
        public int Order { get; set; }

        [BsonElement("pageTitle")]
        [DisplayName("Title")]
        public List<PageTitle> PageTitles { get; set; }
    }
}