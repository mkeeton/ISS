using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ISS.Authentication.Domain.Models
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public DateTime? Used { get; set; }
    }
}
