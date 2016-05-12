using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityServer3.Core.Extensions;
using TripCompany.IdentityServer.Models;
using TripCompany.Repository;
using TripCompany.IdentityServer.Services;

namespace TripCompany.IdentityServer.Controllers
{
    public class TwoFactorAuthenticationController : Controller
    {
        // GET: TwoFactorAuthentication
        public async Task<ActionResult> Index()
        {
            // we're only allowed here when we have a partial sign-in
            var ctx = Request.GetOwinContext();
            var partialSignInUser = await ctx.Environment.GetIdentityServerPartialLoginAsync();
            if (partialSignInUser == null)
            {
                return View("No partially signed-in user found.");
            }

            return View(new TwoFactorAuthenticationModel());
        }

        [HttpPost]
        public async Task<ActionResult> Index(TwoFactorAuthenticationModel model)
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
                using (var twoFactorTokenService = new TwoFactorTokenService())
                {
                    if (twoFactorTokenService.VerifyTwoFactorCodeFor(partialSignInUser.GetSubjectId(), model.Code))
                    {
                        // continue where we left off                
                        return Redirect(await ctx.Environment.GetPartialLoginResumeUrlAsync());
                    }
                    else
                    {
                        // show error
                        return View("This code is invalid.");
                    }
                }
            }

            return View();
        }
    
    }
}