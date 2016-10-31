using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer.Identity
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Username = "MSFTPlayground",
                    Password = "secret",
                    Subject = "1",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "MSFT"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Playground"),
                        new Claim(Constants.ClaimTypes.Role, "Test"),
                        new Claim(Constants.ClaimTypes.Role, "Application")
                    }
                }
            };
        }
    }
}