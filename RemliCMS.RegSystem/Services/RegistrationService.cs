using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using RemliCMS.RegSystem.Entities;

namespace RemliCMS.RegSystem.Services
{
    public class RegistrationService : EntityService<Registration>
    {
        public bool AllowRegistration()
        {
            return true;
        }

        public bool IsExistEmail(string submittedEmail)
        {
            // checks whether Email exist and return false otherwise.
            var emailQuery = Query<Registration>.EQ(g => g.RegEmail, submittedEmail.ToLower());
            var foundEmail = MongoConnectionHandler.MongoCollection.FindOne(emailQuery);

            if (foundEmail != null)
            {
                return true;
            }
            return false;
        }

        public bool IsExistPhone(string submittedPhone)
        {
            // checks whether Phone exist and return false otherwise.
            var phoneQuery = Query<Registration>.EQ(g => g.RegPhone, submittedPhone);
            var foundPhone = MongoConnectionHandler.MongoCollection.FindOne(phoneQuery);

            if (foundPhone != null)
            {
                return true;
            }
            return false;
        }

        public int GetLastId()
        {
            var foundRegistration = MongoConnectionHandler.MongoCollection.FindAll()
                .SetSortOrder(SortBy<Registration>.Ascending(g => g.RegId))
                .LastOrDefault();

            if (foundRegistration == null)
            {
                return 0;
            }
            return foundRegistration.RegId;

        }
    }
}
