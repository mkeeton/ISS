using System.Threading.Tasks;
using ISS.Authentication.Domain.Models;

namespace ISS.Authentication.Data.Interfaces.Repositories
{
    public interface IUserSessionRepository
    {
        Task CreateAsync(UserSession userSession);

        Task DeleteAsync(UserSession userSession);
    }
}
