using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevStack.Identity;
using RevStack.Plunk;
using RevStack.Plunk.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RevStack.Plunk.Identity.Repositories
{
    /// <summary>
    /// Class that represents the Users table in the Database
    /// </summary>
    internal sealed class UserRepository<TUser, TKey> where TUser : class, IIdentityUser<TKey>
    {
        private readonly PlunkDataContext _context;
        private const string ID = "id";

        /// <summary>
        /// Constructor that takes a PlunkDataContext 
        /// </summary>
        /// <param name="context"></param>
        public UserRepository(PlunkDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the User's name given a User id
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public string GetUserName(string id) 
        {
            TUser user = this.GetUserById(id);
            if (user == null)
                return null;
            return user.UserName;
        }

        /// <summary>
        /// Returns a User ID given a User name
        /// </summary>
        /// <param name="userName">The User's name</param>
        /// <returns></returns>
        public TKey GetUserId(string userName)
        {
            List<TUser> users = this.GetUserByName(userName);
            if (users.Count() == 0)
                return default(TKey);
            return users.FirstOrDefault().Id;
        }

        /// <summary>
        /// Returns an TUser given the User's id
        /// </summary>
        /// <param name="memberId">The Member's id</param>
        /// <returns></returns>
        public TUser GetUserById(string idOrName) 
        {
            var query = "Select from OUser where " + ID + " = '" + idOrName + "' or name = '" + idOrName + "'";
            IQueryable<TUser> users = this.Get(query);
            if (users.Count() == 0)
                return null;
            return users.FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of TUser instances given a Member name
        /// </summary>
        /// <param name="userName">Member's name</param>
        /// <returns></returns>
        public List<TUser> GetUserByName(string userName) 
        {
            var query = "Select from OUser where name = '" + userName + "'";
            IQueryable<TUser> users = this.Get(query);
            if (users.Count() == 0)
                return null;
            return users.ToList();
        }

        public TUser GetUserByEmail(string email) 
        {
            var query = "Select from OUser where email = '" + email + "'";
            IQueryable<TUser> users = this.Get(query);
            if (users.Count() == 0)
                return null;
            return users.ToList().FirstOrDefault();
        }

        /// <summary>
        /// Return the Member's password hash
        /// </summary>
        /// <param name="memberId">The Member's id</param>
        /// <returns></returns>
        public string GetPasswordHash(string idOrName) 
        {
            var query = "Select from OUser where " + ID + " = '" + idOrName + "' or name = '" + idOrName + "'";
            IQueryable<TUser> users = this.Get(query);
            if (users.Count() == 0)
                return null;
            return users.FirstOrDefault().PasswordHash;
        }

        /// <summary>
        /// Sets the Member's password hash
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public void SetPasswordHash(string id, string passwordHash)
        {
            string cmd = "update OUser set PasswordHash = " + passwordHash + " where " + ID + " = '" + id + "' or name = '" + id + "'";
            _context.Client().V1AppidDatastoreCommandSqlGet(_context.AppId, cmd);
        }

        /// <summary>
        /// Returns the Member's security stamp
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public string GetSecurityStamp(string idOrName) 
        {
            var query = "Select from OUser where " + ID + " = '" + idOrName + "' or name = '" + idOrName + "'";
            IQueryable<TUser> users = this.Get(query);
            if (users.Count() == 0)
                return null;
            return users.FirstOrDefault().SecurityStamp;
        }

        /// <summary>
        /// Inserts a new Member in the Users table
        /// </summary>
        /// <param name="Member"></param>
        /// <returns></returns>
        public TUser Add(TUser user)
        {
            string obj = CamelCaseJsonSerializer.SerializeObject(user);
            JObject json = JObject.Parse(obj);
            json["@class"] = "OUser";
            JObject results = (JObject)_context.Client().V1AppidAdminUserPost(_context.AppId, json);
            return (TUser)JsonConvert.DeserializeObject(results.ToString(), typeof(TUser));
        }

        /// <summary>
        /// Deletes a Member from the Users table
        /// </summary>
        /// <param name="memberId">The Member's id</param>
        /// <returns></returns>
        private void Delete(string idOrName)
        {
            string cmd = "Delete from OUser where " + ID + " = '" + idOrName + "' or name = '" + idOrName + "'";
            _context.Client().V1AppidDatastoreCommandSqlGet(_context.AppId, cmd);
        }

        /// <summary>
        /// Deletes a Member from the Users table
        /// </summary>
        /// <param name="Member"></param>
        /// <returns></returns>
        public void Delete(TUser user) 
        {
            Delete(user.UserName);
        }

        /// <summary>
        /// Updates a Member in the Users table
        /// </summary>
        /// <param name="Member"></param>
        /// <returns></returns>
        public TUser Update(TUser user)
        {
            string obj = CamelCaseJsonSerializer.SerializeObject(user);
            JObject json = JObject.Parse(obj);
            json["@class"] = "OUser";
            JObject results = (JObject)_context.Client().V1AppidAdminUserPut(_context.AppId, json);
            return (TUser)JsonConvert.DeserializeObject(results.ToString(), typeof(TUser));
        }

        public IQueryable<TUser> Get() 
        {
            JArray array = (JArray)_context.Client().V1AppidAdminUserGet(_context.AppId);
            IEnumerable<TUser> users = (IEnumerable<TUser>)JsonConvert.DeserializeObject(array.ToString(), typeof(IEnumerable<TUser>));
            return users.AsQueryable<TUser>();
        }

        private IQueryable<TUser> Get(string query)
        {
            string limit = "-1";
            string page = "-1";
            string top = "-1";
            string fetch = "*:-1";
            JObject results = (JObject)_context.Client().V1AppidDatastoreQuerySqlGet(query, _context.AppId, limit: limit, page: page, top: top, fetch: fetch);
            var array = results.Value<JArray>("data");
            IEnumerable<TUser> users = (IEnumerable<TUser>)JsonConvert.DeserializeObject(array.ToString(), typeof(IEnumerable<TUser>));
            return users.AsQueryable<TUser>();
        }
    }
}
