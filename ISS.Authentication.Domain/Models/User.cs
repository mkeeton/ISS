using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Authentication.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int FailedLoginCount { get; set; }

        public bool Locked { get; set; }

        public DateTime? LockedUntil { get; set; }

        public class SearchParameters
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
