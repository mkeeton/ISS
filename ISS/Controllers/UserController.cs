using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Threading.Tasks;

using ISS.Framework;

using ISS.Authentication.Public.Domain.ViewModels;
using ISS.Authentication.Data.Repositories;
using ISS.Authentication.Infrastructure.Factories;
using ISS.Authentication.Data.UnitOfWork;

namespace ISS.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {

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

        private UnitOfWork _unitOfWork;

        protected UnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork;
            }
            set
            {
                _unitOfWork = value;
            }
        }

        public UserController(UserFactory userFactory, UnitOfWork unitOfWork)
        {
            _userFactory = userFactory;
            _unitOfWork = unitOfWork;
        }

        // POST api/User/CurrentUser
        [Authorize]
        [Route("CurrentUser")]
        [HttpGet]
        public async Task<IHttpActionResult> CurrentUser()
        {
            try
            {
                User _user = await _userFactory.Build(await _unitOfWork.UserStore.FindByNameAsync(User.Identity.Name));
                return Ok(_user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        // POST api/User/GetUser
        [Authorize]
        [Route("GetUser")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            try
            {
                User _user = await _userFactory.Build(NullHandlers.NGUID(Id));
                return Ok(_user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        // POST api/User/GetUsers
        [Authorize]
        [Route("GetUsers")]
        [HttpPost]
        public async Task<IHttpActionResult> GetUsers(List<Guid> Ids)
        {
            try
            {
                List<User> _users = await _userFactory.Build(Ids);
                return Ok(_users);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        // POST api/User/UsersFromRole
        [Authorize(Roles = "HABC Administrator")]
        [Route("UsersFromRole")]
        [HttpGet]
        public async Task<IHttpActionResult> UsersFromRole(string roleName)
        {
            try
            {
                List<User> _users = await _userFactory.Build(await _unitOfWork.UserStore.ListUsersInRoleAsync(roleName));
                return Ok(_users);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
    }
}
