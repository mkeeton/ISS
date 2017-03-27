using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Authentication.Domain.Models
{
    public class ClientURL
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }

        public string URL { get; set; }

    }
}
