using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Stenn.AspNetCore.Versioning
{
    public interface IApiVersionInfoProvider
    {
        ApiVersion Default { get; }
        IReadOnlyList<ApiVersionInfo> Versions { get; }
    }
}