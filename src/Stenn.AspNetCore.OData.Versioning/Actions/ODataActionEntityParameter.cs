namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    public class ODataActionEntityParameter<T> : IODataActionParameter<T>
        where T : class
    {
        public ODataActionEntityParameter(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        public string Name { get; }
    }
}