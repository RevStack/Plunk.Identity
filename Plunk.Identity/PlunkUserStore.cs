using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using RevStack.Pattern;
using RevStack.Mvc;
using RevStack.Identity;
using RevStack.Plunk;
using RevStack.Plunk.Identity.Repositories;

namespace RevStack.Plunk.Identity
{
    public class PlunkUserStore<TUser, TUserLogin, TUserClaim, TUserRole, TRole, TKey> : IIdentityUserStore<TUser, TKey>
        where TUser : class, IIdentityUser<TKey>
        where TUserLogin : class, IIdentityUserLogin<TKey>
        where TUserClaim : class, IIdentityUserClaim<TKey>
        where TUserRole : class, IIdentityUserRole<TKey>
        where TRole : class, IIdentityRole
    {
        #region "Private Fields"
        private const string DUPLICATE_USER_MSG = "Error: Cannot Create User.Username already in use";
        private readonly UserRepository<TUser, TKey> _userRepository;
        private readonly IRepository<TUserLogin, TKey> _userLoginRepository;
        private readonly IRepository<TUserClaim, TKey> _userClaimRepository;
        private readonly UserRoleRepository<TUserRole, TKey> _userRoleRepository;
        private readonly RoleRepository<TRole> _roleRepository;
        private readonly Func<TUserLogin> _identityUserLoginFactory;
        private readonly Func<TUserClaim> _identityUserClaimFactory;
        private readonly Func<TUserRole> _identityUserRoleFactory;

        #endregion

        #region "Constructor"
        public PlunkUserStore(PlunkDataContext context,
            IRepository<TUserLogin, TKey> userLoginRepository,
            IRepository<TUserClaim, TKey> userClaimRepository,
            Func<TUserLogin> identityUserLoginFactory,
            Func<TUserClaim> identityUserClaimFactory,
            Func<TUserRole> identityUserRoleFactory

            )
        {
            _userRepository = new UserRepository<TUser, TKey>(context);
            _userLoginRepository = userLoginRepository;
            _userClaimRepository = userClaimRepository;
            _userRoleRepository = new UserRoleRepository<TUserRole, TKey>(context);
            _roleRepository = new RoleRepository<TRole>(context);
            _identityUserLoginFactory = identityUserLoginFactory;
            _identityUserClaimFactory = identityUserClaimFactory;
            _identityUserRoleFactory = identityUserRoleFactory;
        }
        #endregion

        #region "Public Members"
        public virtual Task CreateAsync(TUser user)
        {
            _create(user);
            return Task.FromResult(true);
        }

        public virtual Task DeleteAsync(TUser user)
        {
            _delete(user);
            return Task.FromResult(true);
        }

        public virtual Task<TUser> FindByIdAsync(TKey userId)
        {
            return Task.FromResult(_findById(userId));
        }

        public virtual Task<TUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(_findByName(userName));
        }

