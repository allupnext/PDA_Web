
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure_Shared;
using PDAEstimator_Infrastructure_Shared.Services;
using System.Net;

namespace PDA_Web.Controllers
{
    public class ResetPasswordController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<LoginController> _logger;
        private readonly IEmailSender _emailSender;

        private readonly IToastNotification _toastNotification;
        public ResetPasswordController(ILogger<LoginController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;
             _emailSender = emailSender;
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
                var MachineName = System.Environment.MachineName;

                string CookieskeyMacAddress = "MacAddress";
                var ChekCustomer = unitOfWork.Customer.ChangePassword(resetPassword.Password, resetPassword.userId, resetPassword.MacAddress, MachineName);
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // Expires in 30 days
                    IsEssential = true, // Necessary for the application to function
                    HttpOnly = true, // Accessible only by the server
                    Secure = true // Only sent over HTTPS
                };
                Response.Cookies.Append(CookieskeyMacAddress, resetPassword.MacAddress, cookieOptions);

                var custuserdata = await unitOfWork.CustomerUserMaster.GetByIdAsync(resetPassword.userId);
                if (custuserdata != null)
                {
                    string Email = custuserdata.Email;
                    var CustomerUserData = await unitOfWork.CustomerUserMaster.GetCustomerUserByEmailAsync(Email);

                    var CustomerId = CustomerUserData.Select(x => x.CustomerId).FirstOrDefault();

                    var corecustomerdata = await unitOfWork.Customer.GetByIdAsync(Convert.ToInt32(CustomerId));

                    Int64 PrimaryCompanyId = Convert.ToInt64(corecustomerdata.PrimaryCompany);

                    var FromPrimaryCompany = await unitOfWork.Company.GetByIdAsync(PrimaryCompanyId);
                    var PrimaryCompnayName = FromPrimaryCompany.CompanyName;


                    Random random = new Random();
                    int length = Email.Length; // Desired string length
                    string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    char[] randomString = new char[length];

                    for (int i = 0; i < length; i++)
                    {
                        randomString[i] = characters[random.Next(characters.Length)];
                    }

                    string token = new string(randomString);
                    await unitOfWork.Customer.GenerateEmailConfirmationTokenAsync(token, custuserdata.ID);


                    List<string> recipients = new List<string>
                    {
                        Email
                    };
                    string Content = "<html> <body> <div>  <p>Dear "+ custuserdata.FirstName +" " + custuserdata.LastName + ", <br> Password has been Reset. New Login Credentials: </br> <p> Registered Email/Login: "+ custuserdata.Email +" </p>  <p> Password: "+ custuserdata.Password +" </p> </br> Please email your feedback/suggestions to EPDA.Support@samsaragroup.com </br>  </div> </body> </html> ";
                    string Subject = "Reset your Password for PDA Portal";
                    List<string> ccrecipients = new List<string>();
                    string FromCompany = "";
                    string ToEmail = "";
                    var emailconfig = await unitOfWork.EmailNotificationConfigurations.GetByCompanyandProcessNameAsync((int)PrimaryCompanyId, "Customer Reset Password");
                    if (emailconfig != null)
                    {
                        ToEmail = emailconfig.ToEmail;
                        FromCompany = emailconfig.FromEmail;
                        if (emailconfig.ToEmail != null)
                        {
                            ccrecipients = ToEmail.Split(',').ToList();
                        }
                    }

                    var Msg = new Message(recipients, ccrecipients, Subject, Content, FromCompany);
                    /*            _toastNotification.AddSuccessToastMessage("Email hase been sent to given Email Address");*/
                    _emailSender.SendEmail(Msg);
                }

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
                    var SetPassword = unitOfWork.Customer.ChangePasswordByCurrent(Data.NewPassword, Data.userId);
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
