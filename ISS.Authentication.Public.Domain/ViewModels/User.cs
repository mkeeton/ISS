using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ISS.Authentication.Public.Domain.ViewModels
{
    public class User
    {

        public User()
        {
            _logins = new List<ViewModels.Login>();
            _claims = new List<Claim>();
            _roles = new List<ViewModels.Role>();
        }

        private List<ViewModels.Login> _logins;

        private List<Claim> _claims;

        private List<ViewModels.Role> _roles;

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public bool HasPassword { get; set; }

        public string SecurityStamp { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<Claim> Claims
        {
            get
            {
                return _claims;
            }
        }

        public List<ViewModels.Login> Logins
        {
            get
            {
                return _logins;
            }
        }

        public List<ViewModels.Role> Roles
        {
            get
            {
                return _roles;
            }
        }
    }

    public class UserListViewModel
    {

        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class UserDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<UserRoleViewModel> AssignedRoles { get; set; }

        public List<UserRoleViewModel> AvailableRoles { get; set; }
    }
}
