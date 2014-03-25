using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.RegSystem.Entities
{
    [BsonIgnoreExtraElements]
    public class Registration : MongoEntity
    {
        [ScaffoldColumn(false)]
        [BsonElement("regId")]
        [DisplayName("Reg ID 注册号")]
        public int RegId { get; set; }

        [BsonElement("regEmail")]
        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("E-mail 電郵")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Email is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string RegEmail { get; set; }

        [BsonElement("regPhone")]
        [Required(ErrorMessage = "Phone is required")]
        [DisplayName("Phone 電話")]
        [RegularExpression(@"^\(?([2-9][0-8][0-9])\)?[- .●]?([2-9][0-9]{2})[- .●]?([0-9]{4})$", ErrorMessage = "Phone is not valid.")]
        [DataType(DataType.PhoneNumber)]
        public string RegPhone { get; set; }

        [ScaffoldColumn(false)]
        [BsonElement("isConfirmed")]
        public Boolean IsConfirmed { get; set; }

        [ScaffoldColumn(false)]
        [BsonElement("dateCreated")]
        public DateTime DateCreated { get; set; }
    }
}
