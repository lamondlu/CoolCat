using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystique.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NoneRequestParameterAttribute : RequestParameterTypeAttribute
    {
        public NoneRequestParameterAttribute() : base(null)
        {

        }
    }
}
