using System.Security.Claims;
using GraphQL.Authorization;

namespace Graphity.Authorisation
{
    public class GraphQLUserContext : IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}