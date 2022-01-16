#nullable enable

using System;
using System.Linq.Expressions;
using NUnit.Framework;
using TestSample.Controllers.OData;

namespace Stenn.AspNetCore.OData.Versioning.Tests
{
    public class Tests
    {
        [Test]
        public void Temp()
        {
            var name = AddOperation<BooksController>(c => c.EBooks(
                EdmOp.Param<string>(p => p.Required()),
                default));

            var name2 = AddOperation<BooksController>(c => c.EBooksPost(EdmOp.ActionParams()));
        }

        private string AddOperation<T>(Expression<Action<T>> operationExpression)
        {
            var methodCallProvider = operationExpression.Body as MethodCallExpression;
            if (methodCallProvider is null)
            {
                throw new ArgumentException(nameof(operationExpression), "Supports only method call expression");
            }
            return HandleOperation<T>(methodCallProvider);
        }

        private string HandleOperation<T>(MethodCallExpression methodCallProvider)
        {
            //TODO: Check for HttpGet and HttpPost
            //methodCallProvider.Method.CustomAttributes

            //TODO: Mutator: Check method for api version and etc.
            //methodCallProvider.Method.CustomAttributes

            //TODO: Mutator: Check odata controller 
            //typeof(T).CustomAttributes


            //methodCallProvider.Arguments
            return null;
        }

        
    }
}