namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    public class ODataActionCollectionParameter<T> : IODataActionCollectionParameter<T>
    {
        public ODataActionCollectionParameter(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }
    }
}