using System;
using GraphQL.Authorization;
using Microsoft.AspNetCore.Http;

namespace Graphity.Authorisation
{
    internal class GraphQLSettings
    {
        public Func<HttpContext, GraphQLUserContext> BuildUserContext { get; set; }
    }

    internal class NamedAuthorisationPolicy
    {
        public NamedAuthorisationPolicy(string name, IAuthorizationPolicy authorizationPolicy)
        {
            Name = name;
            AuthorizationPolicy = authorizationPolicy;
        }

        public string Name { get; }
        public IAuthorizationPolicy AuthorizationPolicy { get; }
    }
}
