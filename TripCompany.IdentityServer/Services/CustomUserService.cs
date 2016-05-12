using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripCompany.Repository;
using TripCompany.Repository.Entities;
using IdentityServer3.Core.Extensions;
using System.Security.Claims;
using Microsoft.Owin;
using IdentityServer3.Core.Services;

namespace TripCompany.IdentityServer.Services
{
    public class CustomUserService : UserServiceBase
    {
      
        
        public CustomUserService()
        {

        } 
        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            using (var userRepository = new UserRepository())
            {
                // is the external provider already linked to an account?
                var existingLinkedUser = userRepository.GetUserForExternalProvider(context.ExternalIdentity.Provider,
                     context.ExternalIdentity.ProviderId);

                // it is - set as authentication result;
                if (existingLinkedUser != null)
                {
                    context.AuthenticateResult = new AuthenticateResult(
                        existingLinkedUser.Subject,
                        existingLinkedUser.UserClaims.First(c => c.ClaimType == Constants.ClaimTypes.GivenName).ClaimValue,
                        existingLinkedUser.UserClaims.Select<UserClaim, Claim>(uc => new Claim(uc.ClaimType, uc.ClaimValue)),
                        authenticationMethod: Constants.AuthenticationMethods.External,
                        identityProvider: context.ExternalIdentity.Provider);

                    return Task.FromResult(0);
                }               

                // no existing link, get email claim to match user
                var emailClaim = context.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == "email");
                if (emailClaim == null)
                {
                    // return error - we need an email claim to match
                    context.AuthenticateResult = new AuthenticateResult("No email claim available.");
                    return Task.FromResult(0);
                }

                // find a user with a matching e-mail claim.  
                var userWithMatchingEmailClaim = userRepository.GetUserByEmail(emailClaim.Value);
                
                if (userWithMatchingEmailClaim == null && context.ExternalIdentity.Provider == "windows")
                {             
                    // no existing link.  If it's a windows user, we're going to ask for additional
                    // information.
                    context.AuthenticateResult = 
                        new AuthenticateResult("~/completeadditionalinformation", context.ExternalIdentity);
                    return Task.FromResult(0);
                }
             

                if (userWithMatchingEmailClaim == null)
                {
                    //// return error - we need an existing account
                    //context.AuthenticateResult = new AuthenticateResult("No existing account found.");
                    //return Task.FromResult(0);    

                    // create a new account
                    var newUser = new User();
                    newUser.Subject = Guid.NewGuid().ToString();
                    newUser.IsActive = true;

                    // add the external identity provider as login provider
                    newUser.UserLogins.Add(new UserLogin()
                        {
                            Subject = newUser.Subject,
                            LoginProvider = context.ExternalIdentity.Provider,
                            ProviderKey = context.ExternalIdentity.ProviderId
                        });

                    // create a list of claims from the information we got from the external provider
                    // this can be provider-specific
                    if (context.ExternalIdentity.Provider.ToLowerInvariant() == "facebook")
                    {
                        newUser.UserClaims = context.ExternalIdentity
                            .Claims.Where(c =>
                               c.Type.ToLowerInvariant() == Constants.ClaimTypes.GivenName
                            || c.Type.ToLowerInvariant() == Constants.ClaimTypes.FamilyName
                            || c.Type.ToLowerInvariant() == Constants.ClaimTypes.Email)
                            .Select<Claim, UserClaim>(c => new UserClaim()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Subject = newUser.Subject,
                                ClaimType = c.Type.ToLowerInvariant(),
                                ClaimValue = c.Value
                            }).ToList();
                    }

                    // create a new user with the FreeUser role by default
                    newUser.UserClaims.Add(new UserClaim()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Subject = newUser.Subject,
                        ClaimType = "role",
                        ClaimValue = "FreeUser"
                    });

                    // add the user
                    userRepository.AddUser(newUser);

                    // use this new user
                    context.AuthenticateResult = new AuthenticateResult(
                       newUser.Subject,
                       newUser.UserClaims.First(c => c.ClaimType == Constants.ClaimTypes.GivenName).ClaimValue,
                       newUser.UserClaims.Select<UserClaim, Claim>(uc => new Claim(uc.ClaimType, uc.ClaimValue)),
                       authenticationMethod: Constants.AuthenticationMethods.External,
                       identityProvider: context.ExternalIdentity.Provider);

