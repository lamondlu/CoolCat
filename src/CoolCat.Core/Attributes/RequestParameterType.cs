using System;

namespace CoolCat.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequestParameterTypeAttribute : Attribute
    {
        private Type _requestType;

        public RequestParameterTypeAttribute(Type requestType)
        {
            _requestType = requestType;
        }

        public Type RequestType
        {
            get
            {
                return _requestType;
            }
        }
    }
}
