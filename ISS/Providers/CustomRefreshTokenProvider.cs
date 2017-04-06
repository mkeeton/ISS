using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISS.Authentication.Domain.Models;
using ISS.Framework;

namespace ISS.Providers
{
    public class CustomRefreshTokenProvider : IAuthenticationTokenProvider
    {

        public ISS.Authentication.Data.UnitOfWork.UnitOfWork UnitOfWork
        {
            get
            {
                return ISS.WebApiApplication.GetContainer().Kernel.Resolve<ISS.Authentication.Data.UnitOfWork.UnitOfWork>();
            }
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Guid userid = NullHandlers.NGUID(context.Ticket.Properties.Dictionary["as:user_id"]);
            Client _client = context.OwinContext.Get<Client>("as:requested_client");
            if (_client == null)
            {
                if (userid == Guid.Empty)
                {
                    return;
                }

                var refreshTokenId = Guid.NewGuid().ToString("n");

                var refreshTokenLifeTime = 1440;

                var token = new RefreshToken()
                {
                    Id = Encryption.GetHash(refreshTokenId),
                    UserId = userid,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

                token.ProtectedTicket = context.SerializeTicket();
                var result = await UnitOfWork.RefreshTokenStore.CreateAsync(token);

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = "NONE";

            List<ClientAllowedOrigin> _allowedOrigins;
            _allowedOrigins = await UnitOfWork.ClientStore.ListAllowedOrigins(new Guid());

            string _originPath = NullHandlers.NES(context.Request.Headers["Origin"]);
            if (_originPath == "")
            {
                allowedOrigin = "*";
            }
            else
            {
                string _originCompare = _originPath.ToLower().Trim();
                if (_originCompare.LastIndexOf("/") != (_originCompare.Length - 1)) _originCompare += "/";
                foreach (ClientAllowedOrigin _origin in _allowedOrigins)
                {
                    string _allowedCompare = _origin.AllowedURL.ToLower().Trim();
                    if ((_allowedCompare.LastIndexOf("/") != (_allowedCompare.Length - 1)) && (_allowedCompare != "*")) _allowedCompare += "/";
                    if ((_origin.AllowedURL.ToLower().Trim() == "*") || (_origin.AllowedURL.ToLower().Trim() == _originCompare))
                    {
                        allowedOrigin = _originPath;
                        break;
                    }
                }
            }
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = Encryption.GetHash(context.Token);
            var refreshToken = await UnitOfWork.RefreshTokenStore.FindById(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);

                var currentClient = context.OwinContext.Get<string>("as:current_client_id");
                var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];

                if (originalClient.ToLower().Trim() == currentClient.ToLower().Trim())
                {
                    var result = await UnitOfWork.RefreshTokenStore.DeleteAsync(hashedTokenId);
                }
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}