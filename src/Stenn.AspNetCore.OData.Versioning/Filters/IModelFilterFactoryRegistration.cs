namespace Stenn.AspNetCore.OData.Versioning.Filters
{
    public interface IModelFilterFactoryRegistration
    {
        bool RequestOnly { get; }
        IModelFilterFactory Factory { get; }
    }
}