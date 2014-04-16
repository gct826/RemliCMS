using RemliCMS.Models;
using ExtendedMongoMembership.Services;

namespace RemliCMS.Helpers
{
    public class DefaultUserProfileService : UserProfileServiceBase<SampleUserProfile>
    {
        public DefaultUserProfileService(string connectionString)
            : base(connectionString)
        {

        }
    }
}