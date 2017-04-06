using System;
using Microsoft.AspNet.Identity;

namespace ISS.Authentication.Domain.Models
{
    public class Role : IRole<Guid>
    {
        public Guid Id { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public string Name
        {
            get
            {
                return RoleName;
            }
            set
            {
                RoleName = value;
            }
        }
    }
}
