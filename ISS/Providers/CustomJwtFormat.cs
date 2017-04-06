using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;
using Thinktecture.IdentityModel.Tokens;
using System.Security.Claims;
using System.Globalization;
using ISS.Authentication.Domain.Models;

namespace ISS.Providers
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {

        private string _issuer = string.Empty;

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (_issuer == "")
            {
                _issuer = data.Properties.Dictionary["as:issuer"];
            }

            string audienceId = data.Properties.Dictionary["as:client_id"];//ConfigurationManager.AppSettings["as:AudienceId"];

            string symmetricKeyAsBase64 = data.Properties.Dictionary["as:client_secret"]; //ConfigurationManager.AppSettings["as:AudienceSecret"];

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

            var signingKey = new HmacSigningCredentials(keyByteArray);

            var issued = data.Properties.IssuedUtc;

            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            var handler = new JwtSecurityTokenHandler();
            TokenValidationParameters _tokenParams = new TokenValidationParameters();
            string audienceId = "";
            string symmetricKeyAsBase64 = "";

            Client _sourceClient = HttpContext.Current.GetOwinContext().Get<Client>("as:source_client");
            if (_sourceClient == null)
            {
                audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
                symmetricKeyAsBase64 = ConfigurationManager.AppSettings["as:AudienceSecret"];
            }
            else
            {
                audienceId = _sourceClient.ClientId;
                symmetricKeyAsBase64 = _sourceClient.Secret;
            }

            _tokenParams.ValidIssuer = ConfigurationManager.AppSettings["as:IssuerId"];
            _tokenParams.ValidAudience = audienceId;

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

            var signingKey = new HmacSigningCredentials(keyByteArray).SigningKey;

            var signingKeys = new List<SecurityKey>();

            signingKeys.Add(signingKey);

            _tokenParams.IssuerSigningKeys = signingKeys;

            SecurityToken _validatedToken = null;

            ClaimsPrincipal _principal = null;

            _principal = handler.ValidateToken(protectedText.ToString(), _tokenParams, out _validatedToken);

            if (_validatedToken == null)
            {
                throw new Exception("Invalid Authentication Token");
            }
            else
            {

                string IssuedAtClaimName = "iat";

                string ExpiryClaimName = "exp";

                string JwtIdClaimName = "jti";

                DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var claimsIdentity = (ClaimsIdentity)_principal.Identity;

                // Fill out the authenticationExtra issued and expires times if the equivalent claims are in the JWT
                var authenticationExtra = new AuthenticationProperties(new Dictionary<string, string>());
                if (claimsIdentity.Claims.Any(c => c.Type == ExpiryClaimName))
                {
                    string expiryClaim = (from c in claimsIdentity.Claims where c.Type == ExpiryClaimName select c.Value).Single();
                    authenticationExtra.ExpiresUtc = _epoch.AddSeconds(Convert.ToInt64(expiryClaim, CultureInfo.InvariantCulture));
                }

                if (claimsIdentity.Claims.Any(c => c.Type == IssuedAtClaimName))
                {
                    string issued = (from c in claimsIdentity.Claims where c.Type == IssuedAtClaimName select c.Value).Single();
                    authenticationExtra.IssuedUtc = _epoch.AddSeconds(Convert.ToInt64(issued, CultureInfo.InvariantCulture));
                }

                // Finally, create a new ClaimsIdentity so the auth type is JWT rather than Federated.
                var returnedIdentity = new ClaimsIdentity(claimsIdentity.Claims, "JWT");

                return new AuthenticationTicket(returnedIdentity, authenticationExtra);
            }
            throw new NotImplementedException();

        }

    }
}