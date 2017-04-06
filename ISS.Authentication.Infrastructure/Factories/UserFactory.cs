using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using ISS.Authentication.Data.UnitOfWork;

namespace ISS.Authentication.Infrastructure.Factories
{
    public class UserFactory : IDisposable
    {

        UnitOfWork _unitOfWork;

        public UserFactory(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ISS.Authentication.Public.Domain.ViewModels.User> Build(Guid userId)
        {
            ISS.Authentication.Domain.Models.User _user = await _unitOfWork.UserStore.FindByIdAsync(userId);
            return await Build(_user);
        }

        public async Task<List<ISS.Authentication.Public.Domain.ViewModels.User>> Build(List<Guid> userIds)
        {
            List<ISS.Authentication.Public.Domain.ViewModels.User> _users = new List<ISS.Authentication.Public.Domain.ViewModels.User>();
            userIds.ForEach(async x => _users.Add(await (Build(x))));
            return _users;
        }

        public async Task<ISS.Authentication.Public.Domain.ViewModels.User> Build(ISS.Authentication.Domain.Models.User user)
        {
            ISS.Authentication.Public.Domain.ViewModels.User _viewUser = new ISS.Authentication.Public.Domain.ViewModels.User();
            _viewUser.Id = user.Id;
            _viewUser.UserName = user.UserName;
            _viewUser.HasPassword = (user.PasswordHash != "");
            _viewUser.SecurityStamp = user.SecurityStamp;
            _viewUser.Email = user.Email;
            _viewUser.EmailConfirmed = user.EmailConfirmed;
            _viewUser.FirstName = user.FirstName;
            _viewUser.LastName = user.LastName;
            IList<UserLoginInfo> _logins = await _unitOfWork.UserStore.GetLoginsAsync(user);
            foreach (UserLoginInfo _login in _logins)
            {
                _viewUser.Logins.Add(new ISS.Authentication.Public.Domain.ViewModels.Login() { LoginProvider = _login.LoginProvider, ProviderKey = _login.ProviderKey });
            }
            return _viewUser;
        }

        public async Task<List<ISS.Authentication.Public.Domain.ViewModels.User>> Build(List<ISS.Authentication.Domain.Models.User> users)
        {
            List<ISS.Authentication.Public.Domain.ViewModels.User> _users = new List<ISS.Authentication.Public.Domain.ViewModels.User>();
            foreach (ISS.Authentication.Domain.Models.User _user in users)
            {
                _users.Add(await Build(_user));
            }
            return _users;
        }

        public void Dispose()
        {

        }
    }
}
