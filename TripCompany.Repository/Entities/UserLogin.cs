using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCompany.Repository.Entities
{
    public class UserLogin
    {
        public string Subject { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }

    }
}
