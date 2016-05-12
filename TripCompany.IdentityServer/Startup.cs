using System.Configuration;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Services.InMemory;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.WsFederation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TripCompany.IdentityServer.Config;
using TripCompany.IdentityServer.Services;

namespace TripCompany.IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idsrvApp =>
            {
               
                var options = new IdentityServerOptions
                {
                    Factory = Factory.Configure(), 
                    SiteName = "TripCompany Security Token Service",
                    SigningCertificate = LoadCertificate(),
                    IssuerUri = TripGallery.Constants.TripGalleryIssuerUri,
                    PublicOrigin = TripGallery.Constants.TripGallerySTSOrigin,
                    AuthenticationOptions = new AuthenticationOptions()
                    {
                        EnablePostSignOutAutoRedirect = true,
                        LoginPageLinks = new List<LoginPageLink>()
                        {
                            new LoginPageLink()
                            {
                                Type= "createaccount",
                                Text = "Create a new account",
                                Href = "~/createuseraccount"
                            }
                        },
                        IdentityProviders = ConfigureAdditionalIdProviders
                    },
                    CspOptions = new CspOptions()
                    {
                        Enabled = false
                        // once available, leave Enabled at true and use:
                        // FrameSrc = "https://localhost:44318 https://localhost:44316"
                        // or
                        // FrameSrc = "*" for all URI's.
                    }
                };

                idsrvApp.UseIdentityServer(options);
            });
        }

        private void ConfigureAdditionalIdProviders(IAppBuilder appBuilder, string signInAsType)
        {
            var fbAuthOptions = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "895739530475035",
                AppSecret = "af8fb8900e65ebd7b7056265d130c3ee",
                Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        using (var client = new HttpClient())
                        {
                            // get claims from FB's graph 

                            var result = client.GetAsync("https://graph.facebook.com/me?fields=first_name,last_name,email&access_token="
                              + context.AccessToken).Result;

                            if (result.IsSuccessStatusCode)
                            {
                                var userInformation = result.Content.ReadAsStringAsync().Result;
                                var fbUser = JsonConvert.DeserializeObject<FacebookUser>(userInformation);

                                context.Identity.AddClaim(new System.Security.Claims.Claim(
                                    IdentityServer3.Core.Constants.ClaimTypes.GivenName, fbUser.first_name));
                                context.Identity.AddClaim(new System.Security.Claims.Claim(
                                    IdentityServer3.Core.Constants.ClaimTypes.FamilyName, fbUser.last_name));
                                context.Identity.AddClaim(new System.Security.Claims.Claim(
                                 IdentityServer3.Core.Constants.ClaimTypes.Email, fbUser.email));

                                //// there's no role concept...
                                //context.Identity.AddClaim(new System.Security.Claims.Claim(
                                //    "role", "FreeUser"));
                            }

                        }

                        return Task.FromResult(0);
                    }
                }
            };
            fbAuthOptions.Scope.Add("email");

            appBuilder.UseFacebookAuthentication(fbAuthOptions);

            var windowsAuthentication = new WsFederationAuthenticationOptions
            {
                AuthenticationType = "windows",
                Caption = "Windows",
                SignInAsAuthenticationType = signInAsType,
                MetadataAddress = "https://localhost:44330/",
                Wtrealm = "urn:win"

            };
            appBuilder.UseWsFederationAuthentication(windowsAuthentication);
        }




        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\certificates\idsrv3test.pfx",
                AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}
