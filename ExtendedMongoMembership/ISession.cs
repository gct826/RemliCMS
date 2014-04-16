
using System;
namespace ExtendedMongoMembership
{
    public interface ISession : IDisposable
    {
        void Add<T>(System.Collections.Generic.IEnumerable<T> items) where T : class;
        void Add<T>(T item) where T : class;
        bool CreateUserRow(string userName, string UserNameColumn, string userTableName, System.Collections.Generic.IDictionary<string, object> values);
        void DeleteById(object id, string collectionName);
        void DeleteById(int id, string collectionName);
        void DeleteById(Guid id, string collectionName);
        void DeleteById(string id, string collectionName);
        void DeleteById<T>(int id) where T : class;
        void DeleteById<T>(Guid id) where T : class;
        void DeleteById<T>(string id) where T : class;
        void DeleteByQuery<T>(MongoDB.Driver.IMongoQuery query) where T : class;
        int GetUserId(string userTableName, string userNameColumn, string userName);
        System.Linq.IQueryable<OAuthToken> OAuthTokens { get; }
        System.Linq.IQueryable<MembershipRole> Roles { get; }
        void Save<T>(T item) where T : class;
        void Update<T>(T item) where T : class;
        System.Linq.IQueryable<MembershipAccount> Users { get; }
    }
}
