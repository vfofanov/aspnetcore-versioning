namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public class ModelFilterFactoryRegistration<T> : IModelFilterFactoryRegistration
        where T : IModelFilterFactory
    {
        /// <inheritdoc />
        public bool RequestOnly { get; }

        /// <inheritdoc />
        public IModelFilterFactory Factory { get; }

        public ModelFilterFactoryRegistration(bool requestOnly, IModelFilterFactory factory)
        {
            RequestOnly = requestOnly;
            Factory = factory;
        }
    }
}