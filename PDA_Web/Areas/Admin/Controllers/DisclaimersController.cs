using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure_Shared.Services;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DisclaimersController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        private readonly IEmailSender _emailSender;

        public DisclaimersController(IUnitOfWork unitOfWork, IToastNotification toastNotification, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Index()
        {
            //var message = new Message(new string[] { "patelalpeshn@gmail.com" }, "Test email", "This is the content from our email.");
            //_emailSender.SendEmail(message);
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
        public async Task<ActionResult> DisclaimerSave(Disclaimers Disclaimers)
        {

            if (Disclaimers.ID > 0)
            {
                var Notsdata = await unitOfWork.Disclaimers.GetAllAsync();
                var DisclaimerName = Notsdata.Where(x => x.Disclaimer.ToUpper() == Disclaimers.Disclaimer.ToUpper() && x.ID != Disclaimers.ID).ToList();
                if (DisclaimerName.Count > 0 && DisclaimerName != null)
                {
                    _toastNotification.AddWarningToastMessage("Disclaimer is Already is exist.");
                }
                else
                {
                   
                    await unitOfWork.Disclaimers.UpdateAsync(Disclaimers);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var Notsdata = await unitOfWork.Disclaimers.GetAllAsync();
                var DisclaimerName = Notsdata.Where(x => x.Disclaimer.ToUpper() == Disclaimers.Disclaimer.ToUpper()).ToList();
                if (DisclaimerName != null && DisclaimerName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Disclaimer is Already is exist.");
                }
                else
                {

                    await unitOfWork.Disclaimers.AddAsync(Disclaimers);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
        public async Task<IActionResult> LoadAll()
        {
            var data = await unitOfWork.Disclaimers.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            return PartialView("partial/_ViewAll", data);
        }
        public async Task<ActionResult> editDisclaimer(Disclaimers Disclaimers)
        {
            var data = await unitOfWork.Disclaimers.GetByIdAsync(Disclaimers.ID);
            return Json(new
            {
                Disclaimers = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteDisclaimer(Disclaimers Disclaimers)
        {
            var data = await unitOfWork.Disclaimers.DeleteAsync(Disclaimers.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }

}
