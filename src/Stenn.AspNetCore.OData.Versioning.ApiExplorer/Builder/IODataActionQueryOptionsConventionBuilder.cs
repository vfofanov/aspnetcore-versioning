using System;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
    /// <summary>
    ///     Defines the behavior of an OData query options convention builder for an action.
    /// </summary>
    public interface IODataActionQueryOptionsConventionBuilder
    {
        /// <summary>
        ///     Gets the type of controller the convention builder is for.
        /// </summary>
        /// <value>The corresponding controller <see cref="Type">type</see>.</value>
        Type ControllerType { get; }

        /// <summary>
        ///     Gets or creates a convention builder for the specified controller action method.
        /// </summary>
        /// <param name="actionMethod">
        ///     The controller action <see cref="MethodInfo">method</see>
        ///     to get or create a convention for.
        /// </param>
        /// <returns>A new or existing <see cref="ODataActionQueryOptionsConventionBuilder" />.</returns>
        ODataActionQueryOptionsConventionBuilder Action(MethodInfo actionMethod);
    }
}