using DynamicPlugins.Core.Contracts;
using DynamicPlugins.Core.Repositories;
using DynamicPlugins.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicPlugins.Core.BusinessLogics
{
    public class PluginManager : IPluginManager
    {
        private IUnitOfWork _unitOfWork = null;

        public PluginManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<PluginListItemViewModel> GetAllPlugins()
        {
            return _unitOfWork.PluginRepository.GetAllPlugins();
        }

        public void AddPlugins()
        {

        }
    }
}
