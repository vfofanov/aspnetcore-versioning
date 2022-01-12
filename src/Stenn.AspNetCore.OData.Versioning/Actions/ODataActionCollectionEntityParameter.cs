namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    public class ODataActionCollectionEntityParameter<T> : IODataActionCollectionParameter<T>
        where T : class
    {
        public ODataActionCollectionEntityParameter(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }
    }
}