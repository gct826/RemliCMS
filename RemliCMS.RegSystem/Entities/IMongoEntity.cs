using MongoDB.Bson;

namespace RemliCMS.RegSystem.Entities
{
    public interface IMongoEntity
    {
        ObjectId Id { get; set; }
    }
}
