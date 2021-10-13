using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.OData.Query;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Represents the default
    ///     <see cref="IODataQueryOptionDescriptionProvider">OData query option description provider.</see>.
    /// </summary>
    public class DefaultODataQueryOptionDescriptionProvider : IODataQueryOptionDescriptionProvider
    {
        private const char Space = ' ';

        /// <inheritdoc />
        public virtual string Describe(AllowedQueryOptions queryOption, ODataQueryOptionDescriptionContext context)
        {
            if (queryOption < AllowedQueryOptions.Filter || queryOption > AllowedQueryOptions.Supported ||
                queryOption != AllowedQueryOptions.Filter && (int)queryOption % 2 != 0)
            {
                throw new ArgumentException(SR.MultipleQueryOptionsNotAllowed, nameof(queryOption));
            }

            return queryOption switch
            {
                AllowedQueryOptions.Filter => DescribeFilter(context),
                AllowedQueryOptions.Expand => DescribeExpand(context),
                AllowedQueryOptions.Select => DescribeSelect(context),
                AllowedQueryOptions.OrderBy => DescribeOrderBy(context),
                AllowedQueryOptions.Top => DescribeTop(context),
                AllowedQueryOptions.Skip => DescribeSkip(context),
                AllowedQueryOptions.Count => DescribeCount(context),
#pragma warning disable CA1308 // Normalize strings to uppercase
                _ => throw new ArgumentException(SR.UnsupportedQueryOption.FormatDefault(queryOption.ToString().ToLowerInvariant()), nameof(queryOption)),
#pragma warning restore CA1308
            };
        }

        /// <summary>
        ///     Describes the $filter query option.
        /// </summary>
        /// <param name="context">The current <see cref="ODataQueryOptionDescriptionContext">description context</see>.</param>
        /// <returns>The query option description.</returns>
        protected virtual string DescribeFilter(ODataQueryOptionDescriptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var description = new StringBuilder();

            description.Append(SR.FilterQueryOptionDesc);

            if (context.MaxNodeCount > 1)
            {
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.MaxExpressionDesc, context.MaxNodeCount);
            }

            AppendAllowedOptions(description, context);

            if (context.AllowedFilterProperties.Count > 0)
            {
                var properties = ToCommaSeparatedValues(context.AllowedFilterProperties);
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.AllowedPropertiesDesc, properties);
            }

            return description.ToString();
        }

        /// <summary>
        ///     Describes the $expand query option.
        /// </summary>
        /// <param name="context">The current <see cref="ODataQueryOptionDescriptionContext">description context</see>.</param>
        /// <returns>The query option description.</returns>
        protected virtual string DescribeExpand(ODataQueryOptionDescriptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var description = new StringBuilder();

            description.Append(SR.ExpandQueryOptionDesc);

            if (context.MaxExpansionDepth > 0)
            {
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.MaxDepthDesc, context.MaxExpansionDepth);
            }

            if (context.AllowedExpandProperties.Count > 0)
            {
                var properties = ToCommaSeparatedValues(context.AllowedExpandProperties);
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.AllowedPropertiesDesc, properties);
            }

            return description.ToString();
        }

        /// <summary>
        ///     Describes the $select query option.
        /// </summary>
        /// <param name="context">The current <see cref="ODataQueryOptionDescriptionContext">description context</see>.</param>
        /// <returns>The query option description.</returns>
        protected virtual string DescribeSelect(ODataQueryOptionDescriptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var description = new StringBuilder();

            description.Append(SR.SelectQueryOptionDesc);

            if (context.AllowedSelectProperties.Count > 0)
            {
                var properties = ToCommaSeparatedValues(context.AllowedSelectProperties);
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.AllowedPropertiesDesc, properties);
            }

            return description.ToString();
        }

        /// <summary>
        ///     Describes the $orderby query option.
        /// </summary>
        /// <param name="context">The current <see cref="ODataQueryOptionDescriptionContext">description context</see>.</param>
        /// <returns>The query option description.</returns>
        protected virtual string DescribeOrderBy(ODataQueryOptionDescriptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var description = new StringBuilder();

            description.Append(SR.OrderByQueryOptionDesc);

            if (context.MaxOrderByNodeCount > 1)
            {
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.MaxExpressionDesc, context.MaxOrderByNodeCount);
            }

            if (context.AllowedOrderByProperties.Count > 0)
            {
                var properties = ToCommaSeparatedValues(context.AllowedOrderByProperties);
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.AllowedPropertiesDesc, properties);
            }

            return description.ToString();
        }

        /// <summary>
        ///     Describes the $top query option.
        /// </summary>
        /// <param name="context">The current <see cref="ODataQueryOptionDescriptionContext">description context</see>.</param>
        /// <returns>The query option description.</returns>
        protected virtual string DescribeTop(ODataQueryOptionDescriptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var description = new StringBuilder();

            description.Append(SR.TopQueryOptionDesc);

            if (context.MaxTop != null && context.MaxTop.Value > 0)
            {
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.MaxValueDesc, context.MaxTop.Value);
            }

            return description.ToString();
        }

        /// <summary>
        ///     Describes the $skip query option.
        /// </summary>
        /// <param name="context">The current <see cref="ODataQueryOptionDescriptionContext">description context</see>.</param>
        /// <returns>The query option description.</returns>
        protected virtual string DescribeSkip(ODataQueryOptionDescriptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var description = new StringBuilder();

            description.Append(SR.SkipQueryOptionDesc);

            if (context.MaxSkip != null && context.MaxSkip.Value > 0)
            {
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.MaxValueDesc, context.MaxSkip.Value);
            }

            return description.ToString();
        }

        /// <summary>
        ///     Describes the $count query option.
        /// </summary>
        /// <param name="context">The current <see cref="ODataQueryOptionDescriptionContext">description context</see>.</param>
        /// <returns>The query option description.</returns>
        protected virtual string DescribeCount(ODataQueryOptionDescriptionContext context)
        {
            return SR.CountQueryOptionDesc;
        }

        private static void AppendAllowedOptions(StringBuilder description, ODataQueryOptionDescriptionContext context)
        {
            if (context.AllowedLogicalOperators != AllowedLogicalOperators.None &&
                context.AllowedLogicalOperators != AllowedLogicalOperators.All)
            {
                var operators = ToCommaSeparatedValues(EnumerateLogicalOperators(context.AllowedLogicalOperators));
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.AllowedLogicalOperatorsDesc, operators);
            }

            if (context.AllowedArithmeticOperators != AllowedArithmeticOperators.None &&
                context.AllowedArithmeticOperators != AllowedArithmeticOperators.All)
            {
                var operators = ToCommaSeparatedValues(EnumerateArithmeticOperators(context.AllowedArithmeticOperators));
                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.AllowedArithmeticOperatorsDesc, operators);
            }

            if (context.AllowedFunctions != AllowedFunctions.None &&
                context.AllowedFunctions != AllowedFunctions.All)
            {
#pragma warning disable CA1308 // Normalize strings to uppercase
                var functions = context.AllowedFunctions.ToString().ToLowerInvariant();
#pragma warning restore CA1308

                description.Append(Space);
                description.AppendFormat(CultureInfo.CurrentCulture, SR.AllowedFunctionsDesc, functions);
            }
        }

        private static IEnumerable<string> EnumerateLogicalOperators(AllowedLogicalOperators logicalOperators)
        {
            if (logicalOperators.HasFlag(AllowedLogicalOperators.Equal))
            {
                yield return "eq";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.NotEqual))
            {
                yield return "ne";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.GreaterThan))
            {
                yield return "gt";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.GreaterThanOrEqual))
            {
                yield return "ge";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.LessThan))
            {
                yield return "lt";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.LessThanOrEqual))
            {
                yield return "le";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.Has))
            {
                yield return "has";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.And))
            {
                yield return "and";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.Or))
            {
                yield return "or";
            }

            if (logicalOperators.HasFlag(AllowedLogicalOperators.Not))
            {
                yield return "not";
            }
        }

        private static IEnumerable<string> EnumerateArithmeticOperators(AllowedArithmeticOperators arithmeticOperators)
        {
            if (arithmeticOperators.HasFlag(AllowedArithmeticOperators.Add))
            {
                yield return "add";
            }

            if (arithmeticOperators.HasFlag(AllowedArithmeticOperators.Subtract))
            {
                yield return "sub";
            }

            if (arithmeticOperators.HasFlag(AllowedArithmeticOperators.Multiply))
            {
                yield return "mul";
            }

            if (arithmeticOperators.HasFlag(AllowedArithmeticOperators.Divide))
            {
                yield return "div";
            }

            if (arithmeticOperators.HasFlag(AllowedArithmeticOperators.Modulo))
            {
                yield return "mod";
            }
        }

        private static string ToCommaSeparatedValues(IEnumerable<string> values)
        {
            using var iterator = values.GetEnumerator();

            if (!iterator.MoveNext())
            {
                return string.Empty;
            }

            var csv = new StringBuilder();

            csv.Append(iterator.Current);

            while (iterator.MoveNext())
            {
                csv.Append(", ");
                csv.Append(iterator.Current);
            }

            return csv.ToString();
        }
    }
}