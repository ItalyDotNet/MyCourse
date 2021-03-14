using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyCourse.Models.Services.Infrastructure
{
    public class AdoNetRoleStore :
        IRoleStore<IdentityRole>,
        IRoleClaimStore<IdentityRole>
    {
        private readonly IDatabaseAccessor db;
        public AdoNetRoleStore(IDatabaseAccessor db)
        {
            this.db = db;
        }


        #region Implementation of IRoleStore<IdentityRole>

        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ({role.Id}, {role.Name}, {role.NormalizedName}, {role.ConcurrencyStamp})", token);
            if (affectedRows > 0)
            {
                return IdentityResult.Success;
            }
            var error = new IdentityError { Description = "Could not insert role" };
            return IdentityResult.Failed(error);
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"UPDATE AspNetRoles SET Name={role.Name}, NormalizedName={role.NormalizedName}, ConcurrencyStamp={role.ConcurrencyStamp} WHERE Id={role.Id}", token);
            if (affectedRows > 0)
            {
                return IdentityResult.Success;
            }
            var error = new IdentityError { Description = "Could not update role" };
            return IdentityResult.Failed(error);
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken token)
        {
            int affectedRows = await db.CommandAsync($"DELETE FROM AspNetRoles WHERE Id={role.Id}", token);
            if (affectedRows > 0)
            {
                return IdentityResult.Success;
            }
            var error = new IdentityError { Description = "Role could not be found" };
            return IdentityResult.Failed(error);
        }

        public async Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT Id, Name, NormalizedName, ConcurrencyStamp FROM AspNetRoles WHERE Id={roleId}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            return IdentityRoleFromDataRow(dataSet.Tables[0].Rows[0]);
        }

        public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken token)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT Id, Name, NormalizedName, ConcurrencyStamp FROM AspNetRoles WHERE NormalizedName={normalizedRoleName}", token);
            if (dataSet.Tables[0].Rows.Count == 0)
            {
                return null;
            }

            return IdentityRoleFromDataRow(dataSet.Tables[0].Rows[0]);
        }

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken token)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken token)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken token)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken token)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken token)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }
        #endregion


        #region Implementation of IRoleClaimStore<IdentityRole>
        public async Task AddClaimAsync(IdentityRole role, Claim claim, CancellationToken token = default)
        {
            int affectedRows = await db.CommandAsync($"INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES ({role.Id}, {claim.Type}, {claim.Value})", token);
            if (affectedRows > 0)
            {
                return;
            }

            throw new InvalidOperationException("Could not insert claim");
        }
        
        public async Task RemoveClaimAsync(IdentityRole role, Claim claim, CancellationToken token = default)
        {
            int affectedRows = await db.CommandAsync($"DELETE FROM AspNetRoleClaims WHERE RoleId={role.Id} AND ClaimType={claim.Type} AND ClaimValue={claim.Value}", token);
            if (affectedRows > 0)
            {
                return;
            }

            throw new InvalidOperationException("Role claim could not be found");
        }

        public async Task<IList<Claim>> GetClaimsAsync(IdentityRole role, CancellationToken token = default)
        {
            DataSet dataSet = await db.QueryAsync($"SELECT ClaimType, ClaimValue FROM AspNetRoleClaims WHERE RoleId={role.Id}", token);
            return dataSet.Tables[0].AsEnumerable().Select(row => ClaimFromDataRow(row)).ToList();
        }

        public void Dispose()
        {
        }
        #endregion

        private static IdentityRole IdentityRoleFromDataRow(DataRow role)
        {
            return new IdentityRole()
            {
                Id = Convert.ToString(role["Id"]),
                Name = Convert.ToString(role["Name"]),
                NormalizedName = Convert.ToString(role["NormalizedName"]),
                ConcurrencyStamp = Convert.ToString(role["ConcurrencyStamp"])
            };
        }

        private static Claim ClaimFromDataRow(DataRow claim)
        {
            return new Claim(type: Convert.ToString(claim["ClaimType"]), value: Convert.ToString(claim["ClaimValue"]));
        }
    }
}