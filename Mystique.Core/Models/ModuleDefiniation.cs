using System;
using System.Collections.Generic;
using System.Text;

namespace Mystique.Core.Models
{
    public abstract class ModuleDefiniation
    {
        private string _moduleName = string.Empty;

        public ModuleDefiniation(string moduleName)
        {
            _moduleName = moduleName;
        }

        public string ModuleName
        {
            get
            {
                return _moduleName;
            }
        }
    }
}
