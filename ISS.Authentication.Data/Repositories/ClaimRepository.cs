﻿using System;
using System.Collections.Generic;
using ISS.Authentication.Data.Interfaces.DbContext;
using ISS.Authentication.Domain.Models;
using System.Security.Claims;

namespace ISS.Authentication.Data.Repositories
{
    public class ClaimRepository : Interfaces.Repositories.IClaimRepository
    {

        private readonly IDbContext CurrentContext;

        public ClaimRepository(IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("connectionString");

            this.CurrentContext = context;
        }

        public IEnumerable<Claim> GetClaims(User user)
        {

            List<Claim> claims = new List<Claim>();

            //var daysInWork = (DateTime.Now.Date - user.JoinDate).TotalDays;

            //if (daysInWork > 90)
            //{
            //  claims.Add(CreateClaim("FTE", "1"));

            //}
            //else
            //{
            //  claims.Add(CreateClaim("FTE", "0"));
            //}

            return claims;
        }

        public Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, ClaimValueTypes.String);
        }
    }
}
