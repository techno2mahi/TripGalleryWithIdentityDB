using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCompany.IdentityServer.Services
{
    public class TwoFactorTokenService : IDisposable
    {
        private class TwoFactorCode
        {
            public string Code { get; set; }
            public DateTime CanBeVerifiedUntil { get; set; }
            public bool IsVerified { get; set; }

            public TwoFactorCode(string code)
            {
                Code = code;
                CanBeVerifiedUntil = DateTime.Now.AddMinutes(5);
                IsVerified = false;
            }
        }


        private static Dictionary<string, TwoFactorCode> _twoFactorCodeDictionary
            = new Dictionary<string, TwoFactorCode>();

        public void GenerateTwoFactorCodeFor(string subject)
        {
            // create random code & notify the user 
            // (via sms, mail, or another mechanism)

            // for the demo, we'll use a dummy code
            var code = "123";
            var twoFactorCode = new TwoFactorCode(code);

            // add or overwrite code
            TwoFactorTokenService._twoFactorCodeDictionary[subject] = twoFactorCode;
        }

        public bool VerifyTwoFactorCodeFor(string subject, string code)
        {
            TwoFactorCode twoFactorCodeFromDictionary = null;
            // find subject in dictionary
            if (TwoFactorTokenService._twoFactorCodeDictionary
                .TryGetValue(subject, out twoFactorCodeFromDictionary))
            {
                if (twoFactorCodeFromDictionary.CanBeVerifiedUntil > DateTime.Now
                    && twoFactorCodeFromDictionary.Code == code)
                {
                    twoFactorCodeFromDictionary.IsVerified = true;
                    return true;
                }                 
            }            
            return false;            
        }

        public bool HasVerifiedTwoFactorCode(string subject)
        {
            TwoFactorCode twoFactorCodeFromDictionary = null;
            // find subject in dictionary
            if (TwoFactorTokenService._twoFactorCodeDictionary
                .TryGetValue(subject, out twoFactorCodeFromDictionary))
            {
                return twoFactorCodeFromDictionary.IsVerified;
            }
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // clean up if needed
        }
    }
}
