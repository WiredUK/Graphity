using System;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace Graphity
{
    internal class ScopedFuncDependencyResolver : FuncDependencyResolver, IScopedDependencyResolver
    {
        private readonly Func<IServiceScope> _createScope;

        public ScopedFuncDependencyResolver(Func<Type, object> resolver, Func<IServiceScope> createScope) : base(resolver)
        {
            _createScope = createScope;
        }

        public IServiceScope CreateScope()
        {
            return _createScope();
        }
    }
}