using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using System.Threading;

namespace Mystique.Mvc.Infrastructure
{
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
