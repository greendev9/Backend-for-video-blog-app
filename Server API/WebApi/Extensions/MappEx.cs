using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApi.Extensions
{
    public static class MappEx
    {

        public static void MapFromIfNotNull<TSource, TDestination, TProperty>(
        this IMemberConfigurationExpression<TSource, TDestination, TProperty> map,
        Expression<Func<TSource, object>> selector)
        {
            var function = selector.Compile();
            map.Condition(source => function(source) != null);
            map.MapFrom(selector);
        }


    }
}
