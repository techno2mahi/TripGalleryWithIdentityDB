using IdentityServer.WindowsAuthentication.Configuration;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TripCompany.WSFederationServer.Services;

namespace TripCompany.WSFederationServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWindowsAuthenticationService(new WindowsAuthenticationOptions
            {
                IdpRealm = "urn:win",
                IdpReplyUrl = TripGallery.Constants.TripGallerySTS + "/was",
                PublicOrigin = "https://localhost:44330/",
                SigningCertificate = LoadCertificate(),
                CustomClaimsProvider = new AdditionalWindowsClaimsProvider()
            });
        }


        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\certificates\idsrv3test.pfx",
                AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}
