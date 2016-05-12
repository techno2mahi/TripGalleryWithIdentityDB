using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.Core.Services.InMemory;
using TripCompany.IdentityServer.Config;
using TripCompany.IdentityServer.Services;

namespace TripCompany.IdentityServer
{
    class Factory
    {
        public static IdentityServerServiceFactory Configure()
        {
              var corsPolicyService = new DefaultCorsPolicyService()
            {
                AllowAll = true
            };

            // var defaultViewServiceOptions = new DefaultViewServiceOptions();
            // defaultViewServiceOptions.CacheViews = false;

            var idServerServiceFactory = new IdentityServerServiceFactory()
                            .UseInMemoryClients(Clients.Get())
                            .UseInMemoryScopes(Scopes.Get());
            //  .UseInMemoryUsers(Users.Get());

            idServerServiceFactory.CorsPolicyService = new
                Registration<IdentityServer3.Core.Services.ICorsPolicyService>(corsPolicyService);

            // idServerServiceFactory.ConfigureDefaultViewService(defaultViewServiceOptions);
            idServerServiceFactory.ViewService = new Registration<IViewService>(typeof(CustomViewService));

            // use custom UserService
            //var customUserService = new CustomUserService();
            // idServerServiceFactory.UserService = new Registration<IUserService>(resolver => customUserService);

            //idServerServiceFactory.Configure(connectionString);

            idServerServiceFactory.ConfigureUserService("AspId");
            return idServerServiceFactory;
        }
    }
}