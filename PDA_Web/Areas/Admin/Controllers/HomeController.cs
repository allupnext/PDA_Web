using Microsoft.AspNetCore.Mvc;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
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
            ViewBag.Customers = customerCount;

            var CargoType = await unitOfWork.CargoDetails.GetAllAsync();
            ViewBag.Cargo = CargoType;

            var CompanyData = await unitOfWork.Company.GetAllAsync();
            ViewBag.Companys = CompanyData;

            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser =  HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName( Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;

            ViewBag.CustomerCount = customerCount.Count;
            ViewBag.pendingCustomer = customerCount.Where(x => x.Status == "Pending For Approval").Count();

            
            var totalPDA = await unitOfWork.PDAEstimitor.GetAllAsync();
            ViewBag.PDAs = totalPDA.Count;  
            ViewBag.lastThrityDaysPDAs = totalPDA.Where(x=> x.ETA>= DateTime.Now.AddDays(-30)).Count();

            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                List<PDAEstimatorList> pDAEstimatorLists = new List<PDAEstimatorList>();
                AdminDashboard adminDashboards = new AdminDashboard();

                var userdata = await unitOfWork.User.GetAllUsersById(Convert.ToInt64(userid));
                var userwithRole = await unitOfWork.User.GetByIdAsync(Convert.ToInt64(userid));
                var LastLogin = userdata.LoginDateTime != null ? Convert.ToDateTime(userdata.LoginDateTime).AddHours(5).AddMinutes(30).ToString("MMM dd yyyy hh:mm tt") : "First Time Login";
                HttpContext.Session.SetString("LastLogin", LastLogin.ToString());
                HttpContext.Session.SetString("loginusername", userdata.FirstName + " " + userdata.LastName);
                if (userwithRole.RoleName == "Admin")
                {
                    pDAEstimatorLists = await unitOfWork.PDAEstimitor.GetPDAEstiomatorListOfLast30Days();
                }
                else
                {
                    if (userdata.Ports != null && userdata.Ports != "")
                    {
                        List<int> PortIds = userdata.Ports.Split(',').Select(int.Parse).ToList();
                        pDAEstimatorLists = await unitOfWork.PDAEstimitor.GetPDAEstiomatorListOfLast30Days();
                        pDAEstimatorLists = pDAEstimatorLists.Where(x => PortIds.Contains(x.PortID)).ToList();

                    }
                }
                var customerdata = await unitOfWork.Customer.GetAlllistAsync();
                customerdata = customerdata.Where(x => x.Status == "Pending For Approval").ToList();
                adminDashboards.pDAEstimatorLists = pDAEstimatorLists;
                adminDashboards.customerList = customerdata;

                return View(adminDashboards);
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }


/*        public async Task<IActionResult> LoadAll(PDAEstimator pDAEstimator)
        {
            List<PDAEstimatorList> pDAEstimatorLists = new List<PDAEstimatorList>();
            var userid = HttpContext.Session.GetString("UserID");

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            var userdata = await unitOfWork.User.GetAllUsersById(Convert.ToInt64(userid));
            var userwithRole = await unitOfWork.User.GetByIdAsync(Convert.ToInt64(userid));
            if (userwithRole.RoleName == "Admin")
            {
                pDAEstimatorLists = await unitOfWork.PDAEstimitor.GetAlllistAsync();
            }
            else
            {
                if (userdata.Ports != null && userdata.Ports != "")
                {
                    List<int> PortIds = userdata.Ports.Split(',').Select(int.Parse).ToList();
                    pDAEstimatorLists = await unitOfWork.PDAEstimitor.GetAlllistAsync();
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => PortIds.Contains(x.PortID)).ToList();

                }
            }

*//*            if (pDAEstimatorLists.Count() > 0)
            {
                if (pDAEstimator.CustomerID != null && pDAEstimator.CustomerID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.CustomerID == pDAEstimator.CustomerID && x.PortID == pDAEstimator.PortID).ToList();
                }
                if (pDAEstimator.PortID != null && pDAEstimator.PortID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.PortID == pDAEstimator.PortID).ToList();
                }
                if (pDAEstimator.TerminalID != null && pDAEstimator.TerminalID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.TerminalID == pDAEstimator.TerminalID).ToList();
                }
                if (pDAEstimator.CallTypeID != null && pDAEstimator.CallTypeID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.CallTypeID == pDAEstimator.CallTypeID).ToList();
                }
                if (pDAEstimator.CreatedBy != null && pDAEstimator.CreatedBy != "")
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => Convert.ToInt64(x.UserId) == Convert.ToInt64(pDAEstimator.CreatedBy)).ToList();

                }
            }*//*
            return View(pDAEstimatorLists);
        }*/


        /*        public async Task<IActionResult> GetUserPermissionRights()
                {
                    var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();

                    return View();
                }*/

    }
}
