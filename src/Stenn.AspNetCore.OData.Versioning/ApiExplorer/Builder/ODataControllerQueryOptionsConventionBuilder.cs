using System;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Represents an OData query options convention builder.
    /// </summary>
    public class ODataControllerQueryOptionsConventionBuilder : IODataQueryOptionsConventionBuilder, IODataActionQueryOptionsConventionBuilder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ODataControllerQueryOptionsConventionBuilder" /> class.
        /// </summary>
        /// <param name="controllerType">The <see cref="Type">type</see> of controller the convention builder is for.</param>
        public ODataControllerQueryOptionsConventionBuilder(Type controllerType)
        {
            ControllerType = controllerType;
            ActionBuilders = new ODataActionQueryOptionsConventionBuilderCollection(this);
        }

        /// <summary>
        ///     Gets a collection of controller action convention builders.
        /// </summary>
        /// <value>
        ///     A <see cref="ODataActionQueryOptionsConventionBuilderCollection">collection</see> of
        ///     <see cref="ODataActionQueryOptionsConventionBuilder">controller action convention builders</see>.
        /// </value>
        protected internal virtual ODataActionQueryOptionsConventionBuilderCollection ActionBuilders { get; }

        /// <summary>
        ///     Gets the type of controller the convention builder is for.
        /// </summary>
        /// <value>The corresponding controller <see cref="Type">type</see>.</value>
        public Type ControllerType { get; }

        /// <summary>
        ///     Gets or creates the convention builder for the specified controller action method.
        /// </summary>
        /// <param name="actionMethod">The <see cref="MethodInfo">method</see> representing the controller action.</param>
        /// <returns>A new or existing <see cref="ODataActionQueryOptionsConventionBuilder" />.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ODataActionQueryOptionsConventionBuilder Action(MethodInfo actionMethod)
        {
            return ActionBuilders.GetOrAdd(actionMethod);
        }

        /// <inheritdoc />
        public virtual IODataQueryOptionsConvention Build(ODataQueryOptionSettings settings)
        {
            return new ODataControllerQueryOptionConvention(Lookup, settings);
        }

        private bool Lookup(MethodInfo action, ODataQueryOptionSettings settings, out IODataQueryOptionsConvention? convention)
        {
            if (ActionBuilders.TryGetValue(action, out var builder))
            {
                convention = builder!.Build(settings);
                return true;
            }

            convention = default;
            return false;
        }
    }
}