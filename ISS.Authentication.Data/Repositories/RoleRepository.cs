using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISS.Authentication.Data.Interfaces.DbContext;
using ISS.Authentication.Domain.Models;
using System.Data;
using Dapper;

namespace ISS.Authentication.Data.Repositories
{
    public class RoleRepository : Interfaces.Repositories.IRoleRepository
    {

        private readonly IDbContext CurrentContext;

        public RoleRepository(IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("connectionString");

            this.CurrentContext = context;
        }

        public void Dispose()
        {

        }

        public virtual Task<List<Role>> ListAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<Role>("select Id, RoleName, Description from auth_Roles", new { }).AsList();
            });
        }

        public virtual Task CreateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");
            var owner = this.FindByNameAsync(role.Name);
            if ((owner == null) || (owner.Result == null))
            {
                return Task.Factory.StartNew(() =>
                {
                    role.Id = Guid.NewGuid();
                    using (IDbConnection connection = CurrentContext.OpenConnection())
                        connection.Execute("insert into auth_Roles(Id, RoleName) values(@Id, @Name)", new { Id=role.Id, Name=role.Name });
                });
            }
            else
            {
                role.Id = owner.Result.Id;
                return Task.FromResult(0);
            }
        }

        public virtual Task DeleteAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("delete from auth_Roles where Id = @Id", new { Id=role.Id });
            });
        }

        public virtual Task<Role> FindByIdAsync(Guid roleId)
        {
            if (roleId == Guid.Empty)
                throw new ArgumentNullException("roleId");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<Role>("select Id, RoleName, Description from auth_Roles where Id = @Id", new { Id = roleId }).SingleOrDefault();
            });
        }

        public virtual Task<Role> FindByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException("roleName");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<Role>("select Id, RoleName, Description from auth_Roles where LCASE(RoleName) = LCASE(@roleName)", new { roleName=roleName }).SingleOrDefault();
            });
        }

        public virtual Task UpdateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("update auth_Roles set RoleName = @Name where Id = @Id", new { Name=role.Name, Id=role.Id });
            });
        }

        public virtual Task<List<Role>> GetRolesForActionAndMethod(string currentAction, string currentMethod)
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<Role>("select R.Id, R.RoleName, R.Description from auth_Roles R INNER JOIN auth_RoleApiPaths RAP ON R.Id=RAP.RoleId WHERE RAP.ActionPath=@CurrentAction AND RAP.ActionMethod=@CurrentMethod", new { CurrentAction = currentAction, CurrentMethod = currentMethod }).AsList();
            });
        }

        public virtual Task<List<RoleApiPath>> ListRoleApiPathsAsync(Guid roleId)
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<RoleApiPath>("select Id, RoleId, ActionPath, ActionMethod from auth_RoleApiPaths WHERE RoleId=@RoleId", new { RoleId=roleId }).AsList();
            });
        }

        public virtual Task CreateRoleApiPathAsync(RoleApiPath rolePath)
        {
            if (rolePath == null)
                throw new ArgumentNullException("rolePath");
            var owner = this.FindRoleApiPathAsync(rolePath.RoleId, rolePath.ActionPath, rolePath.ActionMethod);
            if ((owner == null) || (owner.Result == null))
            {
                return Task.Factory.StartNew(() =>
                {
                    rolePath.Id = Guid.NewGuid();
                    using (IDbConnection connection = CurrentContext.OpenConnection())
                        connection.Execute("insert into auth_RoleApiPaths(Id, RoleId, ActionPath, ActionMethod) values(@Id, @RoleId, @ActionPath, @ActionMethod)", new { Id=rolePath.Id, RoleId=rolePath.RoleId, ActionPath=rolePath.ActionPath, ActionMethod=rolePath.ActionMethod });
                });
            }
            else
            {
                rolePath.Id = owner.Result.Id;
                return Task.FromResult(0);
            }
        }

        public virtual Task<RoleApiPath> FindRoleApiPathAsync(Guid roleId, string actionPath, string actionMethod)
        {
            //if (string.IsNullOrWhiteSpace(roleName))
            //  throw new ArgumentNullException("roleName");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<RoleApiPath>("select Id, RoleId, ActionPath, ActionMethod from auth_RoleApiPaths where RoleId=@RoleId AND LCASE(ActionPath)=LCASE(@ActionPath) AND LCASE(ActionMethod)=LCASE(@ActionMethod)", new { RoleId = roleId, ActionPath = actionPath, ActionMethod = actionMethod }).SingleOrDefault();
            });
        }

        public virtual Task DeleteRoleApiPathAsync(Guid ApiPathId)
        {
            if (ApiPathId == Guid.Empty)
                throw new ArgumentNullException("ApiPathId");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("delete from auth_RoleApiPaths where Id = @Id", new { Id = ApiPathId });
            });
        }
    }
}
