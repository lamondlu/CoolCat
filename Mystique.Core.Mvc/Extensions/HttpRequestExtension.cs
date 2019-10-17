using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace Mystique.Core.Mvc.Extensions
{
    public static class HttpRequestExtension
    {
        public static Stream GetPluginStream(this HttpRequest request)
        {
            var package = request?.Form?.Files.FirstOrDefault();
            if (package == null)
            {
                throw new Exception("The plugin package is missing.");
            }
            return package.OpenReadStream();
        }
    }
}
