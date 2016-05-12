using IdentityServer3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TripCompany.IdentityServer.Models;
using TripCompany.Repository;
using TripCompany.Repository.Entities;

namespace TripCompany.IdentityServer.Controllers
{
    public class CreateUserAccountController : Controller
    {
        // GET: CreateUserAccount
        [HttpGet]
        public ActionResult Index(string signin)
        {
            return View(new CreateUserAccountModel());
        }

        [HttpPost]
        public ActionResult Index(string signin, CreateUserAccountModel model)
        {
            if (ModelState.IsValid)
            {
                 using (var userRepository = new UserRepository())
                {
                    // create a user in our user store, including claims 

                    // create a new account
                    var newUser = new User();
                    newUser.Subject = Guid.NewGuid().ToString();
                    newUser.IsActive = true;
                    newUser.UserName = model.UserName;
                    newUser.Password = model.Password;
 

                    // create claims from the model
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = Constants.ClaimTypes.Email,
                        ClaimValue = model.Email
                    });                
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = Constants.ClaimTypes.GivenName,
                        ClaimValue = model.FirstName
                    });
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = Constants.ClaimTypes.FamilyName,
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

                    // redirect to the login page, passing in 
                    // the signin parameter
                    return Redirect("~/identity/" + Constants.RoutePaths.Login + "?signin=" + signin);
                }
            } 
            return View();
        }
    }
}