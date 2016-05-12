using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace TripCompany.IdentityServer.Models
{
    public class CreateUserAccountModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName {get;set;}
        public string Password {get;set;}
        public string Email {get;set;}
        public string Role { get; set; }

        public List<SelectListItem> Roles { get; set; }
        public CreateUserAccountModel()
        {
            Roles = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Free version", Value = "FreeUser"},
                new SelectListItem() { Text = "Plus version", Value = "PayingUser"}
            };
        }
    }
}
