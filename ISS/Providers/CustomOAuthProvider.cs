using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ISS.Authentication.Domain.Models;
using System.Configuration;
using ISS.Framework;

namespace ISS.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {

        public ISS.Authentication.Data.UnitOfWork.UnitOfWork UnitOfWork
        {
            get
            {
                return ISS.WebApiApplication.GetContainer().Kernel.Resolve<ISS.Authentication.Data.UnitOfWork.UnitOfWork>();
            }
        }

        public Managers.ApplicationUserManager UserManager
        {
            get
            {
                return ISS.WebApiApplication.GetContainer().Kernel.Resolve<Managers.ApplicationUserManager>();
            }
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            string clientId = string.Empty;
            string clientSecret = string.Empty;
            try
            {
                context.OwinContext.Set<string>("as:remote_login_id", context.Parameters["remoteLogin_id"]);
            }
            catch (Exception exNoParam)
            {
                context.OwinContext.Set<string>("as:remote_login_id", "");
            }
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.OwinContext.Set<string>("as:current_client_id", ConfigurationManager.AppSettings["as:AudienceId"]);
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();

                return Task.FromResult<object>(null);
            }

            Client client = UnitOfWork.ClientStore.FindByURL(context.ClientId).Result;

            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:current_client_id", client.ClientId);

            context.Validated();
            context.OwinContext.Set<Client>("as:requested_client", client);

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Client client = context.OwinContext.Get<Client>("as:requested_client");
            string _authToken = String.Empty;
            if (client != null)
            {
                foreach (var _header in context.OwinContext.Request.Headers)
                {
                    if (_header.Key.ToLower() == "authorization")
                    {
                        if (_header.Value.Length > 0)
                        {
                            _authToken = _header.Value[0];
                            break;
                        }
                    }
                }
            }
            if (_authToken.IndexOf("Bearer ", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                _authToken = _authToken.Substring(7, _authToken.Length - 7);
            }
            var allowedOrigin = "NONE";

            List<ClientAllowedOrigin> _allowedOrigins;

            if (!(client == null))
            {
                _allowedOrigins = client.AllowedOrigins;
            }
            else
            {
                _allowedOrigins = await UnitOfWork.ClientStore.ListAllowedOrigins(new Guid());
            }

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
                        Client _sourceClient = await UnitOfWork.ClientStore.FindByURL(_allowedCompare);
                        if (_sourceClient != null)
                        {
                            context.OwinContext.Set<Client>("as:source_client", _sourceClient);
                        }
                        break;
                    }
                }
            }

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            AuthenticationProperties prop = new AuthenticationProperties();
            ClaimsIdentity oAuthIdentity = null;
            User user = null;

            //If the passed authentication token is empty then attempt to load the user from the username and password and then insert it into the AuthenticationTicket.
            //If the authentication token is not empty the ndecode the token and take the user identity from the token and place it in the new token for the remote resource.

                if (_authToken == "")
                {
                    user = await UserManager.FindAsync(context.UserName, context.Password);

                    if ((user == null) || ((user.Locked == true) && (user.LockedUntil.HasValue == true) && (user.LockedUntil.Value > DateTime.Now)))
                    {
                        if (user == null)
                        {
                            user = await UnitOfWork.UserStore.FindByEmailAsync(context.UserName);
                            if (user != null)
                            {
                                if ((user.Locked == true) && (user.LockedUntil.HasValue == true) &&
                                    (user.LockedUntil.Value > DateTime.Now))
                                {
                                    user.FailedLoginCount = 0;
                                    context.SetError("invalid_grant", "This user account is temporarily locked. Please try again later.");
                                }
                                else
                                {
                                    user.FailedLoginCount += 1;
                                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                                }
                            }
                            else
                            {
                                context.SetError("invalid_grant", "The user name or password is incorrect.");
                            }
                        }

                        if (user != null)
                        {
                            if (user.FailedLoginCount >= 3)
                            {
                                user.Locked = true;
                                user.LockedUntil = DateTime.Now.AddMinutes(10);
                            }
                            await UnitOfWork.UserStore.UpdateAsync(user);
                        }
                        return;
                    }
                    else
                    {
                        //Clear all currently stored expired refresh tokens.
                        await UnitOfWork.RefreshTokenStore.ClearExpiredAsync();
                    }
                    if (user.Locked == true)
                    {
                        user.FailedLoginCount = 0;
                        user.Locked = false;
                        user.LockedUntil = null;
                        await UnitOfWork.UserStore.UpdateAsync(user);
                    }
                    oAuthIdentity = await UserManager.GenerateUserIdentityAsync(user, "JWT");
                    oAuthIdentity.AddClaims(UnitOfWork.ClaimStore.GetClaims(user));

                    prop.Dictionary.Add("as:user_id", NullHandlers.NES(user.Id));
                }
                else
                {
                    Guid _userId = new Guid();
                    AuthenticationTicket _authTicket = context.Options.AccessTokenFormat.Unprotect(_authToken);
                    ClaimsIdentity _ident = _authTicket.Identity;
                    foreach (Claim _claim in _ident.Claims)
                    {
                        foreach (var _prop in _claim.Properties)
                        {
                            if (_prop.Value == "nameid")
                            {
                                _userId = NullHandlers.NGUID(_claim.Value);
                                break;
                            }
                        }
                        if (_userId != Guid.Empty)
                        {
                            break;
                        }
                    }
                    if ((_userId == Guid.Empty) || (_ident == null))
                    {
                        throw new Exception("Invalid Authentication Token");
                    }
                    oAuthIdentity = _ident;
                    prop.Dictionary.Add("as:user_id", NullHandlers.NES(_userId));
                }

            prop.Dictionary.Add("as:issuer", ConfigurationManager.AppSettings["as:IssuerId"]);

            if (client == null)
            {
                prop.Dictionary.Add("as:client_id", ConfigurationManager.AppSettings["as:AudienceId"]);
                prop.Dictionary.Add("as:client_secret", ConfigurationManager.AppSettings["as:AudienceSecret"]);
            }
            else
            {
                prop.Dictionary.Add("as:client_id", client.ClientId);
                prop.Dictionary.Add("as:client_secret", client.Secret);
            }
            var ticket = new AuthenticationTicket(oAuthIdentity, prop);

            context.Validated(ticket);

        }



        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newClaim = newIdentity.Claims.Where(c => c.Type == "newClaim").FirstOrDefault();
            if (newClaim != null)
            {
                newIdentity.RemoveClaim(newClaim);
            }
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            AuthenticationProperties prop = context.Ticket.Properties;
            Client _client = context.OwinContext.Get<Client>("as:requested_client");
            if (!(_client == null))
            {
                prop.Dictionary.Remove("as:client_id");
                prop.Dictionary.Remove("as:client_secret");
                prop.Dictionary.Add("as:client_id", _client.ClientId);
                prop.Dictionary.Add("as:client_secret", _client.Secret);
            }
            var ticket = new AuthenticationTicket(newIdentity, prop);

            context.Validated(ticket);

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            //foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            //{
            //  context.AdditionalResponseParameters.Add(property.Key, property.Value);
            //}

            return Task.FromResult<object>(null);
        }

        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            if (context.IsTokenEndpoint && context.Request.Method == "OPTIONS")
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "authorization" });
                context.RequestCompleted();
                return Task.FromResult(0);
            }

            return base.MatchEndpoint(context);
        }
    }
}