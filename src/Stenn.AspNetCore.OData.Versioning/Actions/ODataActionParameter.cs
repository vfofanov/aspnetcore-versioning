#nullable enable
namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    public class ODataActionParameter<T> : IODataActionParameter<T>
    {
        public ODataActionParameter(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }
    }
}