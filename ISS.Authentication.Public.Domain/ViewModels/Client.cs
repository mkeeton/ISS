using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Authentication.Public.Domain.ViewModels
{
    public class Client
    {
        public Guid Id { get; set; }

        public string ClientName { get; set; }

        public string Path { get; set; }
    }
}
