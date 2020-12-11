namespace Mystique.Core.ViewModels
{
    public class PageRouteViewModel
    {
        public PageRouteViewModel(string pageName, string area, string controller, string action)
        {
            PageName = pageName;
            Area = area;
            Controller = controller;
            Action = action;
        }

        public string PageName { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Url => $"{Area}/{Controller}/{Action}";
    }
}
