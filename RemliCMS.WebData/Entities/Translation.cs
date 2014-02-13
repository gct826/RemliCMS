using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.WebData.Entities
{
    [BsonIgnoreExtraElements]
    public class Translation : MongoEntity
    {
        [BsonElement("name")]
        [DisplayName("Translation Name")]
        [Required(ErrorMessage = "Translation Name is required")]
        public string Name { get; set; }

        [BsonElement("url")]
        [DisplayName("Url")]
        [RegularExpression(@"^[a-zA-Z-]+$", ErrorMessage = "Use letters only please")]
        [Required(ErrorMessage = "Translation Url is Required")]
        public string Url { get; set; }

        [BsonElement("code")]
        [DisplayName("Language Code")]
        [RegularExpression(@"^[a-zA-Z-]+$", ErrorMessage = "Use letters only please")]
        [Required(ErrorMessage = "Language Code is required.")]
        public string Code { get; set; }

        [BsonElement("isRtl")]
        [DisplayName("Right-to-Left Switch")]
        public bool IsRtl { get; set; }

        [BsonElement("isDefault")]
        [DisplayName("Default")]
        public bool IsDefault { get; set; }

        [BsonElement("isActive")]
        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}