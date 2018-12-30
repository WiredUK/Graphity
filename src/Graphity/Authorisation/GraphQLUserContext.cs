using System.Security.Claims;
using GraphQL.Authorization;

namespace Graphity.Authorisation
{
    internal class GraphQLUserContext : IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}