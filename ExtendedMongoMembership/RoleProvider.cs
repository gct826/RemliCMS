using ExtendedMongoMembership.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;

namespace ExtendedMongoMembership
{
    public class MongoRoleProvider : RoleProvider
    {
        private string _AppName;
        private string _connectionString;
        private bool _useAppHarbor;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name))
                name = "MongoDbRoleProvider";
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "EazyRole Provider for MongoDb");
            }
            base.Initialize(name, config);


            bool.TryParse(config["useAppHarbor"], out _useAppHarbor);
            //if (_useAppHarbor)
            //{
            //    _connectionString =
            //        ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
            //        ConfigurationManager.AppSettings.Get("MONGOLAB_URI");
            //}
            //else
            //{
                var dbUser = System.Configuration.ConfigurationManager.AppSettings["MongoDbUser"];
                var dbPwd = System.Configuration.ConfigurationManager.AppSettings["MongoDbPwd"];
                var dbLocation = System.Configuration.ConfigurationManager.AppSettings["MongoDbLocation"];
                var dbName = System.Configuration.ConfigurationManager.AppSettings["MongoDbName"];

                if (dbUser != "")
                {
                    dbLocation = "mongodb://" + dbUser + ":" + dbPwd + "@" + dbLocation;
                }
                else
                {
                    dbLocation = "mongodb://" + dbLocation;
                }

                _connectionString = dbLocation + "/" + dbName;
            //}

            if (string.IsNullOrEmpty(_connectionString))
            {
                string temp = config["connectionStringName"];
                if (string.IsNullOrEmpty(temp))
                    throw new ProviderException(StringResources.GetString(StringResources.Connection_name_not_specified));
                _connectionString = SecUtility.GetConnectionString(temp, true, true);

                if (string.IsNullOrEmpty(_connectionString))
                {
                    throw new ProviderException(StringResources.GetString(StringResources.Connection_string_not_found, temp));
                }
            }

            _AppName = config["applicationName"];

            if (string.IsNullOrEmpty(_AppName))
                _AppName = SecUtility.GetDefaultAppName();

            if (_AppName.Length > 256)
            {
                throw new ProviderException(StringResources.GetString(StringResources.Provider_application_name_too_long));
            }

            config.Remove("useAppHarbor");
            config.Remove("connectionStringName");
            config.Remove("applicationName");
            config.Remove("commandTimeout");

            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                    throw new ProviderException(StringResources.GetString(StringResources.Provider_unrecognized_attribute, attribUnrecognized));
            }
        }


        public override bool IsUserInRole(string username, string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            SecUtility.CheckParameter(ref username, true, false, true, 256, "username");
            if (username.Length < 1)
                return false;

            try
            {
                using (var session = new MongoSession(_connectionString))
                {

                    var user = (from u in session.Users
                                where u.UserName == username
                                select u).SingleOrDefault();

                    if (user == null)
                        return false;

                    if (user.Roles == null)
                        return false;

                    if (user.Roles.Where(r => r.RoleName == roleName).Any())
                        return true;

                    return false;
                }
            }
            catch
            {
                throw;
            }
        }


        public override string[] GetRolesForUser(string username)
        {
            SecUtility.CheckParameter(ref username, true, false, true, 256, "username");
            if (username.Length < 1)
                return new string[0];

            try
            {

                using (var session = new MongoSession(_connectionString))
                {

                    var user = (from u in session.Users
                                where u.UserName == username
                                select u).SingleOrDefault();

                    if (user == null)
                        return new string[0];

                    if (user.Roles == null)
                        return new string[0];

                    return user.Roles.Select(r => r.RoleName).ToArray();
                }
            }
            catch
            {
                throw;
            }
        }


        public override void CreateRole(string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            try
            {

                using (var session = new MongoSession(_connectionString))
                {

                    var roles = from r in session.Roles
                                where r.RoleName == roleName
                                select r;

                    if (roles.Any())
                    {
                        throw new ProviderException(StringResources.GetString(StringResources.Provider_role_already_exists, roleName));
                    }


                    var role = new MembershipRole
                                   {
                                       RoleName = roleName,
                                       LoweredRoleName = roleName.ToLowerInvariant()
                                   };

                    session.Add(role);
                }
            }
            catch
            {
                throw;
            }
        }


        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            try
            {

                using (var session = new MongoSession(_connectionString))
                {

                    var role = (from r in session.Roles
                                where r.RoleName == roleName
                                select r).SingleOrDefault();


                    var users = session.Users
                        .Where(u => u.Roles != null)
                        .Where(u => u.Roles.Any(r => r.RoleName == roleName));


                    if (users.Any() && throwOnPopulatedRole)
                    {
                        throw new ProviderException(StringResources.GetString(StringResources.Role_is_not_empty));
                    }

                    session.DeleteById<MembershipRole>(role.RoleId);

                    return true;
                }
            }
            catch
            {
                throw;
            }
        }


        public override bool RoleExists(string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");

            try
            {
                using (var session = new MongoSession(_connectionString))
                {

                    var role = (from r in session.Roles
                                where r.RoleName == roleName
                                select r).SingleOrDefault();

                    return (role != null);
                }
            }
            catch
            {
                throw;
            }
        }



        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            SecUtility.CheckArrayParameter(ref roleNames, true, true, true, 256, "roleNames");
            SecUtility.CheckArrayParameter(ref usernames, true, true, true, 256, "usernames");

            try
            {

                List<string> _usernames = usernames.ToList();
                List<string> _roleNames = roleNames.ToList();

                using (var session = new MongoSession(_connectionString))
                {

                    var users = (from u in session.Users
                                 where _usernames.Contains(u.UserName)
                                 select u).ToList();

                    var roles = (from r in session.Roles
                                 where _roleNames.Contains(r.RoleName)
                                 select r).ToList();

                    foreach (var userEntity in users)
                    {
                        if (userEntity.Roles.Any())
                        {
                            var newRoles = roles.Except(userEntity.Roles);
                            userEntity.Roles.AddRange(newRoles);
                        }
                        else
                        {
                            userEntity.Roles.AddRange(roles);
                        }

                        session.Save(userEntity);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            SecUtility.CheckArrayParameter(ref roleNames, true, true, true, 256, "roleNames");
            SecUtility.CheckArrayParameter(ref usernames, true, true, true, 256, "usernames");

            try
            {
                List<string> _usernames = usernames.ToList();
                List<string> _roleNames = roleNames.ToList();

                using (var session = new MongoSession(_connectionString))
                {

                    var users = (from u in session.Users
                                 where _usernames.Contains(u.UserName)
                                 select u).ToList();

                    var roles = (from r in session.Roles
                                 where _roleNames.Contains(r.RoleName)
                                 select r).ToList();

                    foreach (var userEntity in users)
                    {
                        if (userEntity.Roles.Any())
                        {
                            int oldCount = userEntity.Roles.Count;
                            var matchedRoles = roles.Intersect(userEntity.Roles, new RoleComparer());

                            foreach (var matchedRole in matchedRoles)
                                userEntity.Roles.Remove(matchedRole);

                            if (oldCount != userEntity.Roles.Count)
                                session.Save(userEntity);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }



        public override string[] GetUsersInRole(string roleName)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");

            try
            {

                using (var session = new MongoSession(_connectionString))
                {

                    var role = (from r in session.Roles
                                where r.RoleName == roleName
                                select r).SingleOrDefault();

                    if (role == null)
                    {
                        throw new ProviderException(StringResources.GetString(StringResources.Provider_role_not_found, roleName));
                    }

                    var users = from u in session.Users
                                where u.Roles.Any(r => r.RoleName == roleName)
                                select u;

                    if (users == null || !users.Any())
                        return new string[0];

                    return users.Select(u => u.UserName).ToArray();
                }
            }
            catch
            {
                throw;
            }
        }



        public override string[] GetAllRoles()
        {
            try
            {
                using (var session = new MongoSession(_connectionString))
                {

                    var roles = from r in session.Roles
                                select r;

                    if (roles == null || !roles.Any())
                        return new string[0];

                    return roles.Select(u => u.RoleName).ToArray();
                }
            }
            catch
            {
                throw;
            }
        }


        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            SecUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 256, "usernameToMatch");

            try
            {

                using (var session = new MongoSession(_connectionString))
                {

                    var role = (from r in session.Roles
                                where r.RoleName == roleName
                                select r).SingleOrDefault();

                    if (role == null)
                    {
                        throw new ProviderException(StringResources.GetString(StringResources.Provider_role_not_found, roleName));
                    }

                    var users = from u in session.Users
                                where u.UserName == usernameToMatch &&
                                u.Roles.Any(r => r.RoleName == roleName)
                                select u;

                    if (users == null || !users.Any())
                        return new string[0];

                    return users.Select(u => u.UserName).ToArray();
                }
            }
            catch
            {
                throw;
            }
        }

        public override string ApplicationName
        {
            get { return _AppName; }
            set
            {
                _AppName = value;

                if (_AppName.Length > 256)
                {
                    throw new ProviderException(StringResources.GetString(StringResources.Provider_application_name_too_long));
                }
            }
        }
    }

}
