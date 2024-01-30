using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ROENameController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public ROENameController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }
        public async Task<IActionResult> LoadAll(ROENames rOENames)
        {
            var data = await unitOfWork.ROENames.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (rOENames.ROEName!= null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.ROEName.ToUpper().Contains(rOENames.ROEName.ToUpper())).ToList();
            }
            return PartialView("partial/_ViewAll", data);
        }

        public async Task<ActionResult> ROENameSave(ROENames rOENames)
        {
            if (rOENames.ID > 0)
            {
                await unitOfWork.ROENames.UpdateAsync(rOENames);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.ROENames.AddAsync(rOENames);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditROEName(ROENames rOENames)
        {
            var data = await unitOfWork.ROENames.GetByIdAsync(rOENames.ID);
            return Json(new
            {
                ROENames = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteROENames(ROENames rOENames)
        {
            var data = await unitOfWork.ROENames.DeleteAsync(rOENames.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
