using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.RegSystem.Entities
{
    [BsonIgnoreExtraElements]
    public class Participant : MongoEntity
    {
        [BsonElement("partId")]
        [ScaffoldColumn(false)]
        [DisplayName("Id 编号")]
        public int PartId { get; set; }

        [BsonElement("regId")]
        [ScaffoldColumn(false)]
        [DisplayName("Reg Id 注册号")]
        public int RegId { get; set; }

        [BsonElement("statusId")]
        [ScaffoldColumn(false)]
        [DisplayName("Status")]
        public int StatusId { get; set; }

        [BsonElement("sessionId")]
        [Required(ErrorMessage = "Required")]
        [DisplayName("Session 所屬會話")]
        public int SessionId { get; set; }

        [BsonElement("ageRangeId")]
        [Required(ErrorMessage = "Required")]
        [DisplayName("Age Range 年龄组")]
        public int AgeRangeId { get; set; }

        [BsonElement("genderId")]
        [Required(ErrorMessage = "Required")]
        [DisplayName("Gender 性別")]
        public int GenderId { get; set; }

        [BsonElement("roomTypeId")]
        [DisplayName("Rooming Preference 房型偏好")]
        [Required(ErrorMessage = "Required")]
        public int RoomTypeId { get; set; }

        [BsonElement("roomNote")]
        [DisplayName("Rooming Request 房型要求")]
        public string RoomNote { get; set; }

        [BsonElement("firstName")]
        [Required(ErrorMessage = "Required")]
        [DisplayName("First Name 名(英)")]
        [StringLength(20, ErrorMessage = "Too Long")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        [Required(ErrorMessage = "Required")]
        [DisplayName("Last Name 姓(英)")]
        [StringLength(20, ErrorMessage = "Too Long")]
        public string LastName { get; set; }

        [BsonElement("chineseName")]
        [DisplayName("Chinese Name 中文姓名")]
        [StringLength(20, ErrorMessage = "Too Long")]
        public string ChineseName { get; set; }

        [BsonElement("partPrice")]
        [DisplayName("Price 价钱")]
        public decimal PartPrice { get; set; }
    }
}
