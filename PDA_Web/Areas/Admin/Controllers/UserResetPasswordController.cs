﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Areas;
using NToastNotify;
using PDA_Web.Controllers;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

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
            if (resetPassword != null)
            {
                var ChekCustomer = unitOfWork.User.ChangePassword(resetPassword.Password, resetPassword.userId);
                return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });   
            }
            else
            {
                _toastNotification.AddWarningToastMessage("Password Can't Changed!!!");
                return View();
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
                var SetPassword = unitOfWork.User.ChangePassword(Data.NewPassword, Data.userId);
            }


            return View();
        }
    }
}

