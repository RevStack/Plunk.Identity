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
    /// Class that represents the UserRoles table in the Database
    /// </summary>
    internal sealed class UserRoleRepository<TUserRole, TKey> where TUserRole : class, IIdentityUserRole<TKey>
    {
        private readonly PlunkDataContext _context;
        private const string ID = "id";

        /// <summary>
        /// Constructor that takes a DbManager instance 
        /// </summary>
        /// <param name="context"></param>
        public UserRoleRepository(PlunkDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public List<string> FindByUserId(string userId)
        {
            string limit = "-1";
            string page = "-1";
            string top = "-1";
            string fetch = "*:-1";
            List<string> list = new List<string>();

            try
            {
                JObject results = (JObject)_context.Client().V1AppidDatastoreQuerySqlGet("select roles.name from OUser where " + ID + " = '" + userId + "' or name = '" + userId + "'", _context.AppId, limit: limit, page: page, top: top, fetch: fetch);
                var array = results.Value<JArray>("data");
                if (array.Count() == 0)
                    return list;
               
                dynamic obj = array.FirstOrDefault();
                foreach (dynamic role in obj.roles)
                {
                    list.Add(role.Value);
                }
            }
            catch (Exception e) 
            {

            }
            return list;
        }

        
        public void RemoveUserFromRole(string userName, string roleName)
        {
            try
            {
                JObject obj = new JObject();
                obj["name"] = userName;
                obj["role"] = roleName;
                _context.Client().V1AppidAdminUserRemoveUserFromRolePut(_context.AppId, obj);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Inserts a new role for a user in the UserRoles table
        /// </summary>
        /// <param name="user">The User</param>
        /// <param name="roleId">The Role's id</param>
        /// <returns></returns>
        public void AddUserToRole(string userName, string roleName)
        {
            try
            {
                JObject obj = new JObject();
                obj["name"] = userName;
                obj["role"] = roleName;
                _context.Client().V1AppidAdminUserAddUserToRolePut(_context.AppId, obj);
            }
            catch (Exception e)
            {

            }
        }
    }
}
