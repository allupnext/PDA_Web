using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class EmailNotificationConfigController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public EmailNotificationConfigController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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

                var CompanyData = await unitOfWork.Company.GetAllAsync();
                ViewBag.Company = CompanyData;

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
        public async Task<IActionResult> LoadAll()
        {
            var data = await unitOfWork.EmailNotificationConfigurations.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            return PartialView("partial/_ViewAll", data);
        }

        public async Task<ActionResult> EmailNotificationConfigSave(EmailNotificationConfiguration emailNotificationConfiguration)
        { 
            var data = await unitOfWork.CallTypes.GetAllAsync();
            if (emailNotificationConfiguration.EmailConfigID > 0)
            {
                var ProcessName = data.Where(x => x.CallTypeName.ToUpper() == emailNotificationConfiguration.ProcessName.ToUpper() && x.ID != emailNotificationConfiguration.EmailConfigID).ToList();
                if (ProcessName != null && ProcessName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Process Name already exists.");
                }
                else
                {
                    await unitOfWork.EmailNotificationConfigurations.UpdateAsync(emailNotificationConfiguration);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var data1 = await unitOfWork.EmailNotificationConfigurations.GetAllAsync();
                var CompnyHasSameProcessName = data1.Where(x => x.CompneyName == emailNotificationConfiguration.CompneyName && x.ProcessName == emailNotificationConfiguration.ProcessName).ToList();

                if (CompnyHasSameProcessName != null && CompnyHasSameProcessName.Count >0)
                {
                    _toastNotification.AddWarningToastMessage("Company already has ProcessName " + emailNotificationConfiguration.ProcessName);
                }
                else
                {
                    var savevalue = await unitOfWork.EmailNotificationConfigurations.AddAsync(emailNotificationConfiguration);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editEmailNotificationConfig(EmailNotificationConfiguration emailNotificationConfiguration)
        {
            var data = await unitOfWork.EmailNotificationConfigurations.GetByIdAsync(emailNotificationConfiguration.EmailConfigID);
            return Json(new
            {
                editEmailNotificationConfig = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteEmailNotificationConfig(EmailNotificationConfiguration emailNotificationConfiguration)
        {
            var data = await unitOfWork.EmailNotificationConfigurations.DeleteAsync(emailNotificationConfiguration.EmailConfigID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
