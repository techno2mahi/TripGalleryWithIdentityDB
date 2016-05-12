using System;
namespace TripCompany.Repository
{
    public interface IUserRepository
    {
        void AddUser(TripCompany.Repository.Entities.User user);
        void AddUserClaim(string subject, string claimType, string claimValue);
        void AddUserLogin(string subject, string loginProvider, string providerKey);
        void Dispose();
        TripCompany.Repository.Entities.User GetUser(string subject);
        TripCompany.Repository.Entities.User GetUser(string userName, string password);
        System.Collections.Generic.IList<TripCompany.Repository.Entities.UserClaim> GetUserClaims(string subject);
        TripCompany.Repository.Entities.User GetUserForExternalProvider(string loginProvider, string providerKey);
        TripCompany.Repository.Entities.User GetUserByEmail(string email);
        System.Collections.Generic.IList<TripCompany.Repository.Entities.UserLogin> GetUserLogins(string subject);
    }
}
