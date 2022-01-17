using System;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public static class EdmModelOperationConfigurationExtensions
    {
        public static IEdmModelOperationHolder ToOperationHolder<TEntityType>(this EntityTypeConfiguration<TEntityType> holder) 
            where TEntityType : class
        {
            return new EntityTypeConfigurationOperationHolder<TEntityType>(holder);
        }

        public static IEdmModelOperationHolder ToOperationHolder<TEntityType>(this EntityCollectionConfiguration<TEntityType> holder)
            where TEntityType : class
        {
            return new EntityCollectionConfigurationOperationHolder<TEntityType>(holder);
        }

        public static IEdmModelOperationHolder ToOperationHolder(this ODataConventionModelBuilder holder)
        {
            return new ODataConventionModelBuilderOperationHolder(holder);
        }

        private sealed class EntityTypeConfigurationOperationHolder<TEntityType> : IEdmModelOperationHolder
            where TEntityType : class
        {
            private readonly EntityTypeConfiguration<TEntityType> _holder;

            public EntityTypeConfigurationOperationHolder(EntityTypeConfiguration<TEntityType> holder)
            {
                _holder = holder;
            }

            /// <inheritdoc />
            public Type ClrType => typeof(TEntityType);

            /// <inheritdoc />
            public ActionConfiguration Action(string name)
            {
                return _holder.Action(name);
            }

            /// <inheritdoc />
            public FunctionConfiguration Function(string name)
            {
                return _holder.Function(name);
            }
        }

        private sealed class EntityCollectionConfigurationOperationHolder<TEntityType> : IEdmModelOperationHolder
            where TEntityType : class
        {
            private readonly EntityCollectionConfiguration<TEntityType> _holder;

            public EntityCollectionConfigurationOperationHolder(EntityCollectionConfiguration<TEntityType> holder)
            {
                _holder = holder;
            }

            /// <inheritdoc />
            public Type ClrType => typeof(TEntityType);

            /// <inheritdoc />
            public ActionConfiguration Action(string name)
            {
                return _holder.Action(name);
            }

            /// <inheritdoc />
            public FunctionConfiguration Function(string name)
            {
                return _holder.Function(name);
            }
        }

        private sealed class ODataConventionModelBuilderOperationHolder : IEdmModelOperationHolder
        {
            private readonly ODataConventionModelBuilder _holder;

            public ODataConventionModelBuilderOperationHolder(ODataConventionModelBuilder holder)
            {
                _holder = holder;
            }

            /// <inheritdoc />
            public Type? ClrType => null;

            /// <inheritdoc />
            public ActionConfiguration Action(string name)
            {
                return _holder.Action(name);
            }

            /// <inheritdoc />
            public FunctionConfiguration Function(string name)
            {
                return _holder.Function(name);
            }
        }
    }
}