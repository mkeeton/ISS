using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using ISS.Authentication.Data.Interfaces.DbContext;
using ISS.Authentication.Domain.Models;
using Dapper;

namespace ISS.Authentication.Data.Repositories
{
    public class RefreshTokenRepository : Interfaces.Repositories.IRefreshTokenRepository
    {
        private readonly IDbContext CurrentContext;

        public RefreshTokenRepository(IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("connectionString");

            this.CurrentContext = context;
        }

        public Task<List<RefreshToken>> ListAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    return connection.Query<RefreshToken>("select Id, UserId, IssuedUtc, ExpiresUtc, ProtectedTicket from auth_RefreshTokens", new { }).AsList();
                }
            });
        }

        public Task<RefreshToken> FindById(string tokenId)
        {
            if (tokenId == "")
                throw new ArgumentNullException("tokenId");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    return connection.Query<RefreshToken>("select Id, UserId, IssuedUtc, ExpiresUtc, ProtectedTicket from auth_RefreshTokens where Id = @Id", new { Id = tokenId }).SingleOrDefault();
                }
            });
        }

        public Task<RefreshToken> FindByTokenUser(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    return connection.Query<RefreshToken>("select Id, UserId, IssuedUtc, ExpiresUtc, ProtectedTicket from auth_RefreshTokens where UserId = @UserId", new { UserId = userId }).SingleOrDefault();
                }
            });
        }

        public virtual Task<bool> CreateAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null)
                throw new ArgumentNullException("refreshToken");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Execute("insert into auth_RefreshTokens(Id, UserId, IssuedUtc, ExpiresUtc, ProtectedTicket) values(@Id, @UserId, @IssuedUtc, @ExpiresUtc, @ProtectedTicket)", new { Id=refreshToken.Id, UserId=refreshToken.UserId, IssuedUtc= refreshToken.IssuedUtc.ToString("dd-MM-yyyy HH:mm:ss"), ExpiresUtc= refreshToken.ExpiresUtc.ToString("dd-MM-yyyy HH:mm:ss"), ProtectedTicket= refreshToken.ProtectedTicket }) > 0;
            });
        }

        public virtual Task<bool> DeleteAsync(RefreshToken refreshToken)
        {
            return DeleteAsync(refreshToken.Id);
        }

        public virtual Task<bool> DeleteAsync(string refreshTokenId)
        {
            if (refreshTokenId == "")
                throw new ArgumentNullException("refreshTokenId");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Execute("delete from auth_RefreshTokens where Id = @Id", new { Id = refreshTokenId }) > 0;
            });
        }

        public virtual Task<bool> ClearExpiredAsync()
        {

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Execute("delete from auth_RefreshTokens where ExpiresUtc < @CurrentTime", new { CurrentTime = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss") }) > 0;
            });
        }
    }
}
