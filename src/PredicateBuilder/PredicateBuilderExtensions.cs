using System.Collections.Generic;
using PredicateBuilder;

namespace System.Linq.Expressions
{
    public static class PredicateBuilderExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
            => first.Compose(second, Expression.And);

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
            => first.Compose(second, Expression.Or);

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            Dictionary<ParameterExpression, ParameterExpression> map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);
            
            Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }
}