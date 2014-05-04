using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace RemliCMS.WebData.Entities
{
    public class ViewHistory : MongoEntity
    {
        [BsonElement("pagePermalink")]
        [DisplayName("Permalink")]
        public string PagePermalink { get; set; }

        [BsonElement("pageTranslation")]
        [DisplayName("Translation")]
        public string PageTranslation { get; set; }

        [BsonElement("pageEvent")]
        [DisplayName("Event")]
        public string PageEvent { get; set; }

        [BsonElement("eventTime")]
        [DisplayName("Event Time")]
        public DateTime EventTime { get; set; }
    }
}
