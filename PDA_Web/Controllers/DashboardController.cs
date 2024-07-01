using Microsoft.AspNetCore.Mvc;
using PDA_Web.Models;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;
using System.Diagnostics;

namespace PDA_Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public DashboardController(ILogger<DashboardController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var CustID = HttpContext.Session.GetString("CustID");
            var CustUserID = HttpContext.Session.GetString("ID");
            if (!string.IsNullOrEmpty(CustID))
            {
                var custdata = await _unitOfWork.CustomerUserMaster.GetByIdAsync(Convert.ToInt32(CustUserID));
                if (custdata != null)
                {
                    var LastLogin = custdata.LoginDateTime != null ? Convert.ToDateTime(custdata.LoginDateTime).AddHours(5).AddMinutes(30).ToString("MMM dd yyyy hh:mm tt") : "First Time Login";
                    HttpContext.Session.SetString("LastLogin", LastLogin.ToString());
                }
                List<PDAEstimatorList> pDAEstimatorLists = new List<PDAEstimatorList>();

                var totalPDA = await _unitOfWork.PDAEstimitor.GetAlllistByCustIdAsync(Convert.ToInt32(CustID));
                ViewBag.PDAs = totalPDA.Count;
                ViewBag.lastThrityDaysPDAs = totalPDA.Where(x => x.ETA >= DateTime.Now.AddDays(-30)).Count();

                pDAEstimatorLists = totalPDA.Where(x => x.ETA >= DateTime.Now.AddDays(-30)).ToList();

                return View(pDAEstimatorLists);
            }
            else
            {
                return RedirectToAction("index", "Login");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}