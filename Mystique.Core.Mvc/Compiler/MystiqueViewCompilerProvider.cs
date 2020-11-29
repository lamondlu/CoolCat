using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Logging;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class MystiqueViewCompilerProvider : IViewCompilerProvider
    {
        private MystiqueViewCompiler _compiler;
        private ApplicationPartManager _applicationPartManager;
        private ILoggerFactory _loggerFactory;

        public MystiqueViewCompilerProvider(
            ApplicationPartManager applicationPartManager,
            ILoggerFactory loggerFactory)
        {
            _applicationPartManager = applicationPartManager;
            _loggerFactory = loggerFactory;
            Refresh();
        }

        public void Refresh()
        {
            var feature = new ViewsFeature();
            _applicationPartManager.PopulateFeature(feature);

            _compiler = new MystiqueViewCompiler(feature.ViewDescriptors, _loggerFactory.CreateLogger<MystiqueViewCompiler>());
        }

        public IViewCompiler GetCompiler() => _compiler;
    }
}
