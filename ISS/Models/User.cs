using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using ISS.Authentication.Domain.Models;

namespace ISS.Models
{
    public class User : ISS.Authentication.Domain.Models.User, IUser<Guid>
    {
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
    }
}