using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer.Configuration
{
    public class Config
    {
        /*public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        */
        public static List<Client> Clients = new List<Client>
        {
            new Client
            {
                ClientId = "team-j-client",
                Enabled = true,
                AllowedGrantTypes = new List<string> {GrantType.ResourceOwnerPassword },
                ClientSecrets = {new Secret("secret".Sha256()) },
                //RequireClientSecret = false,
                RequireConsent = false,
                //RedirectUris = new List<string> {"http://localhost:3006/signin-callback.html"},
                //PostLogoutRedirectUris = new List<string> {"http://localhost:3006/"},
                AllowOfflineAccess = false,
                AllowedScopes = { "vaccination-system-api", IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile},
                //AllowedCorsOrigins = new List<string>
                //{
                //    "http://localhost:3006",
                //},
                AccessTokenLifetime = 86400
            }
        };

        public static List<ApiResource> ApiResources = new List<ApiResource>
        {
            new ApiResource
            {
                Name = "vaccination-system-api",
                DisplayName = "Vaccination System API",
                UserClaims = new[] {JwtClaimTypes.Profile, JwtClaimTypes.Role},
            }
        };

        public static IEnumerable<ApiScope> ApiScopes = new List<ApiScope>
        {
            new ApiScope(IdentityServerConstants.StandardScopes.OpenId),
            new ApiScope(IdentityServerConstants.StandardScopes.Profile),
            new ApiScope("vaccination-system-api")
        };
    }
}
