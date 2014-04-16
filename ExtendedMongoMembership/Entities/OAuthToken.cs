
using MongoDB.Bson.Serialization.Attributes;
namespace ExtendedMongoMembership
{
    public class OAuthToken
    {
        public OAuthToken()
        {
            Token = ShortGuid.NewGuid().ToString();
        }

        [BsonId]
        public string Token { get; set; }
        public string Secret { get; set; }
    }
}
