using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace MyCourse.Models.Authorization
{
    public class MultiAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IOptions<AuthorizationOptions> options;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MultiAuthorizationPolicyProvider(IHttpContextAccessor httpContextAccessor, IOptions<AuthorizationOptions> options) : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.options = options;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);
            if (policy != null)
            {
                return policy;
            }

            var policyNames = policyName.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Select(name => name.Trim()).ToArray();
            var builder = new AuthorizationPolicyBuilder();
            builder.RequireAssertion(async (context) =>
            {
                var authService = httpContextAccessor.HttpContext.RequestServices.GetService<IAuthorizationService>();
                foreach (var policyName in policyNames)
                {
                    var result = await authService.AuthorizeAsync(context.User, context.Resource, policyName);
                    if (result.Succeeded)
                    {
                        return true;
                    }
                }

                return false;
            });

            return builder.Build();
        }
    }
}