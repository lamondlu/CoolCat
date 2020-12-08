using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystique.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ResponseTypeAttribute : Attribute
    {
        private Type _responseType;

        public ResponseTypeAttribute(Type responseType)
        {
            _responseType = responseType;
        }

        public Type ResponseType
        {
            get
            {
                return _responseType;
            }
        }
    }
}
