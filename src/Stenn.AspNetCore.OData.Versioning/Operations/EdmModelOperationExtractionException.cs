using System;
using System.Reflection;

namespace Stenn.AspNetCore.OData.Versioning.Operations
{
    public sealed class EdmModelOperationExtractionException : Exception
    {
        /// <inheritdoc />
        public EdmModelOperationExtractionException(MethodInfo methodInfo, string? message, Exception? innerException = null)
            : base(AddPostfix(message, methodInfo), innerException)
        {
        }

        private static string AddPostfix(string? message, MethodInfo methodInfo)
        {
            var postfix = $"Metod '{methodInfo.DeclaringType?.FullName}.{methodInfo.Name}'";
            if (string.IsNullOrWhiteSpace(message))
            {
                return postfix;
            }
            return message.TrimEnd('.') + ". " + postfix;
        }
    }
}