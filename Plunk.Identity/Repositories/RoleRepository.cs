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
    /// Class that represents the Role table in the Database
    /// </summary>
    internal sealed class RoleRepository<TRole> where TRole : class, IIdentityRole
    {
        private readonly PlunkDataContext _context;
        private const string ID = "id";

        /// <summary>
        /// Constructor that takes a PlunkDataContext 
        /// </summary>
        /// <param name="context"></param>
        public RoleRepository(PlunkDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Deletes a role from the Roles table
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns></returns>
        public void Delete(TRole role)  
        {
            _context.Client().V1AppidRoleIdDelete(_context.AppId, role.Id);
        }

        /// <summary>
        /// Inserts a new Role in the Roles table
        /// </summary>
        /// <param name="roleName">The role's name</param>
        /// <returns></returns>
        public TRole Add(TRole role)
        {
            string obj = CamelCaseJsonSerializer.SerializeObject(role);
            JObject json = JObject.Parse(obj);
            json["@class"] = "ORole";
            JObject results = (JObject)_context.Client().V1AppidRolePost(_context.AppId, json);
            return (TRole)JsonConvert.DeserializeObject(results.ToString(), typeof(TRole));
        }

        /// <summary>
        /// Returns a role name given the roleId
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns>Role name</returns>
        public string GetRoleName(string roleName) 
        {
            TRole role = this.GetRoleByIdOrName(roleName);
            if (role == null)
                return null;
            return role.Name;
        }

        /// <summary>
        /// Returns the role Id given a role name
        /// </summary>
        /// <param name="roleName">Role's name</param>
        /// <returns>Role's Id</returns>
        public string GetRoleId(string roleName) 
        {
            TRole role = this.GetRoleByIdOrName(roleName);
            if (role == null)
                return null;
            return role.Id;
        }

        /// <summary>
        /// Gets the IdentityRole given the role Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public TRole GetRoleByIdOrName(string roleIdOrName) 
        {
            string limit = "-1";
            string page = "-1";
            string top = "-1";
            string fetch = "*:-1";

            JObject obj = (JObject)_context.Client().V1AppidDatastoreQuerySqlGet("select from ORole where " + ID + " = '" + roleIdOrName + "' or name = '" + roleIdOrName + "'", _context.AppId, limit: limit, page: page, top: top, fetch: fetch);
            var array = obj.Value<JArray>("data");
            if (array.Count() == 0)
                return default(TRole);

            return (TRole)JsonConvert.DeserializeObject(array[0].ToString(), typeof(TRole));
        }

        public IQueryable<TRole> Get() 
        {
            JArray array = (JArray)_context.Client().V1AppidRoleGet(_context.AppId);
            IEnumerable<TRole> roles = (IEnumerable<TRole>)JsonConvert.DeserializeObject(array.ToString(), typeof(IEnumerable<TRole>));
            return roles.AsQueryable<TRole>();
        }

        public IQueryable<TUser> GetUsersInRole<TUser>(string roleName) where TUser : RevStack.Identity.IIdentityUser<string>, new()
        {
            JArray array = (JArray)_context.Client().V1AppidRoleUsersIdGet(_context.AppId, roleName);
            IEnumerable<TUser> users = (IEnumerable<TUser>)JsonConvert.DeserializeObject(array.ToString(), typeof(IEnumerable<TUser>));
            return users.AsQueryable<TUser>();
        }

        public TRole Update(TRole role) 
        {
            string obj = CamelCaseJsonSerializer.SerializeObject(role);
            JObject json = JObject.Parse(obj);
            json["@class"] = "ORole";
            JObject results = (JObject)_context.Client().V1AppidRolePut(_context.AppId, json);
            return (TRole)JsonConvert.DeserializeObject(results.ToString(), typeof(TRole));
        }
    }
}
