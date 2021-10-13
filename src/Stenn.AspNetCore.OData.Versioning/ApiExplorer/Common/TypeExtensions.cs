using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.OData;

namespace Microsoft.AspNet.OData
{
    /// <summary>
    ///     Provides extension methods for the <see cref="Type" /> class.
    /// </summary>
    public static partial class TypeExtensions
    {
        private static readonly Type HttpResponseType = typeof(HttpResponseMessage);
        private static readonly Type EnumerableOfT = typeof(IEnumerable<>);
        private static readonly Type ODataValue = typeof(ODataValue);
        private static readonly Type SingleResultOfT = typeof(SingleResult<>);
        private static readonly Type ActionResultType = typeof(IActionResult);
        private static readonly Type ActionResultOfT = typeof(ActionResult<>);

        /// <summary>
        ///     Substitutes the specified type, if required.
        /// </summary>
        /// <param name="type">The <see cref="Type">type</see> to be evaluated.</param>
        /// <param name="context">The current <see cref="TypeSubstitutionContext">type substitution context</see>.</param>
        /// <returns>
        ///     The original <paramref name="type" /> or a substitution <see cref="Type">type</see> based on the
        ///     provided <paramref name="context" />.
        /// </returns>
        public static Type SubstituteIfNecessary(this Type type, TypeSubstitutionContext context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var openTypes = new Stack<Type>();
            var apiVersion = context.ApiVersion;
            var resolver = new StructuredTypeResolver(context.Model);

            if (IsSubstitutableGeneric(type, openTypes, out var innerType))
            {
                var structuredType = resolver.GetStructuredType(innerType!);

                if (structuredType == null)
                {
                    return type;
                }

                var newType = context.ModelTypeBuilder.NewStructuredType(structuredType, innerType!, apiVersion, context.Model);

                if (innerType! == newType)
                {
                    return type.ShouldExtractInnerType() ? innerType! : type;
                }

                return CloseGeneric(openTypes, newType);
            }

            if (CanBeSubstituted(type))
            {
                var structuredType = resolver.GetStructuredType(type);

                if (structuredType != null)
                {
                    type = context.ModelTypeBuilder.NewStructuredType(structuredType, type, apiVersion, context.Model);
                }
            }

            return type;
        }

        internal static IEnumerable<CustomAttributeBuilder> DeclaredAttributes(this MemberInfo member)
        {
            foreach (var attribute in member.CustomAttributes)
            {
                var ctor = attribute.Constructor;
                var ctorArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedProperties = new List<PropertyInfo>(attribute.NamedArguments.Count);
                var propertyValues = new List<object>(attribute.NamedArguments.Count);
                var namedFields = new List<FieldInfo>(attribute.NamedArguments.Count);
                var fieldValues = new List<object>(attribute.NamedArguments.Count);

                foreach (var argument in attribute.NamedArguments)
                {
                    if (argument.IsField)
                    {
                        namedFields.Add((FieldInfo)argument.MemberInfo);
                        fieldValues.Add(argument.TypedValue.Value!);
                    }
                    else
                    {
                        namedProperties.Add((PropertyInfo)argument.MemberInfo);
                        propertyValues.Add(argument.TypedValue.Value!);
                    }
                }

                for (var i = 0; i < ctorArgs.Length; i++)
                {
                    if (ctorArgs[i] is IReadOnlyCollection<CustomAttributeTypedArgument> paramsList)
                    {
                        ctorArgs[i] = paramsList.Select(a => a.Value).ToArray();
                    }
                }

                yield return new CustomAttributeBuilder(
                    ctor,
                    ctorArgs,
                    namedProperties.ToArray(),
                    propertyValues.ToArray(),
                    namedFields.ToArray(),
                    fieldValues.ToArray());
            }
        }

        internal static void Deconstruct<T1, T2>(this Tuple<T1, T2> tuple, out T1 item1, out T2 item2)
        {
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }

        internal static Type ExtractInnerType(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type;
            }

            var typeDef = type.GetGenericTypeDefinition();
            var typeArgs = type.GetGenericArguments();

            if (typeArgs.Length != 1)
            {
                return type;
            }

            var typeArg = typeArgs[0];

            if (typeDef.IsDelta() || typeDef.IsODataValue() || typeDef.IsSingleResult() || typeDef.IsActionResult())
            {
                return typeArg;
            }

            return type;
        }

        private static bool IsSubstitutableGeneric(Type type, Stack<Type> openTypes, out Type? innerType)
        {
            innerType = default;

            if (!type.IsGenericType)
            {
                return false;
            }

            var typeDef = type.GetGenericTypeDefinition();
            var typeArgs = type.GetGenericArguments();

            if (typeArgs.Length != 1)
            {
                return false;
            }

            openTypes.Push(typeDef);

            var typeArg = typeArgs[0];

            if (typeDef == EnumerableOfT || typeDef.IsDelta() || typeDef.IsODataValue() || typeDef.IsSingleResult() || typeDef.IsActionResult())
            {
                innerType = typeArg;
            }
            else
            {
                foreach (var @interface in type.GetInterfaces())
                {
                    if (@interface.IsEnumerable(out innerType))
                    {
                        break;
                    }
                }
            }

            if (innerType == null)
            {
                return false;
            }

            // examples: ODataValue<IEnumerable<Entity>>, ActionResult<IEnumerable<Entity>>
            while (innerType.IsEnumerable(out var nextType))
            {
                openTypes.Push(EnumerableOfT);
                innerType = nextType!;
            }

            return true;
        }

        private static Type CloseGeneric(Stack<Type> openTypes, Type innerType)
        {
            var type = openTypes.Pop();

            if (type.ShouldExtractInnerType())
            {
                return innerType;
            }

            type = type.MakeGenericType(innerType);

            while (openTypes.Count > 0)
            {
                type = openTypes.Pop().MakeGenericType(type);
            }

            return type;
        }

        private static bool CanBeSubstituted(Type type)
        {
            return Type.GetTypeCode(type) == TypeCode.Object &&
                   !type.IsValueType &&
                   type != ActionResultType &&
                   type != HttpResponseType &&
                   !type.IsODataActionParameters();
        }

        internal static bool IsEnumerable(this Type type, out Type? itemType)
        {
            itemType = default;

            if (!type.IsGenericType)
            {
                return false;
            }

            var typeDef = type.GetGenericTypeDefinition();

            if (typeDef == EnumerableOfT)
            {
                itemType = type.GetGenericArguments()[0];
                return true;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                if (@interface.IsEnumerable(out itemType))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSingleResult(this Type type)
        {
            return type.Is(SingleResultOfT);
        }

        private static bool IsODataValue(this Type? type)
        {
            while (type != null)
            {
                if (type == ODataValue)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private static bool Is(this Type type, Type typeDefinition)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeDefinition;
        }

        private static bool ShouldExtractInnerType(this Type type)
        {
            return type.IsDelta() || type.IsSingleResult() || type.IsActionResult();
        }

        private static bool IsActionResult(this Type type)
        {
            return type.Is(ActionResultOfT);
        }
    }
}