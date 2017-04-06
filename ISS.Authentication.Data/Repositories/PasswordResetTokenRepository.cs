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
    public class PasswordResetTokenRepository : Interfaces.Repositories.IPasswordResetTokenRepository
    {
        private readonly IDbContext CurrentContext;

        public PasswordResetTokenRepository(IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("connectionString");

            this.CurrentContext = context;
        }

        public Task<List<PasswordResetToken>> ListAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    return connection.Query<PasswordResetToken>("select Id, UserId, Token, Expires, Used from app_PasswordResetTokens", new { }).AsList();
                }
            });
        }

        public async Task<PasswordResetToken> CreateAsync(Guid userId, int expiryMinutes)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException("userId");

            string _token = "";
            System.Security.Cryptography.RandomNumberGenerator cryptoRNG = System.Security.Cryptography.RandomNumberGenerator.Create();
            Byte[] randomBytes = new Byte[40];
            while ((_token == "") || (await IsTokenUnique(_token) == false))
            {
                cryptoRNG.GetBytes(randomBytes);
                _token = Convert.ToBase64String(randomBytes);
            }

            PasswordResetToken _resetToken = new PasswordResetToken();

            _resetToken.Id = Guid.NewGuid();
            _resetToken.UserId = userId;
            _resetToken.Token = _token;
            _resetToken.Expires = DateTime.Now.AddMinutes(expiryMinutes);
            using (IDbConnection connection = CurrentContext.OpenConnection())
                connection.Execute("insert into app_PasswordResetTokens(Id, UserId, Token, Expires, Used) values(@Id, @UserId, @Token, @Expires, @Used)", new { Id=_resetToken.Id, UserId=_resetToken.UserId, Token=_resetToken.Token, Expires=_resetToken.Expires.ToString("dd-MM-yyyy HH:mm:ss"), Used=_resetToken.Used.Value.ToString("dd-MM-yyyy HH:mm:ss") });

            return _resetToken;
        }
        public Task<PasswordResetToken> FindByToken(string token)
        {
            if (token == "")
                throw new ArgumentNullException("token");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    return connection.Query<PasswordResetToken>("select Id, UserId, Token, Expires, Used from app_PasswordResetTokens where Token LIKE @Token", new { Token = token }).SingleOrDefault();
                }
            });
        }

        public virtual Task UpdateAsync(PasswordResetToken token)
        {
            if (token == null)
                throw new ArgumentNullException("token");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    connection.Execute("update app_PasswordResetTokens SET UserID=@UserId, Token=@Token, Expires=@Expires, Used=@Used WHERE ID=@Id", new { UserId=token.UserId, Token=token.Token, Expires=token.Expires.ToString("dd-MM-yyyy HH:mm:ss"), Used=(token.Used.HasValue ? token.Used.Value.ToString("dd-MM-yyyy HH:mm:ss"):"") });
            });
        }

        public Task<bool> IsTokenUnique(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token");

            List<Guid> _tokens = null;

            using (IDbConnection connection = CurrentContext.OpenConnection())
                _tokens = connection.Query<Guid>("select ID from app_PasswordResetTokens where Token LIKE @Token", new { Token = token }).ToList<Guid>();
            bool _return = true;
            if (_tokens.Count > 0)
            {
                _return = false;
            }
            return Task.Factory.StartNew(() =>
            {
                return _return;
            });
        }
    }
}
