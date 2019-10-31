using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using System.Threading;

namespace Mystique.Mvc.Infrastructure
{
    // https://stackoverflow.com/questions/46156649/asp-net-core-register-controller-at-runtime
    // https://github.com/aspnet/Mvc/blob/rel/2.0.0/src/Microsoft.AspNetCore.Mvc.Core/Internal/ActionDescriptorCollectionProvider.cs#L39
    public class MystiqueActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static MystiqueActionDescriptorChangeProvider Instance { get; } = new MystiqueActionDescriptorChangeProvider();

        public CancellationTokenSource TokenSource { get; private set; }

        public bool HasChanged { get; set; }

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }
}
