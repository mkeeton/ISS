using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using ISS.Models;
using ISS.Providers;
using ISS.Results;
using ISS.Authentication.Domain.Models;

using ISS.Authentication.Infrastructure.Factories;

using ISS.Framework;

namespace ISS.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";

        private UserFactory _userFactory;

        protected UserFactory UserFactory
        {
            get
            {
                return _userFactory;
            }
            set
            {
                _userFactory = value;
            }
        }

        private Managers.ApplicationUserManager _userManager;

        protected Managers.ApplicationUserManager UserManager
        {
            get
            {
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }

        //private IEmailService _emailService;

        //protected IEmailService EmailService
        //{
        //    get
        //    {
        //        return _emailService;
        //    }
        //    set
        //    {
        //        _emailService = value;
        //    }
        //}

        public ISS.Authentication.Data.UnitOfWork.UnitOfWork UnitOfWork { get; set; }

        public AccountController(UserFactory userFactory, Managers.ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)//, IEmailService emailService)
        {
            _userFactory = userFactory;
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
           // _emailService = emailService;
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        [HttpGet]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            ClaimsIdentity userIdent = User.Identity as ClaimsIdentity;
            foreach (Claim cl in userIdent.Claims)
            {
                if (cl.Type == "authSessionId")
                {
                    break;
                }
            }
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            ISS.Authentication.Public.Domain.ViewModels.User user = await _userFactory.Build(new Guid(User.Identity.GetUserId()));

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (ISS.Authentication.Public.Domain.ViewModels.Login linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.HasPassword == true)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(new Guid(User.Identity.GetUserId()), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(new Guid(User.Identity.GetUserId()), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/PasswordReminder
        [Route("PasswordReminder")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> PasswordReminder(ForgottenPasswordBindingModel model)
        {

            ISS.Authentication.Domain.Models.User _user = await UnitOfWork.UserStore.FindByEmailAsync(model.Email);
            List<ISS.Authentication.Domain.Models.User> _users = await UnitOfWork.UserStore.ListAsync();
            if (_user == null)
            {
                return BadRequest();
            }
            else
            {
                //ISS.Authentication.Domain.Models.EmailTemplate _template = await UnitOfWork.EmailTemplateStore.FindByIdAsync(NullHandlers.NGUID(ConfigurationManager.AppSettings["passwordReminderTemplateId"]));
                //if (_template == null)
                //{
                //    return InternalServerError();
                //}

                ISS.Authentication.Domain.Models.PasswordResetToken _token = await UnitOfWork.PasswordResetTokenStore.CreateAsync(_user.Id, 60);
                if (_token == null)
                {
                    return InternalServerError();
                }

                //string _body = _template.Body.Replace("[[Token]]", _token.Token).Replace("[[User.FirstName]]", _user.FirstName);

                //List<string> _to = new List<string>();
                //_to.Add(model.Email);
                //if (await EmailService.SendEmail(_template.Subject, _body, _template.From, _to, new List<string>(), new List<string>(), new List<string>()))
                //{
                    return Ok();
                //}
                //else
                //{
                //    return InternalServerError();
                //}
            }

        }

        // POST api/Account/ConfirmToken
        [Route("ConfirmToken")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ConfirmToken([FromBody]string token)
        {
            PasswordResetToken _token = await UnitOfWork.PasswordResetTokenStore.FindByToken(token);
            if (_token == null)
            {
                return BadRequest("Invalid Token");
            }
            if (_token.Expires < DateTime.Now)
            {
                return BadRequest("Expired Token");
            }
            if (_token.Used.HasValue)
            {
                return BadRequest("Token Already Used");
            }
            ISS.Authentication.Public.Domain.ViewModels.User _user = await UserFactory.Build(_token.UserId);
            if (_user == null)
            {
                return BadRequest("User not Found");
            }
            return Ok(_user);
        }

        // POST api/Account/PasswordReset
        [Route("PasswordReset")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> PasswordReset(ResetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            PasswordResetToken _token = await UnitOfWork.PasswordResetTokenStore.FindByToken(model.ResetToken);
            if (_token == null)
            {
                return BadRequest("Invalid Token");
            }
            if (_token.Expires < DateTime.Now)
            {
                return BadRequest("Expired Token");
            }
            if (_token.Used.HasValue)
            {
                return BadRequest("Token Already Used");
            }
            ISS.Authentication.Domain.Models.User _user = await UnitOfWork.UserStore.FindByIdAsync(_token.UserId);
            if (_user == null)
            {
                return BadRequest("User not Found");
            }
            IdentityResult _result = await UserManager.RemovePasswordAsync(_user.Id);
            _result = await UserManager.AddPasswordAsync(_user.Id, model.NewPassword);
            _token.Used = DateTime.Now;
            if (_result.Succeeded)
            {
                await UnitOfWork.PasswordResetTokenStore.UpdateAsync(_token);
                return Ok();
            }
            else
            {
                string _errors = "";
                foreach (string _error in _result.Errors)
                {
                    if (_errors != "") { _errors += "; "; }
                    _errors += _error;
                }
                return BadRequest(_errors);
            }
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(new Guid(User.Identity.GetUserId()),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(new Guid(User.Identity.GetUserId()));
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(new Guid(User.Identity.GetUserId()),
                      new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ISS.Authentication.Domain.Models.User user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await UserManager.GenerateUserIdentityAsync(user,
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await UserManager.GenerateUserIdentityAsync(user,
                   CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ISS.Authentication.Domain.Models.User() { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ISS.Authentication.Domain.Models.User() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        // GET api/Account/UserAccountSummary
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserAccountSummary")]
        [HttpGet]
        public async Task<IHttpActionResult> UserAccountSummary()
        {
            var user = await _userFactory.Build(new Guid(User.Identity.GetUserId()));
            return Ok(user);
        }

        // POST api/Account/UpdateAccount
        [Route("UpdateAccount")]
        public async Task<IHttpActionResult> UpdateAccount(UpdateAccountBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ISS.Authentication.Domain.Models.User user = await UserManager.FindByIdAsync(new Guid(User.Identity.GetUserId()));
            if ((model.Email != user.Email) && (model.Password != null) && (model.Password.Trim() != ""))
            {
                if (await UserManager.CheckPasswordAsync(user, model.Password) == true)
                {
                    user.Email = model.Email;
                    user.UserName = user.Email;
                    user.EmailConfirmed = false;
                }
                else
                {
                    return BadRequest("The provided password was incorrect");
                }
            }
            else
            {
                return BadRequest("You must provide your password in order to change your email address");
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            IdentityResult result = UserManager.Update(user);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userFactory != null)
            {
                _userFactory.Dispose();
                _userFactory = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
