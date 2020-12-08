using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystique.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequestParameterTypeAttribute : Attribute
    {
        private Type _requestType;

        public RequestParameterTypeAttribute(Type requestType)
        {
            _requestType = requestType;
        }
    }
}
