using System.Collections.Generic;
using ISS.Authentication.Domain.Models;
using System.Security.Claims;

namespace ISS.Authentication.Data.Interfaces.Repositories
{
    public interface IClaimRepository
    {
        IEnumerable<Claim> GetClaims(User user);
        Claim CreateClaim(string type, string value);
    }
}
