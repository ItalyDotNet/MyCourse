using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyCourse.Models.Entities;

namespace MyCourse.Models.Services.Infrastructure
{
    public class AdoNetUserStore :
        IUserStore<ApplicationUser>,
        IUserRoleStore<ApplicationUser>,
        IUserClaimStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>,
        IUserPhoneNumberStore<ApplicationUser>,
        IUserSecurityStampStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>,
        IUserTwoFactorRecoveryCodeStore<ApplicationUser>,
        IUserAuthenticatorKeyStore<ApplicationUser>,
        IUserAuthenticationTokenStore<ApplicationUser>,
        IUserLockoutStore<ApplicationUser>,
        IUserLoginStore<ApplicationUser>,
        IUserConfirmation<ApplicationUser>
    {
        private readonly IDatabaseAccessor db;
        public AdoNetUserStore(IDatabaseAccessor db)
        {
            this.db = db;
        }

        #region Implementation of IUserStore<ApplicationUser>
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount, FullName) VALUES ({user.Id}, {user.UserName}, {user.NormalizedUserName}, {user.Email}, {user.NormalizedEmail}, {user.EmailConfirmed}, {user.PasswordHash}, {user.SecurityStamp}, {user.ConcurrencyStamp}, {user.PhoneNumber}, {user.PhoneNumberConfirmed}, {user.TwoFactorEnabled}, {user.LockoutEnd}, {user.LockoutEnabled}, {user.AccessFailedCount}, {user.FullName})", token);
            if (affectedRows > 0)
            {
                return IdentityResult.Success;
            }
            var error = new IdentityError { Description = "Could not insert user" };
            return IdentityResult.Failed(error);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"UPDATE AspNetUsers SET UserName={user.UserName}, NormalizedUserName={user.NormalizedUserName}, Email={user.Email}, NormalizedEmail={user.NormalizedEmail}, EmailConfirmed={user.EmailConfirmed}, PasswordHash={user.PasswordHash}, SecurityStamp={user.SecurityStamp}, ConcurrencyStamp={user.ConcurrencyStamp}, PhoneNumber={user.PhoneNumber}, PhoneNumberConfirmed={user.PhoneNumberConfirmed}, TwoFactorEnabled={user.TwoFactorEnabled}, LockoutEnd={user.LockoutEnd}, LockoutEnabled={user.LockoutEnabled}, AccessFailedCount={user.AccessFailedCount}, FullName={user.FullName} WHERE Id={user.Id}", token);
            if (affectedRows > 0)
            {
                return IdentityResult.Success;
            }
            var error = new IdentityError { Description = "Could not update user" };
            return IdentityResult.Failed(error);
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"DELETE FROM AspNetUsers WHERE Id={user.Id}", token);
            if (affectedRows > 0)
            {
                return IdentityResult.Success;
            }
            var error = new IdentityError { Description = "User could not be found" };
            return IdentityResult.Failed(error);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUsers WHERE Id={userId}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ApplicationUser.FromDataRow(dataSet.Tables[0].Rows[0]);
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUsers WHERE NormalizedUserName={normalizedUserName}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ApplicationUser.FromDataRow(dataSet.Tables[0].Rows[0]);
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken token)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken token)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
        #endregion

        #region Implementation of IUserClaimStore<ApplicationUser>
        public async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUserClaims WHERE UserId={user.Id}", token);
            List<Claim> claims = dataSet.Tables[0].AsEnumerable().Select(row => new Claim(
                type: Convert.ToString(row["ClaimType"]),
                value: Convert.ToString(row["ClaimValue"])
            )).ToList();
            return claims;
        }
        public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken token)
        {
            foreach (Claim claim in claims)
            {
                int affectedRows = await db.CommandAsync($"INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue) VALUES ({user.Id}, {claim.Type}, {claim.Value})", token);
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException("Couldn't add the claim");
                }
            }
        }

        public async Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken token)
        {
            await RemoveClaimsAsync(user, new[] { claim }, token);
            await AddClaimsAsync(user, new[] { newClaim }, token);
        }

        public async Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken token)
        {
            foreach (Claim claim in claims)
            {
                int affectedRows = await db.CommandAsync($"DELETE FROM AspNetUserClaims WHERE UserId={user.Id} AND ClaimType={claim.Type} AND ClaimValue={claim.Value}", token);
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException("Couldn't remove the claim");
                }
            }
        }

        public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUserClaims WHERE ClaimType={claim.Type} AND ClaimValue={claim.Value}", token);
            List<ApplicationUser> users = dataSet.Tables[0].AsEnumerable().Select(row => ApplicationUser.FromDataRow(row)).ToList();
            return users;
        }
        #endregion

        #region Implementation of IUserEmailStore<ApplicationUser>

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUsers WHERE NormalizedEmail={normalizedEmail}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ApplicationUser.FromDataRow(dataSet.Tables[0].Rows[0]);
        }
        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken token)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken token)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken token)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
        #endregion

        #region Implementation of IUserPasswordStore<ApplicationUser>
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken token)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken token)
        {
            bool hasPassword = user.PasswordHash != null;
            return Task.FromResult(hasPassword);
        }

        #endregion

        #region Implementation of IUserPhoneNumberStore<Application>
        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken token)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken token)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }
        #endregion

        #region Implementation of IUserSecurityStampStore<ApplicationUser>
        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken token)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.SecurityStamp);
        }
        #endregion

        #region Implementation of IUserTwoFactorStore<ApplicationUser>
        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken token)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
        #endregion

        const string loginProviderName = "[AspNetUserStore]";
        const string authenticatorKeyTokenName = "AuthenticatorKey";
        const string recoveryCodesTokenName = "RecoveryCodes";


        #region Implementation of IUserAuthenticatorKeyStore<ApplicationUser>
        public Task SetAuthenticatorKeyAsync(ApplicationUser user, string key, CancellationToken token)
        {
            return SetTokenAsync(user, loginProviderName, authenticatorKeyTokenName, key, token);
        }

        public Task<string> GetAuthenticatorKeyAsync(ApplicationUser user, CancellationToken token)
        {
            return GetTokenAsync(user, loginProviderName, authenticatorKeyTokenName, token);
        }
        #endregion

        #region Implementation of IUserTwoFactorRecoveryCodeStore<ApplicationUser>
        public Task ReplaceCodesAsync(ApplicationUser user, IEnumerable<string> recoveryCodes, CancellationToken token)
        {
            var codesValue = string.Join(";", recoveryCodes);
            return SetTokenAsync(user, loginProviderName, recoveryCodesTokenName, codesValue, token);
        }

        public async Task<bool> RedeemCodeAsync(ApplicationUser user, string code, CancellationToken token)
        {
            string codesValue = await GetTokenAsync(user, loginProviderName, recoveryCodesTokenName, token);
            if (string.IsNullOrEmpty(codesValue))
            {
                return false;
            }
            List<string> codes = codesValue.Split(';').ToList();
            if (!codes.Remove(code))
            {
                return false;
            }
            await ReplaceCodesAsync(user, codes, token);
            return true;
        }

        public async Task<int> CountCodesAsync(ApplicationUser user, CancellationToken token)
        {
            string codesValue = await GetTokenAsync(user, loginProviderName, recoveryCodesTokenName, token);
            if (string.IsNullOrEmpty(codesValue))
            {
                return 0;
            }
            string[] codes = codesValue.Split(';');
            return codes.Length;
        }
        #endregion

        #region Implementation of IUserAuthenticationTokenStore<ApplicationUser>
        public async Task SetTokenAsync(ApplicationUser user, string loginProvider, string name, string value, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"REPLACE INTO AspNetUserTokens (UserId, LoginProvider, Name, Value) VALUES ({user.Id}, {loginProvider}, {name}, {value})", token);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException($"Couldn't set token '{name}' for login provider '{loginProvider}");
            }
        }

        public async Task RemoveTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"DELETE FROM AspNetUserTokens WHERE UserId={user.Id} AND LoginProvider={loginProvider} AND Name={name}", token);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException($"Couldn't remove token '{name}' for login provider '{loginProvider}'");
            }
        }

        public Task<string> GetTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken token)
        {
            return db.QueryScalarAsync<string>($"SELECT Value FROM AspNetUserTokens WHERE UserId={user.Id} AND LoginProvider={loginProvider} AND Name={name}", token);
        }
        #endregion

        #region Implementation of IUserLoginStore<ApplicationUser>

        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"INSERT INTO AspNetUserLogins (UserId, LoginProvider, ProviderKey, ProviderDisplayName) VALUES ({user.Id}, {login.LoginProvider}, {login.ProviderKey}, {login.ProviderDisplayName})", token);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("Couldn't add a login");
            }
        }

        public async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"DELETE FROM AspNetUserLogins WHERE UserId={user.Id} AND LoginProvider={loginProvider} AND ProviderKey={providerKey}", token);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("Couldn't remove a login");
            }
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT * FROM AspNetUserLogins WHERE UserId={user.Id}", token);
            List<UserLoginInfo> userLogins = dataSet.Tables[0].AsEnumerable().Select(row => new UserLoginInfo(
                providerKey: Convert.ToString(row["ProviderKey"]),
                loginProvider: Convert.ToString(row["LoginProvider"]),
                displayName: Convert.ToString(row["ProviderDisplayName"])
            )).ToList();
            return userLogins;
        }

        public async Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT AspNetUsers.* FROM AspNetUsers LEFT JOIN AspNetUserLogins ON AspNetUsers.Id=AspNetUserLogins.UserId WHERE LoginProvider={loginProvider} AND ProviderKey={providerKey}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ApplicationUser.FromDataRow(dataSet.Tables[0].Rows[0]);
        }
        #endregion

        #region Implementation of IUserLockoutStore<ApplicationUser>

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken token)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken token)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken token)
        {
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken token)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken token)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        #endregion

        #region Implementation of IUserConfirmation<ApplicationUser>
        public Task<bool> IsConfirmedAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }
        #endregion

        #region Implementation of IUserRoleStore<ApplicationUser>
        public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ({user.Id}, (SELECT Id FROM AspNetRoles WHERE NormalizedName={roleName}))", token);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("Couldn't add to role");
            }
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"DELETE FROM AspNetUserRoles WHERE UserId={user.Id} AND RoleId=(SELECT Id FROM AspNetRoles WHERE NormalizedName={roleName})", token);
            if (affectedRows == 0)
            {
                throw new InvalidOperationException("Couldn't remove from role");
            }
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT AspNetRoles.Name FROM AspNetUsers INNER JOIN AspNetUserRoles ON AspNetUsers.Id=AspNetUserRoles.UserId INNER JOIN AspNetRoles ON AspNetUserRoles.RoleId=AspNetRoles.Id WHERE AspNetUsers.Id={user.Id}", token);
            return dataSet.Tables[0].AsEnumerable().Select(row => Convert.ToString(row["Name"])).ToList();
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT AspNetUsers.* FROM AspNetUsers INNER JOIN AspNetUserRoles ON AspNetUsers.Id=AspNetUserRoles.UserId INNER JOIN AspNetRoles ON AspNetUserRoles.RoleId=AspNetRoles.Id WHERE AspNetRoles.NormalizedName={roleName}", token);
            return dataSet.Tables[0].AsEnumerable().Select(row => ApplicationUser.FromDataRow(row)).ToList();
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken token)
        {
            return await db.QueryScalarAsync<bool>($"SELECT COUNT(*) FROM AspNetUsers INNER JOIN AspNetUserRoles ON AspNetUsers.Id=AspNetUserRoles.UserId INNER JOIN AspNetRoles ON AspNetUserRoles.RoleId=AspNetRoles.Id WHERE AspNetUsers.Id={user.Id} AND AspNetRoles.NormalizedName={roleName}", token);
        }
        #endregion
    }
}