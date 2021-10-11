namespace AspNetCore.Versioning
{
    /// <summary>
    /// Factory inteface for <see cref="IApiVersionInfoProvider"/>
    /// </summary>
    public interface IApiVersionInfoProviderFactory
    {
        IApiVersionInfoProvider Create();
    }
}