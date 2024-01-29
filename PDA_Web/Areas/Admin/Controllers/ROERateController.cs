using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ROERateController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public ROERateController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;

        }
        public async Task<IActionResult> Index()
        {
            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                // Temp Solution START
                var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
                ViewBag.UserPermissionModel = UserPermissionModel;
                var Currentuser = HttpContext.Session.GetString("UserID");

                var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
                ViewBag.UserRoleName = UserRole;
                // Temp Solution END
                var ROERateData = await unitOfWork.Rates.GetAllAsync();
                ViewBag.rates = ROERateData;

                var ROERatename = await unitOfWork.ROENames.GetAllAsync();
                ViewBag.ROENames = ROERatename;
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
            
        }


        public async Task<IActionResult> Search(DateTime searchDate)
        {
            var filteredData = await unitOfWork.Rates.GetAlllistbyDate(searchDate);

            return PartialView("partial/_ViewAll", filteredData);

        }

        public async Task<IActionResult> LoadAll()
        {
            var ROERateData = await unitOfWork.Rates.GetAlllistbyLoadAll();
            return PartialView("partial/_ViewAll", ROERateData);

        }

        //public async Task<IActionResult> LoadAll(DateTime searchDate)
        //{
        //    var ROERateData = await unitOfWork.Rates.GetAlllistbyDate(searchDate);
        //    return PartialView("partial/_ViewAll", ROERateData);
            
        //}
        //[HttpPost]
        //public ActionResult ROERateSave(ROERates rOERates)
        //{ 
        //    return View(rOERates);
        //}

        public async Task<ActionResult> ROERateSave(ROERates rOERates)
        {
            if (rOERates.ID > 0)
            {
                await unitOfWork.Rates.UpdateAsync(rOERates);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.Rates.AddAsync(rOERates);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return RedirectToAction("index", "ROERate");
            //return Json(new
            //{
            //    proceed = true,
            //    msg = ""
            //});
        }
    }
}
