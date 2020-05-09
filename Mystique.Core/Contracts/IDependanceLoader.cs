using Mystique.Core.DomainModel;
using System.Collections.Generic;

namespace Mystique.Core.Contracts
{
    public interface IDependanceLoader
    {
        List<DependanceItem> GetDependanceItems(string jsonFilePath);
    }
}
