using Microsoft.AspNetCore.Mvc;

namespace PDA_Web.Areas.Admin.Views.Shared.Components.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
