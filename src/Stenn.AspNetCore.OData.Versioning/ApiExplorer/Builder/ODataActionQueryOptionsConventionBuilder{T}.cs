﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Represents an OData controller action query options convention builder.
    /// </summary>
#pragma warning disable SA1619 // Generic type parameters should be documented partial class; false positive
    public class ODataActionQueryOptionsConventionBuilder<T> : ODataActionQueryOptionsConventionBuilderBase, IODataActionQueryOptionsConventionBuilder<T>
        where T : notnull
#pragma warning restore SA1619
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ODataActionQueryOptionsConventionBuilder{T}" /> class.
        /// </summary>
        /// <param name="controllerBuilder">
        ///     The <see cref="ODataActionQueryOptionsConventionBuilder{T}">controller builder</see>
        ///     the action builder belongs to.
        /// </param>
        public ODataActionQueryOptionsConventionBuilder(ODataControllerQueryOptionsConventionBuilder<T> controllerBuilder)
        {
            ControllerBuilder = controllerBuilder;
        }

        /// <summary>
        ///     Gets the controller builder the action builder belongs to.
        /// </summary>
        /// <value>The associated <see cref="ODataControllerQueryOptionsConventionBuilder{T}" />.</value>
        protected ODataControllerQueryOptionsConventionBuilder<T> ControllerBuilder { get; }

        /// <summary>
        ///     Gets or creates the convention builder for the specified controller action method.
        /// </summary>
        /// <param name="actionMethod">The <see cref="MethodInfo">method</see> representing the controller action.</param>
        /// <returns>A new or existing <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ODataActionQueryOptionsConventionBuilder<T> Action(MethodInfo actionMethod)
        {
            return ControllerBuilder.Action(actionMethod);
        }

        /// <summary>
        ///     Uses the specified validation settings for the convention.
        /// </summary>
        /// <param name="validationSettings">The <see cref="ODataValidationSettings">validation settings</see> to use.</param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> Use(ODataValidationSettings validationSettings)
        {
            if (validationSettings == null)
            {
                throw new ArgumentNullException(nameof(validationSettings));
            }

            ValidationSettings.CopyFrom(validationSettings);
            return this;
        }

        /// <summary>
        ///     Allows the specified arithmetic operators.
        /// </summary>
        /// <param name="arithmeticOperators">
        ///     One or more <see cref="AllowedArithmeticOperators">allowed arithmetic operators</see>
        ///     .
        /// </param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> Allow(AllowedArithmeticOperators arithmeticOperators)
        {
            ValidationSettings.AllowedArithmeticOperators |= arithmeticOperators;
            return this;
        }

        /// <summary>
        ///     Allows the specified functions.
        /// </summary>
        /// <param name="functions">One or more <see cref="AllowedFunctions">allowed functions</see>.</param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> Allow(AllowedFunctions functions)
        {
            ValidationSettings.AllowedFunctions |= functions;
            return this;
        }

        /// <summary>
        ///     Allows the specified logical operators.
        /// </summary>
        /// <param name="logicalOperators">One or more <see cref="AllowedLogicalOperators">allowed logical operators</see>.</param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> Allow(AllowedLogicalOperators logicalOperators)
        {
            ValidationSettings.AllowedLogicalOperators |= logicalOperators;
            return this;
        }

        /// <summary>
        ///     Allows the specified query options.
        /// </summary>
        /// <param name="queryOptions">One or more <see cref="AllowedQueryOptions">allowed query options</see>.</param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> Allow(AllowedQueryOptions queryOptions)
        {
            ValidationSettings.AllowedQueryOptions |= queryOptions;
            return this;
        }

        /// <summary>
        ///     Allows the $skip query option.
        /// </summary>
        /// <param name="max">The maximum value of the $skip query option or zero to indicate no maximum.</param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> AllowSkip(int max)
        {
            ValidationSettings.AllowedQueryOptions |= AllowedQueryOptions.Skip;

            if (max != default)
            {
                ValidationSettings.MaxSkip = max;
            }

            return this;
        }

        /// <summary>
        ///     Allows the $top query option.
        /// </summary>
        /// <param name="max">The maximum value of the $top query option or zero to indicate no maximum.</param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> AllowTop(int max)
        {
            ValidationSettings.AllowedQueryOptions |= AllowedQueryOptions.Top;

            if (max != default)
            {
                ValidationSettings.MaxTop = max;
            }

            return this;
        }

        /// <summary>
        ///     Allows the $expand query option.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of the $expand query option or zero to indicate the default.</param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> AllowExpand(int maxDepth)
        {
            ValidationSettings.AllowedQueryOptions |= AllowedQueryOptions.Expand;

            if (maxDepth != default)
            {
                ValidationSettings.MaxExpansionDepth = maxDepth;
            }

            return this;
        }

        /// <summary>
        ///     Allows the 'Any' and 'All' functions in the $filter query option.
        /// </summary>
        /// <param name="maxExpressionDepth">
        ///     The maximum expression depth of the 'Any' or 'All' function in a query or zero to
        ///     indicate the default.
        /// </param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> AllowAnyAll(int maxExpressionDepth)
        {
            ValidationSettings.AllowedFunctions |= AllowedFunctions.Any | AllowedFunctions.All;
            ValidationSettings.AllowedQueryOptions |= AllowedQueryOptions.Filter;

            if (maxExpressionDepth != default)
            {
                ValidationSettings.MaxAnyAllExpressionDepth = maxExpressionDepth;
            }

            return this;
        }

        /// <summary>
        ///     Allows the $filter query option.
        /// </summary>
        /// <param name="maxNodeCount">The maximum number of nodes in the $filter query option or zero to indicate the default.</param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> AllowFilter(int maxNodeCount)
        {
            ValidationSettings.AllowedQueryOptions |= AllowedQueryOptions.Filter;

            if (maxNodeCount != default)
            {
                ValidationSettings.MaxNodeCount = maxNodeCount;
            }

            return this;
        }

        /// <summary>
        ///     Allows the $orderby query option.
        /// </summary>
        /// <param name="maxNodeCount">
        ///     The maximum number of expressions in the $orderby query option or zero to indicate the
        ///     default.
        /// </param>
        /// <param name="properties">
        ///     The <see cref="IEnumerable{T}">sequence</see> of property names that can appear in the $orderby query option.
        ///     An empty sequence indicates that any property can appear in the $orderby query option.
        /// </param>
        /// <returns>The original <see cref="ODataActionQueryOptionsConventionBuilder{T}" />.</returns>
        public virtual ODataActionQueryOptionsConventionBuilder<T> AllowOrderBy(int maxNodeCount, IEnumerable<string> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            ValidationSettings.AllowedQueryOptions |= AllowedQueryOptions.OrderBy;

            if (maxNodeCount != default)
            {
                ValidationSettings.MaxOrderByNodeCount = maxNodeCount;
            }

            foreach (var property in properties)
            {
                ValidationSettings.AllowedOrderByProperties.Add(property);
            }

            return this;
        }
    }
}