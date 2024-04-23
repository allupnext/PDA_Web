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
            var customerCount = await _unitOfWork.Customer.GetAllAsync();
            ViewBag.Customers = customerCount;

            var CargoType = await _unitOfWork.CargoDetails.GetAllAsync();
            ViewBag.Cargo = CargoType;

            var CompanyData = await _unitOfWork.Company.GetAllAsync();
            ViewBag.Companys = CompanyData;



            var UserPermissionModel = await _unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var userid = HttpContext.Session.GetString("CustID");

            var UserRole = await _unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(userid));
            ViewBag.UserRoleName = UserRole;

            ViewBag.CustomerCount = customerCount.Count;
            ViewBag.pendingCustomer = customerCount.Where(x => x.Status == "Pending For Approval").Count();


            var totalPDA = await _unitOfWork.PDAEstimitor.GetAllAsync();
            ViewBag.PDAs = totalPDA.Count;
            ViewBag.lastThrityDaysPDAs = totalPDA.Where(x => x.ETA >= DateTime.Now.AddDays(-30)).Count();


            if (!string.IsNullOrEmpty(userid))
            {
                List<PDAEstimatorList> pDAEstimatorLists = new List<PDAEstimatorList>();

                var userdata = await _unitOfWork.User.GetAllUsersById(Convert.ToInt64(userid));
                var userwithRole = await _unitOfWork.User.GetByIdAsync(Convert.ToInt64(userid));
                if (userwithRole.RoleName == "Admin")
                {
                    pDAEstimatorLists = await _unitOfWork.PDAEstimitor.GetPDAEstiomatorListOfLast30Days();
                }
                else
                {
                    if (userdata.Ports != null && userdata.Ports != "")
                    {
                        List<int> PortIds = userdata.Ports.Split(',').Select(int.Parse).ToList();
                        pDAEstimatorLists = await _unitOfWork.PDAEstimitor.GetPDAEstiomatorListOfLast30Days();
                        pDAEstimatorLists = pDAEstimatorLists.Where(x => PortIds.Contains(x.PortID)).ToList();

                    }
                }
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