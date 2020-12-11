using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class MystiqueViewCompiler : IViewCompiler
    {
        private readonly Dictionary<string, Task<CompiledViewDescriptor>> _compiledViews;
        private readonly ConcurrentDictionary<string, string> _normalizedPathCache;
        private readonly ILogger _logger;

        public MystiqueViewCompiler(
            IList<CompiledViewDescriptor> compiledViews,
            ILogger logger)
        {
            if (compiledViews == null)
            {
                throw new ArgumentNullException(nameof(compiledViews));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
            _normalizedPathCache = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

            _compiledViews = new Dictionary<string, Task<CompiledViewDescriptor>>(
                compiledViews.Count,
                StringComparer.OrdinalIgnoreCase);

            foreach (var compiledView in compiledViews)
            {

                if (!_compiledViews.ContainsKey(compiledView.RelativePath))
                {
                    _compiledViews.Add(compiledView.RelativePath, Task.FromResult(compiledView));
                }
            }

            if (_compiledViews.Count == 0)
            {

            }
        }

        public Task<CompiledViewDescriptor> CompileAsync(string relativePath)
        {
            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            if (_compiledViews.TryGetValue(relativePath, out var cachedResult))
            {
                return cachedResult;
            }

            var normalizedPath = GetNormalizedPath(relativePath);
            if (_compiledViews.TryGetValue(normalizedPath, out cachedResult))
            {

                return cachedResult;
            }

            return Task.FromResult(new CompiledViewDescriptor
            {
                RelativePath = normalizedPath,
                ExpirationTokens = Array.Empty<IChangeToken>(),
            });
        }

        private string GetNormalizedPath(string relativePath)
        {
            Debug.Assert(relativePath != null);
            if (relativePath.Length == 0)
            {
                return relativePath;
            }

            if (!_normalizedPathCache.TryGetValue(relativePath, out var normalizedPath))
            {
                normalizedPath = NormalizePath(relativePath);
                _normalizedPathCache[relativePath] = normalizedPath;
            }

            return normalizedPath;
        }

        public static string NormalizePath(string path)
        {
            var addLeadingSlash = path[0] != '\\' && path[0] != '/';
            var transformSlashes = path.IndexOf('\\') != -1;

            if (!addLeadingSlash && !transformSlashes)
            {
                return path;
            }

            var length = path.Length;
            if (addLeadingSlash)
            {
                length++;
            }

            return string.Create(length, (path, addLeadingSlash), (span, tuple) =>
            {
                var (pathValue, addLeadingSlashValue) = tuple;
                var spanIndex = 0;

                if (addLeadingSlashValue)
                {
                    span[spanIndex++] = '/';
                }

                foreach (var ch in pathValue)
                {
                    span[spanIndex++] = ch == '\\' ? '/' : ch;
                }
            });
        }
    }
}
