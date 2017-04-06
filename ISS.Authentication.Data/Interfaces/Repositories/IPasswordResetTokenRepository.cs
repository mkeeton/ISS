using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISS.Authentication.Domain.Models;

namespace ISS.Authentication.Data.Interfaces.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        Task<List<PasswordResetToken>> ListAsync();
        Task<PasswordResetToken> CreateAsync(Guid userId, int expiryMinutes);
        Task<PasswordResetToken> FindByToken(string token);
        Task UpdateAsync(PasswordResetToken token);
        Task<bool> IsTokenUnique(string token);
    }
}