                    return Task.FromResult(0);
                }

                // register this external provider for this user account
                userRepository.AddUserLogin(userWithMatchingEmailClaim.Subject,
                    context.ExternalIdentity.Provider,
                    context.ExternalIdentity.ProviderId);

                // use this existing account
                context.AuthenticateResult = new AuthenticateResult(
                       userWithMatchingEmailClaim.Subject,
                       userWithMatchingEmailClaim.UserClaims.First(c => c.ClaimType == Constants.ClaimTypes.GivenName).ClaimValue,
                       userWithMatchingEmailClaim.UserClaims.Select<UserClaim, Claim>(uc => new Claim(uc.ClaimType, uc.ClaimValue)),
                       authenticationMethod: Constants.AuthenticationMethods.External,
                       identityProvider: context.ExternalIdentity.Provider);

                return Task.FromResult(0);
            }
        }


        public override Task AuthenticateLocalAsync(IdentityServer3.Core.Models.LocalAuthenticationContext context)
        {
            using (var userRepository = new UserRepository())
            {
                // get user from repository
                var user = userRepository.GetUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.AuthenticateResult = new AuthenticateResult("Invalid credentials");
                    return Task.FromResult(0);
                }

                context.AuthenticateResult = new AuthenticateResult(
                    user.Subject,
                    user.UserClaims.First(c => c.ClaimType == Constants.ClaimTypes.GivenName).ClaimValue);

                return Task.FromResult(0);
            }
        }
         
        public override Task GetProfileDataAsync(IdentityServer3.Core.Models.ProfileDataRequestContext context)
        {
            using (var userRepository = new UserRepository())
            {
                // find the user
                var user = userRepository.GetUser(context.Subject.GetSubjectId());

                // add subject as claim
                var claims = new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.Subject, user.Subject),
                };

                // add the other UserClaims
                claims.AddRange(user.UserClaims.Select<UserClaim, Claim>(
                    uc => new Claim(uc.ClaimType, uc.ClaimValue)));

                // only return the requested claims
                if (!context.AllClaimsRequested)
                {
                    claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
                }

                // set the issued claims - these are the ones that were requested, if available
                context.IssuedClaims = claims;

                return Task.FromResult(0);
            }
        }

        public override Task IsActiveAsync(IdentityServer3.Core.Models.IsActiveContext context)
        {
            using (var userRepository = new UserRepository())
            {
                if (context.Subject == null)
                {
                    throw new ArgumentNullException("subject");
                }

                var user = userRepository.GetUser(context.Subject.GetSubjectId());

                // set whether or not the user is active
                context.IsActive = (user != null) && user.IsActive;

                return Task.FromResult(0);
            }
        }
 

        public override Task PostAuthenticateAsync(PostAuthenticationContext context)
        {
            // we're logged in to identity server - but it might be that the application
            // requires 2FA.  

            // if we don't require 2FA, return & continue
            var twoFactorRequired = context.SignInMessage.AcrValues.Any(v => v == "2fa");
            if (!twoFactorRequired)
            {
                return Task.FromResult(0);
            }

            // we require 2FA
            using (var twoFactorTokenService = new TwoFactorTokenService())
            {               
                if (twoFactorTokenService.HasVerifiedTwoFactorCode(context.AuthenticateResult.User.GetSubjectId()))
                {
                    return Task.FromResult(0);
                }
                else
                {
                    // the user hasn't inputted a (valid) 2FA code.  Generate one, and redirect
                    twoFactorTokenService.GenerateTwoFactorCodeFor(context.AuthenticateResult.User.GetSubjectId());

                    context.AuthenticateResult =
                         new AuthenticateResult("~/twofactorauthentication", context.AuthenticateResult.User.GetSubjectId(),
                             context.AuthenticateResult.User.GetName(), context.AuthenticateResult.User.Claims);
                    return Task.FromResult(0);
                }
            }
        }
    }
}
