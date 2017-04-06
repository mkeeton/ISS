using System;
using Microsoft.AspNet.Identity;

namespace ISS.Authentication.Domain.Models
{
    public class User : IUser<Guid>
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

        public string UserName
        {
            get
            {
                return Email;
            }
            set
            {
                Email = value;
            }
        }

        public class SearchParameters
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
