using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Areas;
using NToastNotify;
using PDA_Web.Controllers;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Net.NetworkInformation;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserResetPasswordController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<AdminLoginController> _logger;


        private readonly IToastNotification _toastNotification;
        public UserResetPasswordController(ILogger<AdminLoginController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;

        }
        public async Task<IActionResult> ForgotUserPasswordIndex(ResetPassword resetPassword)
        {
            var User =  unitOfWork.User.GetByIdAsync(resetPassword.userId).Result;
            if (User != null && (User.Token != null || User.Token != "" || User.Token == resetPassword.Token))
            {

                return View();
            }
            else
            {
                _toastNotification.AddWarningToastMessage("Please Give valid Token...");
                return View();
            }
        }
        public async Task<IActionResult> ChangePassword(ResetPassword resetPassword)
        {
            string CookieskeyMacAddress = "MacAddress";
            //var macAddress = NetworkInterface
            //             .GetAllNetworkInterfaces()
            //	.Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            //	.Select(nic => nic.GetPhysicalAddress().ToString())
            //	.FirstOrDefault();

            //macAddress = resetPassword.MacAddress;

            if (resetPassword != null)
            {
                var ChekCustomer = unitOfWork.User.ChangePassword(resetPassword.Password, resetPassword.userId, resetPassword.MacAddress);
				var data = new
                {
				    success = true,
                    message = 1
                };

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // Expires in 30 days
                    IsEssential = true, // Necessary for the application to function
                    HttpOnly = true, // Accessible only by the server
                    Secure = true // Only sent over HTTPS
                };
                Response.Cookies.Append(CookieskeyMacAddress, resetPassword.MacAddress, cookieOptions);


                return Json(data);
                /*_toastNotification.AddSuccessToastMessage("Password New Password is Updated.");
                return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });*/
            }
            else
            {
                _toastNotification.AddWarningToastMessage("Password Can't Changed!!!");
                var data = new
                {
                    success = true,
                    message = 0
                };
                return Json(data);
              /*  return View();*/
            }
        }


        public async Task<IActionResult> ChangePasswordByCurrentPassword(ChangePasswordModel Data)
        {
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (Data.CurrentPassword != null && Data.CurrentPassword != "")
            {
                var Currentuserr = HttpContext.Session.GetString("UserID");
                Data.userId = Convert.ToInt32(Currentuserr);
                var ChekUser = unitOfWork.User.AuthenticateById(Data.userId, Data.CurrentPassword);

                if (ChekUser.Result == 1)
                {
                    var SetPassword = unitOfWork.User.ChangePassword(Data.NewPassword, Data.userId,Data.MacAddress);
                    _toastNotification.AddSuccessToastMessage("PassWord Set Successfully..");
                    var data = new
                    {
                        success = true,
                        message = "Operation completed successfully!"
                    };
                    return View(data);
                }
                else
                {
                    _toastNotification.AddWarningToastMessage("Password can't changed");
                    var data = new
                    {
                        success = true,
                        message = "Operation not completed successfully!"
                    };
                    return Json(data);
                }
                

            }


            return View();
        }
    }
}

