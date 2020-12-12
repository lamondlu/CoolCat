using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace CoolCat.Core.Mvc.Extensions
{
    public static class HttpRequestExtension
    {
        public static Stream GetPluginStream(this HttpRequest request)
        {
            if (request == null || request.Form.Files.Count == 0)
            {
                throw new Exception("The plugin package is missing.");
            }

            return request.Form.Files.First().OpenReadStream();
        }
    }
}
