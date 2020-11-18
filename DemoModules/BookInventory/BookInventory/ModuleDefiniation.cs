using Mystique.Core.Models;

namespace DemoPlugin1
{
    public class ModuleDefiniation : Mystique.Core.Models.ModuleDefiniation
    {
        public const string MODULE_NAME = "BookInventory";

        public ModuleDefiniation() : base(MODULE_NAME)
        {

        }
    }
}
