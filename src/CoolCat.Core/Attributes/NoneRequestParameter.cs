using System;

namespace CoolCat.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NoneRequestParameterAttribute : RequestParameterTypeAttribute
    {
        public NoneRequestParameterAttribute() : base(null)
        {

        }
    }
}
