using MongoDB.Bson;

namespace RemliCMS.WebData.Entities
{
    public interface IMongoEntity
    {
        ObjectId Id { get; set; }
    }
}
