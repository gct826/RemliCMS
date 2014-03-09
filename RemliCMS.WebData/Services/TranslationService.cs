using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using RemliCMS.WebData.Entities;

namespace RemliCMS.WebData.Services
{
    public class TranslationService : EntityService<Translation>
    {
        public string GetDefaultUrl()
        {
            var translationQuery = Query<Translation>.EQ(g => g.IsDefault, true);
            var foundTranslation = MongoConnectionHandler.MongoCollection.FindOne(translationQuery);

            if (foundTranslation == null)
            {
                return "";
            }

            return foundTranslation.Url;
        }

        public bool IsActiveUrl(string url)
        {
            // checks whether Translation is active and return false otherwise.
            var translationQuery = Query<Translation>.EQ(g => g.Url, url.ToLower());
            var foundTranslation = MongoConnectionHandler.MongoCollection.FindOne(translationQuery);

            if (foundTranslation != null)
            {
                return foundTranslation.IsActive;
            }
            return false;
        }

        public bool IsExistUrl(string url)
        {
            // checks to make sure Translation exist.
            var translationQuery = Query<Translation>.EQ(g => g.Url, url.ToLower());
            var foundTranslation = MongoConnectionHandler.MongoCollection.FindOne(translationQuery);

            return foundTranslation != null;
        }
        
        public List<Translation> ListAll()
        {
            // returns a list of all Translations, including active and none active ones.
            var translationQuery = Query<Translation>.EQ(g => g.IsDeleted, false);
            var translationList = MongoConnectionHandler.MongoCollection.Find(translationQuery).ToList();

            return translationList;
        }

        public Translation Details(string url)
        {
            // returns one specific Translation based on Code. 
            var translationQuery = Query<Translation>.EQ(g => g.Url, url.ToLower());
            var translation = MongoConnectionHandler.MongoCollection.FindOne(translationQuery);

            return translation;
        }


        public string GetName(ObjectId translationObjectId)
        {
            var translationQuery = Query<Translation>.EQ(g => g.Id, translationObjectId);
            var foundTranslation = MongoConnectionHandler.MongoCollection.FindOne(translationQuery);

            return foundTranslation.Name;
        }

    }
}
