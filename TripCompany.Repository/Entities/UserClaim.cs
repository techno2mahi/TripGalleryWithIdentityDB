using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCompany.Repository.Entities
{
    public class UserClaim
    {
        // requires additional id as a user can have multiple claims
        // of the same type (eg: role)
        public string Id { get; set; }
        public string Subject { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