        public virtual Task UpdateAsync(TUser user)
        {
            _update(user);
            return Task.FromResult(true);
        }

        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            _addLogin(user, login);
            return Task.FromResult(true);
        }

        public virtual Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            _removeLogin(user, login);
            return Task.FromResult(true);
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return Task.FromResult(_getLogins(user));
        }

        public virtual Task<TUser> FindAsync(UserLoginInfo login)
        {
            return Task.FromResult(_find(login));
        }

        public virtual Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return Task.FromResult(_getClaims(user));
        }

        public virtual Task AddClaimAsync(TUser user, Claim claim)
        {
            _addClaim(user, claim);
            return Task.FromResult(true);
        }

        public virtual Task RemoveClaimAsync(TUser user, Claim claim)
        {
            _removeClaim(user, claim);
            return Task.FromResult(true);
        }

        public virtual Task AddToRoleAsync(TUser user, string roleName)
        {
            _addToRole(user, roleName);
            return Task.FromResult(true);
        }

        public virtual Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            _removeFromRole(user, roleName);
            return Task.FromResult(true);
        }

        public virtual Task<IList<string>> GetRolesAsync(TUser user)
        {
            return Task.FromResult(_getRoles(user));
        }

        public virtual Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return Task.FromResult(_isInRole(user, roleName));
        }

        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            _setPasswordHash(user, passwordHash);
            return Task.FromResult(true);
        }

        public virtual Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult(_getPasswordHash(user));
        }

        public virtual Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(_hasPassword(user));
        }

        public virtual Task SetSecurityStampAsync(TUser user, string stamp)
        {
            _setSecurityStamp(user, stamp);
            return Task.FromResult(true);
        }

        public virtual Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(_getSecurityStamp(user));
        }

        public virtual Task SetEmailAsync(TUser user, string email)
        {
            _setEmail(user, email);
            return Task.FromResult(true);
        }

        public virtual Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(_getEmail(user));
        }

        public virtual Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(_getEmailConfirmed(user));
        }

        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            _setEmailConfirmed(user, confirmed);
            return Task.FromResult(true);
        }

        public virtual Task<TUser> FindByEmailAsync(string email)
        {
            return Task.FromResult(_findByEmail(email));
        }

        public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            _setPhoneNumber(user, phoneNumber);
            return Task.FromResult(true);
        }

        public virtual Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(_getPhoneNumber(user));
        }

        public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult(_getPhoneNumberConfirmed(user));
        }

        public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            _setPhoneNumberConfirmed(user, confirmed);
            return Task.FromResult(true);
        }

        public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return Task.FromResult(_getLockoutEndDate(user));
        }

        public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            _setLockoutEndDate(user, lockoutEnd);
            return Task.FromResult(true);
        }

        public virtual Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(_incrementAccessFailedCount(user));
        }

        public virtual Task ResetAccessFailedCountAsync(TUser user)
        {
            _resetAccessFailedCount(user);
            return Task.FromResult(true);
        }

        public virtual Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(_getAccessFailedCount(user));
        }

        public virtual Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(_getLockoutEnabled(user));
        }

        public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            _setLockoutEnabled(user, enabled);
            return Task.FromResult(true);
        }

        public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            _setTwoFactorEnabled(user, enabled);
            return Task.FromResult(true);
        }

        public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(_getTwoFactorEnabled(user));
        }

        #endregion

        #region "Private Members"
        private void _create(TUser user)
        {
            _userRepository.Add(user);
        }

        private void _delete(TUser user)
        {
            _userRepository.Delete(user);
        }

        private TUser _findById(TKey userId)
        {
            return (TUser)_userRepository.Get().Where(x => x.Compare(x.Id, userId)).ToSingleOrDefault();
        }

        private TUser _findByName(string userName)
        {
            return (TUser)_userRepository.Get().Where(x => x.UserName == userName).ToSingleOrDefault();
        }

        private void _update(TUser user)
        {
            _userRepository.Update(user);
        }

        private void _addLogin(TUser user, UserLoginInfo login)
        {
            var identityUserLogin = _identityUserLoginFactory();
            identityUserLogin.Id = user.Id;
            identityUserLogin.LoginProvider = login.LoginProvider;
            identityUserLogin.ProviderKey = login.ProviderKey;

            _userLoginRepository.Add(identityUserLogin);

        }

        private void _removeLogin(TUser user, UserLoginInfo login)
        {
            var identityLogin = _userLoginRepository.Find(x => x.Compare(x.UserId, user.Id) && x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey).ToSingleOrDefault();
            if (identityLogin != null)
            {
                _userLoginRepository.Delete(identityLogin);
            }
        }

        private IList<UserLoginInfo> _getLogins(TUser user)
        {
            var logins = _userLoginRepository.Find(x => x.Compare(x.UserId, user.Id));
            if (logins.Any())
            {
                return logins.Select(x => new UserLoginInfo(x.LoginProvider, x.LoginProvider)).ToList();
            }
            else
            {
                return new List<UserLoginInfo>();
            }
        }

        private TUser _find(UserLoginInfo login)
        {
            TUser user = null;
            var userLogin = _userLoginRepository.Find(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey).ToSingleOrDefault();
            if (userLogin != null)
            {
                user = (TUser)_userRepository.Get().Where(x => x.Compare(x.Id, userLogin.UserId)).ToSingleOrDefault();
            }
            return user;
        }

        private IList<Claim> _getClaims(TUser user)
        {
            var userClaims = _userClaimRepository.Find(x => x.Compare(x.UserId, user.Id));
            if (userClaims.Any())
            {
                return userClaims.Select(x => new Claim(x.ClaimType, x.ClaimType)).ToList();
            }
            else
            {
                return new List<Claim>();
            }
        }

        private void _addClaim(TUser user, Claim claim)
        {
            var identityUserClaim = _identityUserClaimFactory();
            identityUserClaim.UserId = user.Id;
            identityUserClaim.ClaimType = claim.Type;
            identityUserClaim.ClaimValue = claim.Value;

            _userClaimRepository.Add(identityUserClaim);
        }

        private void _removeClaim(TUser user, Claim claim)
        {
            var userClaims = _userClaimRepository.Find(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value && x.Compare(x.UserId, user.Id));
            if (userClaims.Any())
            {
                var userClaim = userClaims.ToSingleOrDefault();
                _userClaimRepository.Delete(userClaim);
            }
        }

        private void _addToRole(TUser user, string roleName)
        {
            var role = _roleRepository.Get().Where(x => x.Name == roleName).ToSingleOrDefault();
            if (role != null)
            {
                _userRoleRepository.AddUserToRole(user.UserName, roleName);
            }
        }

        private void _removeFromRole(TUser user, string roleName)
        {
            var role = _roleRepository.Get().Where(x => x.Name == roleName).ToSingleOrDefault();
            if (role != null)
            {
                _userRoleRepository.RemoveUserFromRole(user.UserName, roleName);
            }
        }

        private IList<string> _getRoles(TUser user)
        {
            return _userRoleRepository.FindByUserId(user.UserName);
        }

        private List<string> _listOfRoles(IQueryable<IIdentityUserRole<TKey>> roles)
        {
            return roles
              .Join(_roleRepository.Get(), x => x.RoleId, r => r.Id, (x, r) => new { x, r })
              .Select(result => result.r.Name).ToList();
        }

        private bool _isInRole(TUser user, string roleName)
        {
            var roles = _userRoleRepository.FindByUserId(user.UserName);
            if (roles.Any())
            {
                return roles.Select(x => x == roleName).SingleOrDefault();
            }
            else
            {
                return false;
            }
        }

        private void _setPasswordHash(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            if (_userExists(user.UserName)) _userRepository.Update(user);
        }

        private string _getPasswordHash(TUser user)
        {
            return user.PasswordHash;
        }

        private bool _hasPassword(TUser user)
        {
            return (user.PasswordHash != null);
        }

        private void _setSecurityStamp(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            if (_userExists(user.UserName)) _userRepository.Update(user);
        }

        private string _getSecurityStamp(TUser user)
        {
            return user.SecurityStamp;
        }

        private void _setEmail(TUser user, string email)
        {
            user.Email = email;
            if (_userExists(user.UserName)) _userRepository.Update(user);
        }

        private string _getEmail(TUser user)
        {
            return user.Email;
        }

        private bool _getEmailConfirmed(TUser user)
        {
            return user.EmailConfirmed;
        }

        private void _setEmailConfirmed(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            _userRepository.Update(user);
        }

        private TUser _findByEmail(string email)
        {
            return (TUser)_userRepository.Get().Where(x => x.Email.ToLower() == email.ToLower()).ToSingleOrDefault();
            ;
        }

        private void _setPhoneNumber(TUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            _userRepository.Update(user);
        }

        private string _getPhoneNumber(TUser user)
        {
            return user.PhoneNumber;
        }

        private bool _getPhoneNumberConfirmed(TUser user)
        {
            return user.PhoneNumberConfirmed;
        }

        private void _setPhoneNumberConfirmed(TUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            _userRepository.Update(user);
        }

        private DateTimeOffset _getLockoutEndDate(TUser user)
        {
            if (user.LockoutEndDate == null) return new DateTimeOffset(DateTime.Now.AddDays(-1));
            return user.LockoutEndDate.Value;
        }

        private void _setLockoutEndDate(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDate = lockoutEnd;
            _userRepository.Update(user);
        }

        private int _incrementAccessFailedCount(TUser user)
        {
            int count = user.AccessFailedCount++;
            _userRepository.Update(user);
            return count;
        }

        private void _resetAccessFailedCount(TUser user)
        {
            user.AccessFailedCount = 0;
            _userRepository.Update(user);
        }

        private int _getAccessFailedCount(TUser user)
        {
            return user.AccessFailedCount;
        }

        private bool _getLockoutEnabled(TUser user)
        {
            return user.IsLockoutEnabled;
        }

        private void _setLockoutEnabled(TUser user, bool enabled)
        {
            user.IsLockoutEnabled = enabled;
            _userRepository.Update(user);
        }

        private void _setTwoFactorEnabled(TUser user, bool enabled)
        {
            user.IsTwoFactorEnabled = enabled;
            _userRepository.Update(user);
        }

        private bool _getTwoFactorEnabled(TUser user)
        {
            return user.IsTwoFactorEnabled;
        }

        private bool _userExists(string userName)
        {
            var _existingUser = _findByName(userName);
            return (_existingUser != null);

        }
        #endregion


        #region IDisposable Support
        private void disposeWork()
        {

        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    disposeWork();
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
