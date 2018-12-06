using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Graphity
{
    public class QueryOptions<TContext>
        where TContext : DbContext
    {
        internal List<LambdaExpression> IncludeDbSets { get; set; }

        public QueryOptions()
        {
            IncludeDbSets = new List<LambdaExpression>();
        }

        public void IncludeSet<TProperty>(Expression<Func<TContext, TProperty>> dbSetExpression)
        {
            IncludeDbSets.Add(dbSetExpression);
        }
    }
}