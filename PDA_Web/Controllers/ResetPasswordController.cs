using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Controllers
{
    public class ResetPasswordController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<LoginController> _logger;

        private readonly IToastNotification _toastNotification;
        public ResetPasswordController(ILogger<LoginController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;

        }
        public IActionResult Index()
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
            return View();
        }
        public async Task<IActionResult> ForgotPasswordIndex(ResetPassword resetPassword)
        {            
            var ChekCustomer = unitOfWork.CustomerUserMaster.GetByIdAsync(resetPassword.userId).Result;
            if (ChekCustomer != null && (ChekCustomer.Token != null || ChekCustomer.Token != "" || ChekCustomer.Token == resetPassword.Token))
            {
                return View();
            }
            else 
            {
                //resetPassword.Error = 1;
                //return View(resetPassword);
                _toastNotification.AddWarningToastMessage("Please Give valid Token...");
                return View();
            }


        }
        public async Task<IActionResult> ChangePassword(ResetPassword resetPassword)
        {
            if (resetPassword != null)
            {
                var ChekCustomer = unitOfWork.Customer.ChangePassword(resetPassword.Password, resetPassword.userId);

                var data = new
                {
                    success = true,
                    message =  1
                };
                return Json(data);

            
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
          

            }

            //return View();
        }



        public async Task<IActionResult> ChangePasswordByCurrentPassword(ChangePasswordModel Data)
        {
            if(Data.CurrentPassword != null && Data.CurrentPassword != "") 
            {
                var Currentuser = HttpContext.Session.GetString("ID");
                Data.userId = Convert.ToInt32(Currentuser);
                var ChekUser = unitOfWork.Customer.AuthenticateById(Data.userId, Data.CurrentPassword);

                if (ChekUser.Result == 1)
                {
                    var SetPassword = unitOfWork.Customer.ChangePassword(Data.NewPassword, Data.userId);
                    _toastNotification.AddSuccessToastMessage("PassWord Set Successfully..");
                    var data = new
                    {
                        success = true,
                        message = "Operation completed successfully!"
                    };
                    return Json(data);
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
