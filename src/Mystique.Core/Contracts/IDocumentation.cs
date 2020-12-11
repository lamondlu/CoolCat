using Mystique.Core.DomainModel;
using System.Collections.Generic;

namespace Mystique.Core.Contracts
{
    public interface IQueryDocumentation
    {
        void BuildDocumentation(string moduleName, IDataStoreQuery query);

        Dictionary<string, List<QueryDocumentItem>> GetAllDocuments();


    }
}
