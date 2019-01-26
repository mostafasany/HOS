using System;
using System.Reflection;
using AutoMapper;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class BaseMapping
    {
        public static TResult GetWithDefault<TSource, TResult>(this TSource instance,
            Func<TSource, TResult> getter,
            TResult defaultValue = default(TResult))
            where TSource : class => instance != null ? getter(instance) : defaultValue;

        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            Type sourceType = typeof(TSource);
            PropertyInfo[] destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (PropertyInfo property in destinationProperties)
                if (sourceType.GetProperty(property.Name, flags) == null)
                    expression.ForMember(property.Name, opt => opt.Ignore());
            return expression;
        }
    }
}