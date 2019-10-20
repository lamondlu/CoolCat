using Mystique.Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Contracts
{
    public interface IDependanceLoader
    {
        List<DependanceItem> GetDependanceItems(string jsonFilePath);
    }
}
