using Mystique.Core.Attributes;
using Mystique.Core.Contracts;
using Mystique.Core.DomainModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystique.Core.Mvc.Infrastructure
{
    public class MystiqueModuleDocumentation : IQueryDocumentation
    {
        private Dictionary<string, List<QueryDocumentItem>> _documentations = new Dictionary<string, List<QueryDocumentItem>>();

        public void BuildDocumentation(string moduleName, IDataStoreQuery query)
        {
            if (!_documentations.ContainsKey(moduleName))
            {
                _documentations.Add(moduleName, new List<QueryDocumentItem>());
            }

            _documentations[moduleName].Add(RefactorDocuments(query));

        }

        public Dictionary<string, List<QueryDocumentItem>> GetAllDocuments()
        {
            return _documentations;
        }

        private QueryDocumentItem RefactorDocuments(IDataStoreQuery query)
        {
            var type = query.GetType();

            var responseType = ((ResponseTypeAttribute[])Attribute.GetCustomAttributes(type, typeof(ResponseTypeAttribute))).FirstOrDefault();

            var requestType = ((RequestParameterTypeAttribute[])Attribute.GetCustomAttributes(type, typeof(RequestParameterTypeAttribute))).FirstOrDefault();

            var item = new QueryDocumentItem();
            item.QueryName = query.QueryName;

            if (requestType is NoneRequestParameterAttribute)
            {
                item.RequestSample = "No Request Parameter.";
            }
            else
            {
                item.RequestSample = BuildSampleFromType(requestType.RequestType);
            }

            item.ResponseSample = BuildSampleFromType(responseType.ResponseType);

            return item;
        }

        private string BuildSampleFromType(Type t)
        {
            var obj = t.Assembly.CreateInstance(t.FullName);

            return JsonConvert.SerializeObject(obj);
        }
    }
}
