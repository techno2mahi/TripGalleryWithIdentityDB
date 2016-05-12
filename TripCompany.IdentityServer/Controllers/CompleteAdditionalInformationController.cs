using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityServer3.Core.Extensions;
using TripCompany.IdentityServer.Models;
using TripCompany.Repository.Entities;
using TripCompany.Repository;


namespace TripCompany.IdentityServer.Controllers
{
    public class CompleteAdditionalInformationController : Controller
    {
        // GET: CompleteAdditionalInformation      
        public async Task<ActionResult> Index()
        {
            // we're only allowed here when we have a partial sign-in
            var ctx = Request.GetOwinContext();
            var partialSignInUser = await ctx.Environment.GetIdentityServerPartialLoginAsync();
            if (partialSignInUser == null)
            {
                return View("No partially signed-in user found.");
            }

            return View(new CompleteAdditionalInformationModel());
        }


        [HttpPost]
        public async Task<ActionResult> Index(CompleteAdditionalInformationModel model)
        {
            // we're only allowed here when we have a partial sign-in
            var ctx = Request.GetOwinContext();
            var partialSignInUser = await ctx.Environment.GetIdentityServerPartialLoginAsync();
            if (partialSignInUser == null)
            {
                return View("No partially signed-in user found.");
            }

            if (ModelState.IsValid)
            {
                using (var userRepository = new UserRepository())
                {
                    // create a user in our user store, including claims & windows as
                    // an external login.

                    // create a new account
                    var newUser = new User();
                    newUser.Subject = Guid.NewGuid().ToString();
                    newUser.IsActive = true;

                    // add the external identity provider as login provider
                    // => external_provider_user_id contains the id/key
                    newUser.UserLogins.Add(new UserLogin()
                    {
                        Subject = newUser.Subject,
                        LoginProvider = "windows",
                        ProviderKey = partialSignInUser.Claims.First(c => c.Type == "external_provider_user_id").Value
                    });

                    // create e-mail claim
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = IdentityServer3.Core.Constants.ClaimTypes.Email,
                        ClaimValue = partialSignInUser.Claims.First(
                          c => c.Type == IdentityServer3.Core.Constants.ClaimTypes.Email).Value
                    });

                    // create claims from the model
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = IdentityServer3.Core.Constants.ClaimTypes.GivenName,
                        ClaimValue = model.FirstName
                    });
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = IdentityServer3.Core.Constants.ClaimTypes.FamilyName,
                        ClaimValue = model.LastName
                    });
                  
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = "role",
                        ClaimValue = model.Role
                    });

                    // add the user             
                    userRepository.AddUser(newUser);

                    // continue where we left off                
                    return Redirect(await ctx.Environment.GetPartialLoginResumeUrlAsync());
                }
            }

            return View();
        }
    }
}