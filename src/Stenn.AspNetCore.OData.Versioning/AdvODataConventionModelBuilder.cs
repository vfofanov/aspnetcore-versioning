using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    /// Proxy class under <see cref="ODataConventionModelBuilder"/> 
    /// </summary>
    public class AdvODataConventionModelBuilder : IODataConventionModelBuilder
    {
        private readonly ODataConventionModelBuilder _builder;

        public AdvODataConventionModelBuilder(ODataConventionModelBuilder builder = null)
        {
            _builder = builder ?? new ODataConventionModelBuilder();
        }

        public EntityTypeConfiguration<TEntity> AddEntityType<TEntity>(Action<EntityTypeConfiguration<TEntity>> typeInitialConfiguration = null)
            where TEntity : class
        {
            return AddEntityType<TEntity>((_, type) => typeInitialConfiguration?.Invoke(type));
        }

        protected virtual EntityTypeConfiguration<TEntity> AddEntityType<TEntity>(
            Action<EntityTypeConfiguration, EntityTypeConfiguration<TEntity>> typeInitialConfiguration = null)
            where TEntity : class
        {
            var commonType = AddEntityType(typeof(TEntity));
            var result = EntityType<TEntity>();
            typeInitialConfiguration?.Invoke(commonType, result);
            return result;
        }

        protected static bool KeyExist(EntityTypeConfiguration entityType)
        {
            if (entityType == null)
            {
                return false;
            }
            return entityType.Keys.Any() || KeyExist(entityType.BaseType);
        }

        /// <summary>
        /// Add entity set and entity type
        /// </summary>
        /// <param name="typeInitialConfiguration"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TController"></typeparam>
        /// <returns></returns>
        public virtual EntitySetConfiguration<TEntity> Add<TEntity, TController>(
            Action<EntityTypeConfiguration<TEntity>> typeInitialConfiguration = null)
            where TEntity : class
            where TController : IODataController<TEntity>
        {
            return Add<TEntity, TController>((_, type) => typeInitialConfiguration?.Invoke(type));
        }

        /// <summary>
        /// Add entity set and entity type
        /// </summary>
        /// <param name="typeInitialConfiguration"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TController"></typeparam>
        /// <returns></returns>
        protected virtual EntitySetConfiguration<TEntity> Add<TEntity, TController>(
            Action<EntityTypeConfiguration, EntityTypeConfiguration<TEntity>> typeInitialConfiguration = null)
            where TEntity : class
            where TController : IODataController<TEntity>
        {
            var entitySetName = EdmExtensions.GetEntitySetName<TController>();
            return AddUnbound<TEntity>(entitySetName, (commonType, type) => typeInitialConfiguration?.Invoke(commonType, type));
        }

        /// <summary>
        /// Add entity set and type without controller
        /// </summary>
        /// <param name="entitySetName"></param>
        /// <param name="typeInitialConfiguration"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public EntitySetConfiguration<TEntity> AddUnbound<TEntity>(string entitySetName,
            Action<EntityTypeConfiguration<TEntity>> typeInitialConfiguration = null)
            where TEntity : class
        {
            return AddUnbound<TEntity>(entitySetName, (_, type) => typeInitialConfiguration?.Invoke(type));
        }

        /// <summary>
        /// Add entity set and type without controller
        /// </summary>
        /// <param name="entitySetName"></param>
        /// <param name="typeInitialConfiguration"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected virtual EntitySetConfiguration<TEntity> AddUnbound<TEntity>(string entitySetName,
            Action<EntityTypeConfiguration, EntityTypeConfiguration<TEntity>> typeInitialConfiguration = null)
            where TEntity : class
        {
            AddEntityType(typeInitialConfiguration);
            return _builder.EntitySet<TEntity>(entitySetName);
        }

        public void EnableLowerCamelCase()
        {
            _builder.EnableLowerCamelCase();
        }

        public virtual EntitySetConfiguration<TEntity> EntitySet<TEntity, TController>()
            where TEntity : class
        {
            var entitySetName = EdmExtensions.GetEntitySetName<TController>();
            return _builder.EntitySet<TEntity>(entitySetName);
        }

        public virtual bool RemoveEntitySet<TController>()
        {
            var entitySetName = EdmExtensions.GetEntitySetName<TController>();
            return _builder.RemoveEntitySet(entitySetName);
        }


        #region Unchanged delegating methods
        public virtual EntityTypeConfiguration<TEntityType> EntityType<TEntityType>()
            where TEntityType : class
        {
            return _builder.EntityType<TEntityType>();
        }

        public virtual ComplexTypeConfiguration<TComplexType> ComplexType<TComplexType>()
            where TComplexType : class
        {
            return _builder.ComplexType<TComplexType>();
        }

        public virtual EnumTypeConfiguration<TEnumType> EnumType<TEnumType>()
        {
            return _builder.EnumType<TEnumType>();
        }

        public virtual SingletonConfiguration<TEntityType> Singleton<TEntityType>(string name)
            where TEntityType : class
        {
            return _builder.Singleton<TEntityType>(name);
        }

        public virtual ActionConfiguration Action(string name)
        {
            return _builder.Action(name);
        }

        public virtual FunctionConfiguration Function(string name)
        {
            return _builder.Function(name);
        }

        public virtual void AddOperation(OperationConfiguration operation)
        {
            _builder.AddOperation(operation);
        }

        public virtual bool RemoveStructuralType(Type clrType)
        {
            return _builder.RemoveStructuralType(clrType);
        }

        public virtual bool RemoveEnumType(Type clrType)
        {
            return _builder.RemoveEnumType(clrType);
        }


        public virtual bool RemoveSingleton(string name)
        {
            return _builder.RemoveSingleton(name);
        }

        public virtual bool RemoveOperation(string name)
        {
            return _builder.RemoveOperation(name);
        }

        public virtual bool RemoveOperation(OperationConfiguration operation)
        {
            return _builder.RemoveOperation(operation);
        }

        public virtual IEdmTypeConfiguration GetTypeConfigurationOrNull(Type type)
        {
            return _builder.GetTypeConfigurationOrNull(type);
        }

        public virtual string Namespace
        {
            get => _builder.Namespace;
            set => _builder.Namespace = value;
        }

        public virtual NavigationPropertyBindingOption BindingOptions
        {
            get => _builder.BindingOptions;
            set => _builder.BindingOptions = value;
        }

        public virtual string ContainerName
        {
            get => _builder.ContainerName;
            set => _builder.ContainerName = value;
        }

        public virtual Version DataServiceVersion
        {
            get => _builder.DataServiceVersion;
            set => _builder.DataServiceVersion = value;
        }

        public virtual Version MaxDataServiceVersion
        {
            get => _builder.MaxDataServiceVersion;
            set => _builder.MaxDataServiceVersion = value;
        }

        public virtual IEnumerable<EntitySetConfiguration> EntitySets => _builder.EntitySets;

        public virtual IEnumerable<SingletonConfiguration> Singletons => _builder.Singletons;

        public virtual IEnumerable<NavigationSourceConfiguration> NavigationSources => _builder.NavigationSources;

        public virtual IEnumerable<StructuralTypeConfiguration> StructuralTypes => _builder.StructuralTypes;

        public virtual IEnumerable<EnumTypeConfiguration> EnumTypes => _builder.EnumTypes;

        public virtual IEnumerable<OperationConfiguration> Operations => _builder.Operations;

        public virtual ODataConventionModelBuilder Ignore<T>()
        {
            return _builder.Ignore<T>();
        }

        public virtual ODataConventionModelBuilder Ignore(params Type[] types)
        {
            return _builder.Ignore(types);
        }

        public virtual EntityTypeConfiguration AddEntityType(Type type)
        {
            return _builder.AddEntityType(type);
        }

        public virtual ComplexTypeConfiguration AddComplexType(Type type)
        {
            return _builder.AddComplexType(type);
        }

        public virtual SingletonConfiguration AddSingleton(string name, EntityTypeConfiguration entityType)
        {
            return _builder.AddSingleton(name, entityType);
        }

        public virtual EnumTypeConfiguration AddEnumType(Type type)
        {
            return _builder.AddEnumType(type);
        }

        public virtual IEdmModel GetEdmModel()
        {
            return _builder.GetEdmModel();
        }

        public virtual void ValidateModel(IEdmModel model)
        {
            _builder.ValidateModel(model);
        }

        public virtual bool ModelAliasingEnabled
        {
            get => _builder.ModelAliasingEnabled;
            set => _builder.ModelAliasingEnabled = value;
        }

        public virtual Action<ODataConventionModelBuilder> OnModelCreating
        {
            get => _builder.OnModelCreating;
            set => _builder.OnModelCreating = value;
        }
        #endregion
    }
}