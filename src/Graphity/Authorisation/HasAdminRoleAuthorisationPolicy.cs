using System.Collections.Generic;
using GraphQL.Authorization;

namespace Graphity.Authorisation
{
    internal class HasClaimAuthorisationPolicy : IAuthorizationPolicy
    {
        private readonly string _claimType;
        private readonly string[] _roles;

        public HasClaimAuthorisationPolicy(string claimType, params string[] roles)
        {
            _claimType = claimType;
            _roles = roles;
        }

        public IEnumerable<IAuthorizationRequirement> Requirements => new[]
        {
            new ClaimAuthorizationRequirement(_claimType, _roles)
        };
    }
}
