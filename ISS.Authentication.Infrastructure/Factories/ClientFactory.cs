using System;
using System.Threading.Tasks;
using ISS.Authentication.Data.UnitOfWork;
using System.Security.Cryptography;

namespace ISS.Authentication.Infrastructure.Factories
{
    public class ClientFactory : IDisposable
    {

        UnitOfWork _unitOfWork;

        public ClientFactory(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ISS.Authentication.Domain.Models.Client> Create(string clientName, string clientURL, int tokenExpiryTime)
        {
            ISS.Authentication.Domain.Models.Client _existingClient = await _unitOfWork.ClientStore.FindByURL(clientURL);
            if (_existingClient == null)
            {
                ISS.Authentication.Domain.Models.Client _client = new ISS.Authentication.Domain.Models.Client();
                _client.ClientName = clientName;
                _client.ClientId = Guid.NewGuid().ToString("N");
                var key = new byte[32];
                RNGCryptoServiceProvider.Create().GetBytes(key);
                _client.Secret = System.Convert.ToBase64String(key);
                _client.RefreshTokenLifeTime = tokenExpiryTime;
                await _unitOfWork.ClientStore.SaveAsync(_client);
                if (_client.Id != Guid.Empty)
                {
                    await _unitOfWork.ClientStore.AddURLToClientAsync(clientURL, _client.Id);
                }
                return _client;
            }
            else
            {
                throw new Exception("URL already assigned to " + _existingClient.ClientName);
            }
        }

        public void Dispose()
        {

        }
    }
}
