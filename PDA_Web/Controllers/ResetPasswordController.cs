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

            
            var ChekCustomer = unitOfWork.Customer.GetByIdAsync(resetPassword.userId).Result;
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
                return View();
            }
            else
            {
                _toastNotification.AddWarningToastMessage("Password Can't Changed!!!");
                return View();

            }

            //return View();
        }


        public async Task<IActionResult> ChangePasswordByCurrentPassword(ChangePasswordModel Data)
        {
            if(Data.CurrentPassword != null && Data.CurrentPassword != "") 
            {
                var Currentuser = HttpContext.Session.GetString("CustID");
                Data.userId = Convert.ToInt32(Currentuser);
                var ChekUser = unitOfWork.Customer.AuthenticateById(Data.userId, Data.CurrentPassword);
                var SetPassword = unitOfWork.Customer.ChangePassword(Data.NewPassword, Data.userId);
                _toastNotification.AddSuccessToastMessage("PassWord Set Successfully..");
            }


            return View();
        }
    }
}
