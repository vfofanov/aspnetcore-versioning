using System;
using Microsoft.OData.ModelBuilder;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public abstract class OperationReturnTypeHolder
    {
        protected OperationReturnTypeHolder(Type? bindedClrType)
        {
            BindedClrType = bindedClrType;
        }

        public Type? BindedClrType { get; }

        public abstract OperationConfiguration Configuration { get; }

        public void Returns(Type elementEntityType, bool isCollection)
        {
            if (isCollection)
            {
                ReturnsCollection(elementEntityType);
            }
            else
            {
                Returns(elementEntityType);
            }
        }

        /// <summary>
        ///     Established the return type of the Action.
        ///     <remarks>Used when the return type is a single Primitive or ComplexType.</remarks>
        /// </summary>
        public abstract void Returns(Type clrReturnType);

        /// <summary>
        ///     Establishes the return type of the Action
        ///     <remarks>Used when the return type is a collection of either Primitive or ComplexTypes.</remarks>
        /// </summary>
        public abstract void ReturnsCollection(Type clrElementType);

        public void ReturnsFromEntitySet(Type elementEntityType, string entitySetName, bool isCollection)
        {
            if (isCollection)
            {
                ReturnsCollectionFromEntitySet(elementEntityType, entitySetName);
            }
            else
            {
                ReturnsFromEntitySet(elementEntityType, entitySetName);
            }
        }

        /// <summary>
        ///     Sets the return type to a single EntityType instance.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="entitySetName">The name of the entity set which contains the returned entity.</param>
        public abstract void ReturnsFromEntitySet(Type entityType, string entitySetName);

        /// <summary>
        ///     Sets the return type to a collection of entities.
        /// </summary>
        /// <param name="elementEntityType">The element entity type.</param>
        /// <param name="entitySetName">The name of the entity set which contains the returned entities.</param>
        public abstract void ReturnsCollectionFromEntitySet(Type elementEntityType, string entitySetName);

        public void ReturnsViaEntitySetPath(Type elementEntityType, string entitySetPath, bool isCollection)
        {
            if (isCollection)
            {
                ReturnsCollectionViaEntitySetPath(elementEntityType, entitySetPath);
            }
            else
            {
                ReturnsEntityViaEntitySetPath(elementEntityType, entitySetPath);
            }
        }

        /// <summary>
        ///     Sets the return type to a single EntityType instance.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <param name="entitySetPath">The entitySetPath which contains the return EntityType instance.</param>
        public abstract void ReturnsEntityViaEntitySetPath(Type entityType, string entitySetPath);

        /// <summary>
        ///     Sets the return type to a collection of EntityType instances.
        /// </summary>
        /// <param name="clrElementEntityType">The element entity type.</param>
        /// <param name="entitySetPath">The entitySetPath which contains the returned EntityType instances.</param>
        public abstract void ReturnsCollectionViaEntitySetPath(Type clrElementEntityType, string entitySetPath);

        /// <summary>
        ///     Sets the return type to a void.
        /// </summary>
        public abstract void ReturnsVoid();

        internal sealed class Action : OperationReturnTypeHolder
        {
            private readonly ActionConfiguration _operation;

            /// <inheritdoc />
            public Action(ActionConfiguration operation, Type? bindedClrType)
                : base(bindedClrType)
            {
                _operation = operation;
            }

            /// <inheritdoc />
            public override OperationConfiguration Configuration => _operation;

            public override void ReturnsFromEntitySet(Type entityType, string entitySetName)
            {
                _operation.ReturnsFromEntitySet(entityType, entitySetName);
            }

            public override void ReturnsCollectionFromEntitySet(Type elementEntityType, string entitySetName)
            {
                _operation.ReturnsCollectionFromEntitySet(elementEntityType, entitySetName);
            }

            public override void Returns(Type clrReturnType)
            {
                _operation.Returns(clrReturnType);
            }

            public override void ReturnsCollection(Type clrElementType)
            {
                _operation.ReturnsCollection(clrElementType);
            }

            public override void ReturnsEntityViaEntitySetPath(Type entityType, string entitySetPath)
            {
                _operation.ReturnsEntityViaEntitySetPath(entityType, entitySetPath);
            }

            public override void ReturnsCollectionViaEntitySetPath(Type clrElementEntityType, string entitySetPath)
            {
                _operation.ReturnsCollectionViaEntitySetPath(clrElementEntityType, entitySetPath);
            }

            /// <inheritdoc />
            public override void ReturnsVoid()
            {
                _operation.ReturnType = null;
            }
        }

        internal sealed class Function : OperationReturnTypeHolder
        {
            private readonly FunctionConfiguration _operation;

            /// <inheritdoc />
            public Function(FunctionConfiguration operation, Type? bindedClrType)
                : base(bindedClrType)
            {
                _operation = operation;
            }

            public override OperationConfiguration Configuration => _operation;

            public override void ReturnsFromEntitySet(Type entityType, string entitySetName)
            {
                _operation.ReturnsFromEntitySet(entityType, entitySetName);
            }

            public override void ReturnsCollectionFromEntitySet(Type elementEntityType, string entitySetName)
            {
                _operation.ReturnsCollectionFromEntitySet(elementEntityType, entitySetName);
            }

            public override void Returns(Type clrReturnType)
            {
                _operation.Returns(clrReturnType);
            }

            public override void ReturnsCollection(Type clrElementType)
            {
                _operation.ReturnsCollection(clrElementType);
            }

            public override void ReturnsEntityViaEntitySetPath(Type entityType, string entitySetPath)
            {
                _operation.ReturnsEntityViaEntitySetPath(entityType, entitySetPath);
            }

            public override void ReturnsCollectionViaEntitySetPath(Type clrElementEntityType, string entitySetPath)
            {
                _operation.ReturnsCollectionViaEntitySetPath(clrElementEntityType, entitySetPath);
            }

            /// <inheritdoc />
            public override void ReturnsVoid()
            {
                _operation.ReturnType = null;
            }
        }
    }
}