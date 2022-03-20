using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(WebApplication.Startup))]

namespace WebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType,
                ClientId = "interactive",
                ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
                Authority = "https://localhost:5001",
                RedirectUri = "https://localhost:44359/",
                PostLogoutRedirectUri = "https://localhost:44359/",
                ResponseType = OpenIdConnectResponseType.Code,
                Scope = "openid profile",
                RedeemCode = true,
                UseTokenLifetime = false,
                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        await Task.CompletedTask;
                    },
                    RedirectToIdentityProvider = async n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            n.ProtocolMessage.IdTokenHint = n.OwinContext?.Authentication?.User?.FindFirst("id_token")?.Value;
                        }

                        await Task.CompletedTask;
                    }
                }
            });
        }
    }
}