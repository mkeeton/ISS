using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISS.Authentication.Domain.Models;

namespace ISS.Authentication.Data.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<List<Client>> ListAsync();
        Task<Client> FindById(Guid Id);
        Task<Client> FindByClientAppId(string clientId);
        Task<Client> FindByURL(string uRL);
        Task SaveAsync(Client client);
        Task AddURLToClientAsync(string uRL, Guid clientId);
        Task<List<ClientAllowedOrigin>> ListAllowedOrigins(Client client);
        Task<List<ClientAllowedOrigin>> ListAllowedOrigins(Guid clientId);
    }
}
