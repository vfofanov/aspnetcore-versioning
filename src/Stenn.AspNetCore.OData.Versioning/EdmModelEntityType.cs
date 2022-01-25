using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning.Operations;

namespace Stenn.AspNetCore.OData.Versioning
{
    public sealed class EdmModelEntityType<TEntityType> : IEdmModelEntityType
        where TEntityType : class
    {
        public EdmModelEntityType(EntityTypeConfiguration commonType, EntityTypeConfiguration<TEntityType> type, EntitySetConfiguration<TEntityType> set, IEdmModelBuilderContext context)
        {
            CommonType = commonType;
            Type = type;
            Set = set;
            Context = context;
        }

        public EntityTypeConfiguration<TEntityType> Type { get; }
        public EntitySetConfiguration<TEntityType> Set { get; }
        public IEdmModelBuilderContext Context { get; }

        public EntityTypeConfiguration CommonType { get; }

        /// <summary>
        ///     Sets the base type of this entity type.
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
        ///     Marks this entity type as abstract.
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
            : this(edmType.CommonType, edmType.Type, edmType.Set, edmType.Context, operationExtractor)
        {
        }

        public EdmModelEntityType(EntityTypeConfiguration commonType, EntityTypeConfiguration<TEntityType> type, EntitySetConfiguration<TEntityType> set,
            IEdmModelBuilderContext context, IEdmModelOperationExtractor operationExtractor)
        {
            CommonType = commonType;
            Type = type;
            Set = set;
            Context = context;
            OperationExtractor = operationExtractor;
        }

        public IEdmModelBuilderContext Context { get; set; }

        private EntityTypeConfiguration<TEntityType> Type { get; }
        public EntityTypeConfiguration CommonType { get; }
        public EntitySetConfiguration<TEntityType> Set { get; }
        private IEdmModelOperationExtractor OperationExtractor { get; }


        /// <summary>
        ///     Sets the base type of this entity type.
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
        ///     Marks this entity type as abstract.
        /// </summary>
        /// <returns>Returns itself so that multiple calls can be chained.</returns>
        public EdmModelEntityType<TEntityType, TController> Abstract()
        {
            Type.Abstract();
            return this;
        }

        public void HasKey<TKey>(Expression<Func<TEntityType, TKey>> keyDefinitionExpression)
        {
            Type.HasKey(keyDefinitionExpression);
        }

        public LengthPropertyConfiguration Property(Expression<Func<TEntityType, string>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public LengthPropertyConfiguration Property(Expression<Func<TEntityType, byte[]>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public PrimitivePropertyConfiguration Property(Expression<Func<TEntityType, Stream>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public DecimalPropertyConfiguration Property(Expression<Func<TEntityType, decimal?>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public DecimalPropertyConfiguration Property(Expression<Func<TEntityType, decimal>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public PrecisionPropertyConfiguration Property(Expression<Func<TEntityType, TimeOfDay?>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public PrecisionPropertyConfiguration Property(Expression<Func<TEntityType, TimeOfDay>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public PrecisionPropertyConfiguration Property(Expression<Func<TEntityType, TimeSpan?>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public PrecisionPropertyConfiguration Property(Expression<Func<TEntityType, TimeSpan>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public PrecisionPropertyConfiguration Property(Expression<Func<TEntityType, DateTimeOffset?>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public PrecisionPropertyConfiguration Property(Expression<Func<TEntityType, DateTimeOffset>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public UntypedPropertyConfiguration Property(Expression<Func<TEntityType, object>> propertyExpression)
        {
            return Type.Property(propertyExpression);
        }

        public PrimitivePropertyConfiguration Property<T>(Expression<Func<TEntityType, T?>> propertyExpression) 
            where T : struct
        {
            return Type.Property(propertyExpression);
        }

        public PrimitivePropertyConfiguration Property<T>(Expression<Func<TEntityType, T>> propertyExpression) 
            where T : struct
        {
            return Type.Property(propertyExpression);
        }

        public EnumPropertyConfiguration EnumProperty<T>(Expression<Func<TEntityType, T?>> propertyExpression) 
            where T : struct
        {
            return Type.EnumProperty(propertyExpression);
        }

        public EnumPropertyConfiguration EnumProperty<T>(Expression<Func<TEntityType, T>> propertyExpression) 
            where T : struct
        {
            return Type.EnumProperty(propertyExpression);
        }

        public ComplexPropertyConfiguration ComplexProperty<TComplexType>(Expression<Func<TEntityType, TComplexType>> propertyExpression)
        {
            return Type.ComplexProperty(propertyExpression);
        }

        public CollectionPropertyConfiguration CollectionProperty<TElementType>(Expression<Func<TEntityType, IEnumerable<TElementType>>> propertyExpression)
        {
            return Type.CollectionProperty(propertyExpression);
        }

        public EdmModelEntityType<TEntityType, TController> AddOperation(Expression<Action<TController>> operationExpression,
            Action<IEdmModelOperation>? init = null)
        {
            OperationExtractor.CreateOperation(Type.ToOperationHolder(), operationExpression, init);
            return this;
        }

        public EdmModelEntityType<TEntityType, TController> AddOperation(Expression<Func<TController, Task>> operationExpression,
            Action<IEdmModelOperation>? init = null)
        {
            OperationExtractor.CreateOperation(Type.ToOperationHolder(), operationExpression, init);
            return this;
        }

        public EdmModelEntityType<TEntityType, TController> AddCollectionOperation(Expression<Action<TController>> operationExpression,
            Action<IEdmModelOperation>? init = null)
        {
            OperationExtractor.CreateOperation(Type.Collection.ToOperationHolder(), operationExpression, init);
            return this;
        }

        public EdmModelEntityType<TEntityType, TController> AddCollectionOperation(Expression<Func<TController, Task>> operationExpression,
            Action<IEdmModelOperation>? init = null)
        {
            OperationExtractor.CreateOperation(Type.Collection.ToOperationHolder(), operationExpression, init);
            return this;
        }
    }
}