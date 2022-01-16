using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public static class EdmModelOperationConfigurationExtensions
    {
        public static IEdmModelOperationHolder ToOperationHolder<T>(this EntityTypeConfiguration<T> holder) 
            where T : class
        {
            return new EntityTypeConfigurationOperationHolder<T>(holder);
        }

        public static IEdmModelOperationHolder ToOperationHolder<T>(this EntityCollectionConfiguration<T> holder)
            where T : class
        {
            return new EntityCollectionConfigurationOperationHolder<T>(holder);
        }

        public static IEdmModelOperationHolder ToOperationHolder(this ODataConventionModelBuilder holder)
        {
            return new ODataConventionModelBuilderOperationHolder(holder);
        }

        private sealed class EntityTypeConfigurationOperationHolder<T> : IEdmModelOperationHolder
            where T : class
        {
            private readonly EntityTypeConfiguration<T> _holder;

            public EntityTypeConfigurationOperationHolder(EntityTypeConfiguration<T> holder)
            {
                _holder = holder;
            }

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

        private sealed class EntityCollectionConfigurationOperationHolder<T> : IEdmModelOperationHolder
            where T : class
        {
            private readonly EntityCollectionConfiguration<T> _holder;

            public EntityCollectionConfigurationOperationHolder(EntityCollectionConfiguration<T> holder)
            {
                _holder = holder;
            }

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