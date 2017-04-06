﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using ISS.Authentication.Data.Interfaces.DbContext;
using ISS.Authentication.Domain.Models;
using System.Data;
using Dapper;

namespace ISS.Authentication.Data.Repositories
{
    public class UserRepository : Interfaces.Repositories.IUserRepository
    {
        private readonly IDbContext CurrentContext;

        public UserRepository(IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("connectionString");

            this.CurrentContext = context;
        }

        public void Dispose()
        {

        }

        public virtual Task<List<User>> ListAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<User>("select Id, PasswordHash, SecurityStamp, [Email], EmailConfirmed, FirstName, LastName, FailedLoginCount, Locked, LockedUntil from auth_Users", new { }).AsList();
            });
        }

        public virtual Task<List<User>> ListWithoutPasswordAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<User>("select Id, PasswordHash, SecurityStamp, [Email], EmailConfirmed, FirstName, LastName, FailedLoginCount, Locked, LockedUntil from auth_Users WHERE ISNULL(PasswordHash,'')=''", new { }).AsList();
            });
        }

        public virtual Task<List<User>> ListUsersInRoleAsync(string roleName)
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<User>("select Id, PasswordHash, SecurityStamp, [Email], EmailConfirmed, FirstName, LastName, FailedLoginCount, Locked, LockedUntil from auth_Users U INNER JOIN (auth_UserRoles UR INNER JOIN auth_Roles R ON UR.RoleID=R.Id) ON UR.UserID=U.Id WHERE LCASE(R.RoleName)=LCASE(@RoleName)", new { RoleName = roleName }).AsList();
            });
        }

        public virtual Task<List<Role>> AvailableRolesForUserAsync(User user)
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<Role>("select R.Id, R.RoleName, R.Description from auth_Roles R LEFT JOIN auth_UserRoles UR ON UR.RoleId=R.Id AND UR.UserId=@UserId WHERE UR.Id IS NULL ORDER BY R.RoleName", new { UserId = user.Id }).AsList();
            });
        }

        public virtual Task<List<Role>> AssignedRolesForUserAsync(User user)
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<Role>("select R.Id, R.RoleName, R.Description from auth_Roles R INNER JOIN auth_UserRoles UR ON UR.RoleId=R.Id AND UR.UserId=@UserId ORDER BY R.RoleName", new { UserId = user.Id }).AsList();
            });
        }

        #region IUserStore
        public virtual Task CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.Factory.StartNew(() =>
            {
                user.Id = Guid.NewGuid();
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("insert into auth_Users(Id, PasswordHash, SecurityStamp, [Email], EmailConfirmed, FirstName, LastName, FailedLoginCount, Locked, LockedUntil) values(@Id, @PasswordHash, @SecurityStamp, @Email, @EmailConfirmed, @FirstName, @LastName, @FailedLoginCount, @Locked, @LockedUntil)", new { Id = user.Id, PasswordHash=user.PasswordHash, SecurityStamp=user.SecurityStamp, Email=user.Email, EmailConfirmed=user.EmailConfirmed, FirstName=user.FirstName, LastName=user.LastName, FailedLoginCount=user.FailedLoginCount, Locked=user.Locked, LockedUntil=(user.LockedUntil.HasValue ? user.LockedUntil.Value.ToString("dd-MM-yyyy HH:mm:ss") : "") });
            });
        }

        public virtual Task DeleteAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("delete from auth_Users where Id = @Id", new { user.Id });
            });
        }

        public virtual Task<User> FindByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<User>("select Id, PasswordHash, SecurityStamp, [Email], EmailConfirmed, FirstName, LastName, FailedLoginCount, Locked, LockedUntil from auth_Users where Id = @Id", new { Id = userId }).SingleOrDefault();
            });
        }

        public virtual Task<User> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException("userName");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<User>("select Id, PasswordHash, SecurityStamp, [Email], EmailConfirmed, FirstName, LastName, FailedLoginCount, Locked, LockedUntil from auth_Users where LCASE(Email) = LCASE(@Email)", new { Email=userName }).SingleOrDefault();
            });
        }

        public virtual Task UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("update auth_Users set PasswordHash=@PasswordHash, SecurityStamp=@SecurityStamp, [Email]=@Email, EmailConfirmed=@EmailConfirmed, FirstName=@FirstName, LastName=@LastName, FailedLoginCount=@FailedLoginCount, Locked=@Locked, LockedUntil=@LockedUntil where Id = @Id", new { PasswordHash=user.PasswordHash, SecurityStamp=user.SecurityStamp, Email=user.Email, EmailConfirmed=user.EmailConfirmed, FirstName=user.FirstName, LastName=user.LastName, FailedLoginCount=user.FailedLoginCount, Locked=user.Locked, LockedUntil= (user.LockedUntil.HasValue ? user.LockedUntil.Value.ToString("dd-MM-yyyy HH:mm:ss") : ""), Id=user.Id });
            });
        }
        #endregion

        #region IUserLoginStore
        public virtual Task AddLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("insert into auth_Logins(Id, UserId, LoginProvider, ProviderKey) values(@Id, @userId, @loginProvider, @providerKey)",
                        new { Id = Guid.NewGuid(), userId = user.Id, loginProvider = login.LoginProvider, providerKey = login.ProviderKey });
            });
        }

        public virtual Task<User> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<User>("select u.* from auth_Users u inner join auth_Logins l on l.UserId = u.Id where l.LoginProvider = @loginProvider and l.ProviderKey = @providerKey", new { loginProvider=login.LoginProvider, providerKey=login.ProviderKey }).SingleOrDefault();
            });
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return (IList<UserLoginInfo>)connection.Query<UserLoginInfo>("select LoginProvider, ProviderKey from auth_Logins where UserId = @userId", new { userId = user.Id }).ToList();
            });
        }

        public virtual Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("delete from auth_Logins where UserId = @userId and LoginProvider = @loginProvider and ProviderKey = @providerKey",
                        new { userId = user.Id, login.LoginProvider, login.ProviderKey });
            });
        }
        #endregion

        #region IUserPasswordStore
        public virtual Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        public virtual Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public virtual Task SetPasswordHashAsync(User user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        #endregion

        #region IUserSecurityStampStore
        public virtual Task<string> GetSecurityStampAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.SecurityStamp);
        }

        public virtual Task SetSecurityStampAsync(User user, string stamp)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        #endregion

        #region IUserRoleStore

        public async Task AddToRoleAsync(User user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            Role _role = null;
            await Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    _role = connection.Query<Role>("select * FROM auth_Roles WHERE RoleName LIKE @roleName", new { roleName = roleName }).FirstOrDefault<Role>();
            });
            if (_role != null)
            {
                if (await IsInRoleAsync(user, roleName) == false)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        using (IDbConnection connection = CurrentContext.OpenConnection())
                            connection.Execute("INSERT INTO auth_UserRoles(UserId, RoleId) VALUES(@userId, @roleId)", new { Id = Guid.NewGuid(), userId = user.Id, roleId = _role.Id });
                    });
                }
            }
            return;
        }

        public virtual Task<System.Collections.Generic.IList<string>> GetRolesAsync(User user)
        {
            return GetRolesAsync(user.Id);
        }

        public virtual Task<System.Collections.Generic.IList<string>> GetRolesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return (IList<string>)connection.Query<string>("select R.RoleName from auth_UserRoles UR INNER JOIN auth_Roles R ON UR.RoleId=R.Id WHERE UR.UserId=@Id", new { Id = userId }).ToList<string>();
            });
        }

        public Task<bool> IsInRoleAsync(User user, string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException("roleName");

            List<Guid> _userRoles = null;

            using (IDbConnection connection = CurrentContext.OpenConnection())
                _userRoles = connection.Query<Guid>("select UR.Id from auth_UserRoles UR INNER JOIN auth_Roles R ON UR.RoleId=R.Id WHERE UR.UserId=@userId AND R.RoleName LIKE @roleName", new { userId = user.Id, roleName = roleName }).ToList<Guid>();
            bool _return = false;
            if (_userRoles.Count > 0)
            {
                _return = true;
            }
            return Task.Factory.StartNew(() =>
            {
                return _return;
            });
        }

        public Task RemoveFromRoleAsync(User user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");


            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("delete from auth_UserRoles where UserId = @Id AND RoleId IN (SELECT Id FROM auth_Roles WHERE RoleName LIKE @roleName)", new { Id = user.Id, roleName = roleName });
            });
        }

        #endregion
        public Task<User> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("email");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<User>("select Id, PasswordHash, SecurityStamp, [Email], EmailConfirmed, FirstName, LastName, FailedLoginCount, Locked, LockedUntil from auth_Users where LCASE([Email]) = LCASE(@Email)", new { email }).FirstOrDefault();
            });
        }

        public Task<string> GetEmailAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailAsync(User user, string email)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }
    }
}
