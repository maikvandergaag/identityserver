using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace IdentityServer.Identity
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client 
                {
                    ClientName = "MSFTPlayground Client",
                    ClientId = "MSFTPlayground",
                    Flow = Flows.Implicit,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44357/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:44357/"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "profile",
                        "roles",
                        "sampleApi"
                    }
                }
            };
        }
    }
}