using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.Identity;
using RevStack.Identity;
using RevStack.Pattern;
using RevStack.Mvc;
using RevStack.Plunk;
using RevStack.Plunk.Identity.Repositories;

namespace RevStack.Plunk.Identity
{
    /// <summary>
    /// Class that implements the RevStack implementation of ASP.NET Identity role store interfaces
    /// </summary>
    public class PlunkRoleStore<TRole> : IQueryableRoleStore<TRole, string>, IIdentityRoleStore<TRole> where TRole : class, IIdentityRole
    {
        #region "Private Fields"
        private readonly RoleRepository<TRole> _roleRepository;
        #endregion

        #region "Constructor"
        public PlunkRoleStore(PlunkDataContext context)
        {
            _roleRepository = new RoleRepository<TRole>(context);
        }
        #endregion

        #region "Public Members"
        public IQueryable<TRole> Roles
        {
            get
            {
                return _roleRepository.Get();
            }
        }

        public Task CreateAsync(TRole role)
        {
            _create(role);
            return Task.FromResult(true);
        }

        public Task DeleteAsync(TRole role)
        {
            _delete(role);
            return Task.FromResult(true);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            return Task.FromResult(_findById(roleId));
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            return Task.FromResult(_findByName(roleName));
        }

        public Task UpdateAsync(TRole role)
        {
            _update(role);
            return Task.FromResult(true);
        }
        #endregion


        #region "Private Members"
        private void _create(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException("role");
            _roleRepository.Add(role);
        }

        private void _delete(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException("role");
            _roleRepository.Delete(role);
        }

        private TRole _findById(string roleId)
        {
            return (TRole)_roleRepository.GetRoleByIdOrName(roleId);
        }

        private TRole _findByName(string roleName)
        {
            return (TRole)_roleRepository.GetRoleByIdOrName(roleName);
        }

        private void _update(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException("role");
            _roleRepository.Update(role);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
