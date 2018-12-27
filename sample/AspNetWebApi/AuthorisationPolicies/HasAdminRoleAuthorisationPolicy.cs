using System.Collections.Generic;
using System.Security.Claims;
using GraphQL.Authorization;

namespace AspNetWebApi.AuthorisationPolicies
{
    public class HasAdminRoleAuthorisationPolicy : IAuthorizationPolicy
    {
        public IEnumerable<IAuthorizationRequirement> Requirements => new[]
        {
            new ClaimAuthorizationRequirement(ClaimTypes.Role, new[] { "Admin" })
        };
    }
}
