using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning.Operations;

namespace Stenn.AspNetCore.OData.Versioning
{
    public sealed class EdmModelEntityType<TEntityType> : IEdmModelEntityType
        where TEntityType : class
    {
        public EdmModelEntityType(EntityTypeConfiguration commonType, EntityTypeConfiguration<TEntityType> type, EntitySetConfiguration<TEntityType> set)
        {
            CommonType = commonType;
            Type = type;
            Set = set;
        }

        public EntityTypeConfiguration CommonType { get; }
        public EntityTypeConfiguration<TEntityType> Type { get; }
        public EntitySetConfiguration<TEntityType> Set { get; }

        /// <summary>
        /// Sets the base type of this entity type.
        /// </summary>
        /// <typeparam name="TBaseType">The base entity type.</typeparam>
        /// <returns>Returns itself so that multiple calls can be chained.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "typeof(TBaseType) is used and getting it as a generic argument is cleaner")]
        public EdmModelEntityType<TEntityType> DerivesFrom<TBaseType>() where TBaseType : class
        {
            Type.DerivesFrom<TBaseType>();
            return this;
        }

        /// <summary>
        /// Marks this entity type as abstract.
        /// </summary>
        /// <returns>Returns itself so that multiple calls can be chained.</returns>
        public EdmModelEntityType<TEntityType> Abstract()
        {
            Type.Abstract();
            return this;
        }
    }

    public sealed class EdmModelEntityType<TEntityType, TController> : IEdmModelEntityType
        where TEntityType : class
    {
        /// <inheritdoc />
        public EdmModelEntityType(EdmModelEntityType<TEntityType> edmType, IEdmModelOperationExtractor operationExtractor)
            : this(edmType.CommonType, edmType.Type, edmType.Set, operationExtractor)
        {
        }

        public EdmModelEntityType(EntityTypeConfiguration commonType, EntityTypeConfiguration<TEntityType> type, EntitySetConfiguration<TEntityType> set,
            IEdmModelOperationExtractor operationExtractor)
        {
            CommonType = commonType;
            Type = type;
            Set = set;
            OperationExtractor = operationExtractor;
        }

        public EntityTypeConfiguration CommonType { get; }
        public EntityTypeConfiguration<TEntityType> Type { get; }
        public EntitySetConfiguration<TEntityType> Set { get; }
        private IEdmModelOperationExtractor OperationExtractor { get; }

        /// <summary>
        /// Sets the base type of this entity type.
        /// </summary>
        /// <typeparam name="TBaseType">The base entity type.</typeparam>
        /// <returns>Returns itself so that multiple calls can be chained.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "typeof(TBaseType) is used and getting it as a generic argument is cleaner")]
        public EdmModelEntityType<TEntityType, TController> DerivesFrom<TBaseType>() where TBaseType : class
        {
            Type.DerivesFrom<TBaseType>();
            return this;
        }

        /// <summary>
        /// Marks this entity type as abstract.
        /// </summary>
        /// <returns>Returns itself so that multiple calls can be chained.</returns>
        public EdmModelEntityType<TEntityType, TController> Abstract()
        {
            Type.Abstract();
            return this;
        }

        public EdmModelEntityType<TEntityType, TController> AddOperation(Expression<Action<TController>> operationExpression,
            Action<EdmModelOperation<TController>>? init = null)
        {
            OperationExtractor.CreateOperation(Type.ToOperationHolder(), operationExpression, init);
            return this;
        }

        public EdmModelEntityType<TEntityType, TController> AddCollectionOperation(Expression<Action<TController>> operationExpression,
            Action<EdmModelOperation<TController>>? init = null)
        {
            OperationExtractor.CreateOperation(Type.Collection.ToOperationHolder(), operationExpression, init);
            return this;
        }
    }
}
