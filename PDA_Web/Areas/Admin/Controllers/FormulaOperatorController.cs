using Microsoft.AspNetCore.Mvc;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FormulaOperatorController : Controller
    {
        public IActionResult Index()
        {
            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }
    }
}
