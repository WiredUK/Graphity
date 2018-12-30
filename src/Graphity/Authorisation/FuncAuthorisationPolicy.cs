using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Authorization;

namespace Graphity.Authorisation
{
    internal class FuncAuthorisationPolicy : IAuthorizationPolicy
    {
        private readonly Func<Task<AuthorisationResult>> _authorise;

        public FuncAuthorisationPolicy(Func<Task<AuthorisationResult>> authorise)
        {
            _authorise = authorise;
        }

        public IEnumerable<IAuthorizationRequirement> Requirements => new[]
        {
            new FuncAuthorisationRequirement(_authorise)
        };
    }
}