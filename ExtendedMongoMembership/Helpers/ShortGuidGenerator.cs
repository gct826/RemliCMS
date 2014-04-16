
using MongoDB.Bson.Serialization;
namespace ExtendedMongoMembership.Helpers
{
    public class ShortGuidGenerator : IIdGenerator
    {
        private static ShortGuidGenerator __instance = new ShortGuidGenerator();

        public object GenerateId(object container, object document)
        {
            return ShortGuid.NewGuid().ToString();
        }

        public bool IsEmpty(object id)
        {
            if (id != null)
            {
                return string.IsNullOrEmpty((string)id);
            }
            return true;
        }

        public static ShortGuidGenerator Instance
        {
            get
            {
                return __instance;
            }
        }
    }
}
