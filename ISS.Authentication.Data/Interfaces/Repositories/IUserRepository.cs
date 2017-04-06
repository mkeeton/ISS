using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using ISS.Authentication.Domain.Models;

namespace ISS.Authentication.Data.Interfaces.Repositories
{
    public interface IUserRepository : IUserStore<User, Guid>, IUserLoginStore<User, Guid>, IUserPasswordStore<User, Guid>, IUserSecurityStampStore<User, Guid>, IUserEmailStore<User, Guid>, IUserRoleStore<User, Guid>
    {
        Task<List<User>> ListAsync();
        Task<List<User>> ListWithoutPasswordAsync();
        Task<List<User>> ListUsersInRoleAsync(string roleName);

        Task<List<Role>> AvailableRolesForUserAsync(User user);
        Task<List<Role>> AssignedRolesForUserAsync(User user);
    }
}
