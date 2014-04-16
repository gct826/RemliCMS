using ExtendedMongoMembership.Entities;
using System;
using System.Collections.Generic;

namespace ExtendedMongoMembership
{
    public class MembershipAccount : MembershipAccountBase
    {
        public MembershipAccount()
        {
            Roles = new List<MembershipRole>();
            OAuthData = new List<OAuthAccountDataEmbedded>();
        }

        public DateTime? CreateDate { get; set; }
        public string ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public string Password { get; set; }
        public DateTime? PasswordChangedDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        public List<MembershipRole> Roles { get; set; }

        public List<OAuthAccountDataEmbedded> OAuthData { get; set; }
    }
}
