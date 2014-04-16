using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedMongoMembership.Helpers
{
    internal class RoleComparer : IEqualityComparer<MembershipRole>
    {
        public bool Equals(MembershipRole x, MembershipRole y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.RoleName == y.RoleName && x.RoleId == y.RoleId && x.LoweredRoleName == y.LoweredRoleName;
        }

        public int GetHashCode(MembershipRole role)
        {
            if (Object.ReferenceEquals(role, null)) return 0;

            int hashRoleName = role.RoleName == null ? 0 : role.RoleName.GetHashCode();
            int hashRoleId = role.RoleId.GetHashCode();

            return hashRoleName ^ hashRoleId;
        }

    }
}
