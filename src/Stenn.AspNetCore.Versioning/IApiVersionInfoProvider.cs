using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    public interface IApiVersionInfoProvider
    {
        ApiVersionInfo Default { get; }
        IReadOnlyList<ApiVersionInfo> Versions { get; }
    }
}