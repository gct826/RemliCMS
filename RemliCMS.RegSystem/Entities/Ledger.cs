using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.RegSystem.Entities
{
    [BsonIgnoreExtraElements]
    public class Ledger: MongoEntity
    {
        [BsonElement("regId")]
        [DisplayName("Registration Id")]
        public int RegId { get; set; }

        [BsonElement("ledgerTypeId")]
        [DisplayName("Entry Type")]
        public int LedgerTypeId { get; set; }

        [BsonElement("ledgerAmount")]
        [DisplayName("Amount")]
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Currency)]
        public decimal LedgerAmount { get; set; }

        [BsonElement("ledgerDate")]
        [DisplayName("Date")]
        public DateTime LedgerDate { get; set; }

        [BsonElement("ledgerNote")]
        [DisplayName("Entry Note")]
        public string LedgerNote { get; set; }

        [BsonElement("isConfirmed")]
        [DisplayName("Confirmed")]
        public bool IsConfirmed { get; set; }

        [BsonElement("isCancelled")]
        [DisplayName("Cancelled")]
        public bool IsCancelled { get; set; }
    }
}
