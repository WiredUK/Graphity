using System;
using System.Threading.Tasks;
using GraphQL.Authorization;

namespace Graphity.Authorisation
{
    internal class FuncAuthorisationRequirement : IAuthorizationRequirement
    {
        private readonly Func<Task<AuthorisationResult>> _authorise;

        public FuncAuthorisationRequirement(Func<Task<AuthorisationResult>> authorise)
        {
            _authorise = authorise;
        }

        public async Task Authorize(AuthorizationContext context)
        {
            var authResult = await _authorise();

            if (authResult.Successful)
            {
                return;
            }

            context.ReportError(authResult.ErrorMessage);
        }
    }
}