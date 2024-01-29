using Microsoft.AspNetCore.Mvc;

namespace PDA_Web.Areas.Admin.Views.Shared.Components.Sidebar
{
    public class SidebarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();  
        }
    }
}
