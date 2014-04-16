using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ExtendedMongoMembership
{
    public class MembershipRole : IEquatable<MembershipRole>
    {
        public MembershipRole()
        {
            RoleId = Guid.NewGuid();
        }

        [BsonId]
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string LoweredRoleName { get; set; }
        public string Description { get; set; }

        public bool Equals(MembershipRole other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return RoleName.Equals(other.RoleName) && RoleId.Equals(other.RoleId);
        }

        public override int GetHashCode()
        {
            int hashRoleName = RoleName == null ? 0 : RoleName.GetHashCode();
            int hashRoleId = RoleId.GetHashCode();
            return hashRoleName ^ hashRoleId;
        }
    }
}
