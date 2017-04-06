using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISS.Authentication.Domain.Models;
using Microsoft.AspNet.Identity;

namespace ISS.Authentication.Data.Interfaces.Repositories
{
    public interface IRoleRepository : IRoleStore<Role, Guid>
    {
        Task<List<Role>> ListAsync();
        Task<List<Role>> GetRolesForActionAndMethod(string currentAction, string currentMethod);
        Task<List<RoleApiPath>> ListRoleApiPathsAsync(Guid roleId);
        Task CreateRoleApiPathAsync(RoleApiPath rolePath);
        Task<RoleApiPath> FindRoleApiPathAsync(Guid roleId, string actionPath, string actionMethod);
        Task DeleteRoleApiPathAsync(Guid ApiPathId);
    }
}
