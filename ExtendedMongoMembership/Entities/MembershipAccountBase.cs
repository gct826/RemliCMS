
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace ExtendedMongoMembership.Entities
{
    [BsonIgnoreExtraElements]
    public class MembershipAccountBase
    {
        [BsonId]
        public int UserId { get; set; }
        [BsonExtraElements]
        public BsonDocument CatchAll { get; set; }
        public string UserName { get; set; }
    }
}
