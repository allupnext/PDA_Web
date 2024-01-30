using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Diagnostics.Metrics;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CallTypeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public CallTypeController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
        public async Task<IActionResult> LoadAll(CallType callType)
        {
            var data = await unitOfWork.CallTypes.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (callType.CallTypeName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CallTypeName.ToUpper().Contains(callType.CallTypeName.ToUpper())).ToList();
            }

            return PartialView("partial/_ViewAll", data);
        }

        public async Task<ActionResult> CallTypeSave(CallType callType)
        { 
            var data = await unitOfWork.CallTypes.GetAllAsync();
            if (callType.ID > 0)
            {
                var CallTypeName = data.Where(x => x.CallTypeName.ToUpper() == callType.CallTypeName.ToUpper() && x.ID != callType.ID).ToList();
                if (CallTypeName != null && CallTypeName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("CallTypeName already exists.");
                }
                else
                {
                    await unitOfWork.CallTypes.UpdateAsync(callType);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var CallTypeName = data.Where(x => x.CallTypeName.ToUpper() == callType.CallTypeName.ToUpper()).ToList();
                if (CallTypeName != null && CallTypeName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("CallTypeName already exists.");
                }
                else
                {
                    await unitOfWork.CallTypes.AddAsync(callType);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editCallType(CallType callType)
        {
            var data = await unitOfWork.CallTypes.GetByIdAsync(callType.ID);
            return Json(new
            {
                CallTypes = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteCallType(CallType callType)
        {
            var data = await unitOfWork.CallTypes.DeleteAsync(callType.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
