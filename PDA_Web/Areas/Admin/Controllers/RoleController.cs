using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Data;
using System.Linq;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public RoleController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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

        public async Task<IActionResult> LoadAll(Roles roles)
        {
            var data = await unitOfWork.Roles.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (roles.RoleName != null /*&& customer.FirstName != 0*/)
            {
                //data = data.Where(x => x.RoleName.Contains("%"+roles.RoleName+"%")).ToList();
                data = data.Where(x => x.RoleName.Contains(roles.RoleName.ToUpper()) || x.RoleName.Contains(roles.RoleName.ToLower())).ToList();
            }
            return PartialView("partial/_ViewAll", data);
        }

        public async Task<ActionResult> RoleSave(Roles role)
        {
            var data = await unitOfWork.Roles.GetAllAsync();
            if (role.RoleID > 0)
            {
                var RoleName = data.Where(x => x.RoleName.ToUpper() == role.RoleName.ToUpper() && x.ID != role.ID).ToList();
                if (RoleName.Count > 0 && RoleName != null)
                {
                    _toastNotification.AddWarningToastMessage("RoleName already Exists");
                }
                else
                {
                    await unitOfWork.Roles.UpdateAsync(role);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }

            }
            else
            {
                var RoleName = data.Where(x => x.RoleName.ToUpper() == role.RoleName.ToUpper()).ToList();
                if (RoleName != null && RoleName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("RoleName already Exists");
                }
                else
                {
                    await unitOfWork.Roles.AddAsync(role);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editRole(Roles role)
        {
            var data = await unitOfWork.Roles.GetByIdAsync(role.RoleID);
            return Json(new
            {
                Roles = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteRole(Roles role)
        {
            var data = await unitOfWork.Roles.DeleteAsync(role.RoleID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
