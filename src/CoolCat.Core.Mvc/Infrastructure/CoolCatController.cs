using CoolCat.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CoolCat.Core.Mvc.Infrastructure
{
    public class CoolCatController : Controller
    {
        private string _moduleName = string.Empty;
        private IDataStore _dataStore;

        public CoolCatController(string moduleName, IDataStore dataStore)
        {
            _moduleName = moduleName;
            _dataStore = dataStore;
        }

        protected string Query(string moduleName, string queryName, string parameter)
        {
            return _dataStore.Query(moduleName, queryName, parameter, source: _moduleName);
        }

        public override RedirectToActionResult RedirectToAction(string actionName, string controllerName)
        {
            return base.RedirectToAction(actionName, controllerName, new { Area = _moduleName });
        }
    }
}
