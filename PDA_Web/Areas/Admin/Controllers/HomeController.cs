using Microsoft.AspNetCore.Mvc;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Infrastructure.Repositories;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult>  Index()
        {
            var customerCount = await unitOfWork.Customer.GetAllAsync();
            ViewBag.CustomerCount = customerCount.Count;
            ViewBag.pendingCustomer = customerCount.Where(x => x.Status == "Pending For Approval").Count();

            
            var totalPDA = await unitOfWork.PDAEstimitor.GetAllAsync();
            ViewBag.PDAs = totalPDA.Count;
            ViewBag.lastThrityDaysPDAs = totalPDA.Where(x=> x.ETA>= DateTime.Now.AddDays(-30)).Count();

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
