using CoolCat.Core.DomainModel;
using System.Collections.Generic;

namespace CoolCat.Core.Contracts
{
    public interface IQueryDocumentation
    {
        void BuildDocumentation(string moduleName, IDataStoreQuery query);

        Dictionary<string, List<QueryDocumentItem>> GetAllDocuments();


    }
}
