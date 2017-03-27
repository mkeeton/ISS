using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Authentication.Public.Domain.ViewModels
{
    public class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


    }

    public class UserRoleViewModel
    {

        public Guid RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleDescription { get; set; }
    }

    public class RoleViewModel
    {
        public Guid Id { get; set; }

        public string RoleName { get; set; }

        public string RoleDescription { get; set; }

        public List<ViewModels.Api> AssignedApis { get; set; }

        public List<ViewModels.Api> AvailableApis { get; set; }

        public List<ViewModels.Client> AvailableClientPaths { get; set; }

        public List<ViewModels.Client> AssignedClientPaths { get; set; }
    }
}
