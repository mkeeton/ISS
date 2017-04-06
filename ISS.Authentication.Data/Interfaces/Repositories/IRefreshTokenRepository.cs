using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISS.Authentication.Domain.Models;

namespace ISS.Authentication.Data.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<List<RefreshToken>> ListAsync();
        Task<RefreshToken> FindById(string tokenId);
        Task<RefreshToken> FindByTokenUser(Guid userId);
        Task<bool> CreateAsync(RefreshToken refreshToken);
        Task<bool> DeleteAsync(RefreshToken refreshToken);
        Task<bool> DeleteAsync(string refreshTokenId);
        Task<bool> ClearExpiredAsync();
    }
}
