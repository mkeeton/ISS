using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ISS.Authentication.Domain.Models
{
    public class Client
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ClientName { get; set; }

        public string ClientId { get; set; }

        [Required]
        public string Secret { get; set; }

        public bool Active { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        public List<ClientURL> URLs { get; set; }

        public List<ClientAllowedOrigin> AllowedOrigins { get; set; }
    }
}
