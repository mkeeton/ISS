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
    public class ClientRepository : Interfaces.Repositories.IClientRepository
    {

        private readonly IDbContext CurrentContext;

        public ClientRepository(IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("connectionString");

            this.CurrentContext = context;
        }

        public Task<List<Client>> ListAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    List<Client> _clientList = (connection.Query<Client>("select Id, ClientName, ClientId, Secret, Active, RefreshTokenLifeTime from auth_Clients", new { }).AsList());
                    foreach (Client _client in _clientList)
                    {
                        _client.URLs = ListURLs(_client).Result;
                        _client.AllowedOrigins = ListAllowedOrigins(_client).Result;
                    }
                    return _clientList;
                }
            });
        }

        public Task<Client> FindById(Guid clientId)
        {
            if (clientId == Guid.Empty)
                throw new ArgumentNullException("clientId");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    Client _client = connection.Query<Client>("select Id, ClientName, ClientId, Secret, Active, RefreshTokenLifeTime from auth_Clients where Id = @Id", new { Id = clientId }).SingleOrDefault();
                    if (_client != null)
                    {
                        _client.URLs = ListURLs(_client).Result;
                        _client.AllowedOrigins = ListAllowedOrigins(_client).Result;
                    }
                    return _client;
                }
            });
        }

        public Task<Client> FindByClientAppId(string clientAppId)
        {
            if (clientAppId == "")
                throw new ArgumentNullException("id");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    Client _client = connection.Query<Client>("select Id, ClientName, ClientId, Secret, Active, RefreshTokenLifeTime from auth_Clients where ClientId = @ClientId", new { ClientId = clientAppId }).SingleOrDefault();
                    if (_client != null)
                    {
                        _client.URLs = ListURLs(_client).Result;
                        _client.AllowedOrigins = ListAllowedOrigins(_client).Result;
                    }
                    return _client;
                }
            });
        }

        public Task<Client> FindByURL(string clientURL)
        {
            if (clientURL == "")
                throw new ArgumentNullException("id");

            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                {
                    Client _client = connection.Query<Client>("select C.Id, C.ClientName, C.ClientId, C.Secret, C.Active, C.RefreshTokenLifeTime from auth_Clients C INNER JOIN auth_ClientURLs CU ON CU.ClientId=C.Id where CU.URL LIKE @ClientURL", new { ClientURL = clientURL.Trim() }).SingleOrDefault();
                    if (_client != null)
                    {
                        _client.URLs = ListURLs(_client).Result;
                        _client.AllowedOrigins = ListAllowedOrigins(_client).Result;
                    }
                    return _client;
                }
            });
        }

        public Task<List<ClientURL>> ListURLs(Client client)
        {
            return ListURLs(client.Id);
        }

        public Task<List<ClientURL>> ListURLs(Guid clientId)
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<ClientURL>("select Id, ClientId, URL from auth_ClientURLs WHERE ClientId=@ClientId", new { ClientId = clientId }).AsList();
            });
        }

        public Task<List<ClientAllowedOrigin>> ListAllowedOrigins(Client client)
        {
            return ListAllowedOrigins(client.Id);
        }

        public Task<List<ClientAllowedOrigin>> ListAllowedOrigins(Guid clientId)
        {
            return Task.Factory.StartNew(() =>
            {
                using (IDbConnection connection = CurrentContext.OpenConnection())
                    return connection.Query<ClientAllowedOrigin>("select Id, ClientId, AllowedURL from auth_ClientAllowedOrigins WHERE ClientId=@ClientId", new { ClientId = clientId }).AsList();
            });
        }

        public virtual Task SaveAsync(Client client)
        {
            if (client == null)
                throw new ArgumentNullException("role");
            if (client.Id == Guid.Empty)
            {
                return Task.Factory.StartNew(() =>
                {
                    client.Id = Guid.NewGuid();
                    using (IDbConnection connection = CurrentContext.OpenConnection())
                        connection.Execute("insert into auth_Clients(Id, ClientName, ClientId, Secret, Active, RefreshTokenLifeTime) values(@Id, @ClientName, @ClientId, @Secret, @Active, @RefreshTokenLifeTime)", new { Id = client.Id, ClientName = client.ClientName, ClientId = client.ClientId, Secret = client.Secret, Active = client.Active, RefreshTokenLifeTime = client.RefreshTokenLifeTime });
                });
            }
            else
            {
                return Task.Factory.StartNew(() =>
                {
                    using (IDbConnection connection = CurrentContext.OpenConnection())
                        connection.Execute("UPDATE auth_Clients SET ClientName=@ClientName, ClientId=@ClientId, Secret=@Secret, Active=@Active, RefeshTokenLifeTime=@RefreshTokenLifeTime WHERE Id=@Id", new { ClientName=client.ClientName, ClientId=client.ClientId, Secret=client.Secret, Active=client.Active, RefreshTokenLifeTime=client.RefreshTokenLifeTime, Id=client.Id });
                });
            }
        }

        public virtual Task AddURLToClientAsync(string uRL, Guid clientId)
        {
            if (clientId == Guid.Empty)
                throw new ArgumentNullException("clientId");

            if (uRL == "")
                throw new ArgumentNullException("uRL");

            Client _client = FindByURL(uRL).Result;

            if (_client == null)
            {
                return Task.Factory.StartNew(() =>
                {
                    ClientURL _uRL = new ClientURL();
                    _uRL.Id = Guid.NewGuid();
                    _uRL.ClientId = clientId;
                    _uRL.URL = uRL;
                    using (IDbConnection connection = CurrentContext.OpenConnection())
                        connection.Execute("insert into auth_ClientURLs(Id, ClientId, URL) values(@Id, @ClientId, @URL)", new { Id=_uRL.Id, ClientId=_uRL.ClientId, URL=_uRL.URL });
                });
            }
            else
            {
                throw new Exception("URL already assigned to " + _client.ClientName);
            }
        }
    }
}
