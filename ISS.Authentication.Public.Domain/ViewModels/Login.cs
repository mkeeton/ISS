using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Authentication.Public.Domain.ViewModels
{
    public class Login
    {
        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }
    }
}
