using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.ModelBuilder;
using Stenn.AspNetCore.OData.Versioning.Filters;
using Stenn.AspNetCore.OData.Versioning.Operations;


namespace Stenn.AspNetCore.OData.Versioning
{
    /// <summary>
    ///     Proxy class under <see cref="ODataConventionModelBuilder" />
    /// </summary>
    public class EdmModelBuilder : IEdmModelBuilder, IEdmModelBuilderContext
    {
        private readonly ODataConventionModelBuilder _builder;
        private IEdmModelMutator? _mutator;
        private IEdmModelOperationExtractor? _operationExtractor;

        private readonly List<Action> _finalInitialization = new();

        
        public EdmModelBuilder(ODataConventionModelBuilder? builder = null)
        {
            _builder = builder ?? new ODataConventionModelBuilder();
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

        protected internal ODataConventionModelBuilder Builder => _builder;

        /// <inheritdoc />
        IEdmModelMutator IEdmModelBuilderContext.Mutator => Mutator;

        protected internal IEdmModelMutator Mutator
        {
            get => _mutator ?? throw new NullReferenceException("Initialize Mutator first");
            internal set => _mutator = value;
        }
        
        protected internal IEdmModelOperationExtractor OperationExtractor
        {
            get => _operationExtractor ?? throw new NullReferenceException("Initialize OperationExtractor first");
            internal set => _operationExtractor = value;
        }

        /// <summary>
        ///     Add entity set and entity type
        /// </summary>
        /// <param name="initAction"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TController"></typeparam>
        /// <returns></returns>
        public virtual void Add<TEntity, TController>(Action<EdmModelEntityType<TEntity, TController>>? initAction = null)
            where TEntity : class
            where TController : IODataController<TEntity>
        {
            var entitySetName = EdmExtensions.GetEntitySetName<TController>();
            var type = AddInternal<TEntity>(entitySetName);

            if (initAction != null)
            {
                var edmType = new EdmModelEntityType<TEntity, TController>(type, OperationExtractor);
                HandleInitAction(initAction, edmType);
            }
        }

        /// <summary>
        ///     Add entity set and type without controller
        /// </summary>
        /// <param name="entitySetName"></param>
        /// <param name="initAction"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public void AddUnbound<TEntity>(string? entitySetName = null, Action<EdmModelEntityType<TEntity>>? initAction = null)
            where TEntity : class
        {
            entitySetName ??= typeof(TEntity).Name + "Set";
            var type = AddInternal<TEntity>(entitySetName);
            HandleInitAction(initAction, type);
        }

        /// <summary>
        ///     Add entity set and type without controller
        /// </summary>
        /// <param name="entitySetName"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        protected virtual EdmModelEntityType<TEntity> AddInternal<TEntity>(string entitySetName)
            where TEntity : class
        {
            var (commonType, type) = AddEntityType<TEntity>();
            var set = _builder.EntitySet<TEntity>(entitySetName);
            return new EdmModelEntityType<TEntity>(commonType, type, set);
        }

        protected virtual (EntityTypeConfiguration commonType, EntityTypeConfiguration<TEntity> type) AddEntityType<TEntity>()
            where TEntity : class
        {
            var commonType = _builder.AddEntityType(typeof(TEntity));
            var type = _builder.EntityType<TEntity>();
            return (commonType, type);
        }

        public virtual void ComplexType<TComplexType>(Action<ComplexTypeConfiguration<TComplexType>>? initAction = null)
            where TComplexType : class
        {
            var configuration = _builder.ComplexType<TComplexType>();
            HandleInitAction(initAction, configuration);
        }

        public virtual void EnumType<TEnumType>(Action<EnumTypeConfiguration<TEnumType>>? initAction = null)
        {
            var configuration =  _builder.EnumType<TEnumType>();
            HandleInitAction(initAction, configuration);
        }
       
        public virtual void AddComplexType(Type type, Action<ComplexTypeConfiguration>? initAction = null)
        {
            var configuration = _builder.AddComplexType(type);
            HandleInitAction(initAction, configuration);
        }
        public virtual void AddEnumType(Type type, Action<EnumTypeConfiguration>? initAction = null)
        {
            var configuration = _builder.AddEnumType(type);
            HandleInitAction(initAction, configuration);
        }
        
        protected static bool KeyExist(IEdmModelEntityType? entityType)
        {
           return KeyExist(entityType?.CommonType);
        }
        protected static bool KeyExist(EntityTypeConfiguration? entityType)
        {
            if (entityType == null)
            {
                return false;
            }
            return entityType.Keys.Any() || KeyExist(entityType.BaseType);
        }

        protected void HandleInitAction<T>(Action<T>? initAction, T configuration)
        {
            if (initAction is null)
            {
                return;
            }
            _finalInitialization.Add(() => initAction(configuration));
        }

        internal void FinalizeBuilderIntenal()
        {
            //NOTE: We run init actions at the end for fill operations with all types already registered
            foreach (var action in _finalInitialization)
            {
                action();
            }
            FinalizeBuilder();
        }

        /// <summary>
        /// Final initialization before model generation
        /// </summary>
        protected virtual void FinalizeBuilder()
        {
        }
        #region TODO
        protected virtual void Ignore<T>()
        {
            _builder.Ignore<T>();
        }

        protected virtual void Ignore(params Type[] types)
        {
            _builder.Ignore(types);
        }
        
        // public virtual ActionConfiguration UnboundAction(string name)
        // {
        //     return _builder.Action(name);
        // }
        //
        // public virtual FunctionConfiguration UnboundFunction(string name)
        // {
        //     return _builder.Function(name);
        // }
        //
        // public virtual void AddOperation(OperationConfiguration operation)
        // {
        //     _builder.AddOperation(operation);
        // }
        // public virtual SingletonConfiguration<TEntityType> Singleton<TEntityType>(string name)
        //     where TEntityType : class
        // {
        //     return _builder.Singleton<TEntityType>(name);
        // }
        // public virtual SingletonConfiguration AddSingleton(string name, EntityTypeConfiguration entityType)
        // {
        //     //TODO: What is it and how handle it?
        //     return _builder.AddSingleton(name, entityType);
        // }
        #endregion
    }
}