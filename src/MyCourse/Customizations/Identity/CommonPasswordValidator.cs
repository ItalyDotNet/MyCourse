using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyCourse.Customizations.Identity
{
    public class CommonPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        private readonly string[] commons;
        public CommonPasswordValidator()
        {
            //Elenco di password comuni
            //TODO: impostare un elenco più significativo, magari ottenuto da:
            //https://github.com/danielmiessler/SecLists/tree/master/Passwords/Common-Credentials
            this.commons = new [] {
                "password", "abc", "123", "qwerty"
            };
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            IdentityResult result;
            if (commons.Any(common => password.Contains(common, StringComparison.CurrentCultureIgnoreCase)))
            {
                result = IdentityResult.Failed(new IdentityError { Description = "Password troppo comune" });
            }
            else
            {
                result = IdentityResult.Success;
            }
            return Task.FromResult(result);
        }
    }
}
