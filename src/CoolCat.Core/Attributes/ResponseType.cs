using System;

namespace CoolCat.Core.Attributes
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
