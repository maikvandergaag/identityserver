using IdentityModel.Client;
using IdentityServer.Identity;
using IdentityServer.Objects;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.InMemory;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace IdentityServer {
    public class Startup {

        public void Configuration(IAppBuilder app) {
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Trace(Serilog.Events.LogEventLevel.Information)
                        .CreateLogger();

            string certificate = WebConfigurationManager.AppSettings[WebConfigurationKeys.Thumbprint];
            X509Certificate2 identityCertificate = GetCertificate(certificate);

            app.Map("/identity", idsrvApp => {
                idsrvApp.UseIdentityServer(new IdentityServerOptions {
                    SiteName = "MSFTPlayground IdentityServer",
                    SigningCertificate = identityCertificate,

                    Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get()),

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions {
                        EnablePostSignOutAutoRedirect = true,
                    }
                });
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationType = "Cookies"
            });


            OpenIdConnectAuthenticationOptions options = GetOpenIdConnectionOptions();
            app.UseOpenIdConnectAuthentication(options);
        }

        private OpenIdConnectAuthenticationOptions GetOpenIdConnectionOptions() {
            string authority = WebConfigurationManager.AppSettings[WebConfigurationKeys.Authority];
            string clientId = WebConfigurationManager.AppSettings[WebConfigurationKeys.ClientId];
            string redirect = WebConfigurationManager.AppSettings[WebConfigurationKeys.RedirectUrl];
            OpenIdConnectAuthenticationOptions retVal = new OpenIdConnectAuthenticationOptions {
                Authority = authority,
                ClientId = clientId,
                ResponseType = "id_token token",
                RedirectUri = redirect,
                SignInAsAuthenticationType = "Cookies",
                UseTokenLifetime = false,
                Notifications = new OpenIdConnectAuthenticationNotifications {
                    SecurityTokenValidated = async n => {
                        var nid = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType, Constants.ClaimTypes.GivenName, Constants.ClaimTypes.Role);
                        // get userinfo data
                        var userInfoClient = new UserInfoClient(new Uri(n.Options.Authority + "/connect/userinfo").ToString());
                        var userInfo = await userInfoClient.GetAsync(n.ProtocolMessage.AccessToken);
                        userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Type, ui.Value)));

                        // keep the id_token for logout
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        // add access token for sample API
                        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                        // keep track of access token expiration
                        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                        // add some other app specific claim
                        nid.AddClaim(new Claim("app_specific", "some data"));

                        n.AuthenticationTicket = new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);
                    },
                    RedirectToIdentityProvider = n => {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest) {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null) {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }
            };

            return retVal;
        }

        public X509Certificate2 GetCertificate(string thumbprint) {

            if (string.IsNullOrEmpty(thumbprint))
                throw new ArgumentNullException("thumbprint", "Argument 'thumbprint' cannot be 'null' or 'string.empty'");

            X509Certificate2 retVal = null;

            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

            if (certCollection.Count > 0) {
                retVal = certCollection[0];
            }
            certStore.Close();

            return retVal;
        }
    }
}