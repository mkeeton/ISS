using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ISS.Authentication.Domain.Models
{
    public class RefreshToken
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public DateTime IssuedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }

        [Required]
        public string ProtectedTicket { get; set; }

    }
}
