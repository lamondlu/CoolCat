using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mystique.Core.Helpers
{
    public interface IRefenerceLoader
    {
        public void LoadStreamsIntoContext(CollectibleAssemblyLoadContext context);
    }
}
