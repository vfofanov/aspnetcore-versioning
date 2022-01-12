namespace Stenn.AspNetCore.OData.Versioning.Actions
{
    public interface IODataActionParameter
    {
        string Name { get; }
    }

    public interface IODataActionParameter<T> : IODataActionParameter
    {
    }

    public interface IODataActionCollectionParameter<T> : IODataActionParameter
    {
    }
}