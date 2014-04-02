
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.RegSystem.Services
{
    public class RegFieldService : EntityService<RegField>
    {
        public List<RegField> ListAll()
        {
            // returns a list of all Translations, including active and none active ones.
            //var regFieldQuery = Query<RegField>.EQ("");
            var regFieldList = MongoConnectionHandler.MongoCollection.FindAll().ToList();

            return regFieldList;
        }

        public bool IsExistKey(string submitKey)
        {
            // checks and see if Field Key exists.
            var regFieldQuery = Query<RegField>.EQ(g => g.Key, submitKey.ToLower());
            var foundRegField = MongoConnectionHandler.MongoCollection.FindOne(regFieldQuery);

            return foundRegField != null;
        }

        public ObjectId FindRegFieldObjectId(string submitKey)
        {
            // returns RegFieldObject Id.
            var regFieldQuery = Query<RegField>.EQ(g => g.Key, submitKey.ToLower());
            var foundRegField = MongoConnectionHandler.MongoCollection.FindOne(regFieldQuery);

            if (foundRegField == null)
            {
                return ObjectId.Empty;
            }
            return foundRegField.Id;
        }

    }
}
