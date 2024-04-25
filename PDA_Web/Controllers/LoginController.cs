    using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using NToastNotify;
using NuGet.Common;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure_Shared;
using PDAEstimator_Infrastructure_Shared.Services;
using System.Net;
using System.Net.Mail;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PDA_Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<LoginController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IToastNotification _toastNotification;
        public LoginController(ILogger<LoginController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification , IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<ActionResult> Index(CustomerAuth customerAuth)
        {
            if (customerAuth != null)
            {
                CustomerUserMaster isAuthenticated = await unitOfWork.Customer.Authenticate(customerAuth.Email, customerAuth.CustomerPassword);
                if (isAuthenticated != null)
                {
                    HttpContext.Session.SetString("CustID", isAuthenticated.CustomerId.ToString());
                    HttpContext.Session.SetString("ID", isAuthenticated.ID.ToString());
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> ForgotPassword(string Email)
        {
            try 
            {
                if (Email != null)
                {
                    var EmailExist = await unitOfWork.Customer.CheckEmailExist(Email);
                    if (EmailExist != null)
                    {
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
                        await unitOfWork.Customer.GenerateEmailConfirmationTokenAsync(token, EmailExist.ID);


                        var confirmationLink = Url.Action("ForgotPasswordIndex", "ResetPassword",
                       new { userId = EmailExist.ID, token = token }, Request.Scheme);
                        _logger.Log(Microsoft.Extensions.Logging.LogLevel.Warning, confirmationLink);

                        List<string> recipients = new List<string>
                    {
                        Email
                    };
                        string Content = "<html> <body>   <p>Hello, <br> You recently requested to reset the password for your PDAEstimator account. Click the button below to proceed.    </p> <div> <a  href=" + confirmationLink + "> <button style='height:30px; margin-bottom:30px; font-size:14px;' type='button'> Reset Password </button> </a> </div> </body> </html> ";
                        string Subject = "Reset Password";
                        string FromCompany = "";
                        if (PrimaryCompnayName == "Merchant Shipping Services Private Limited")
                        {
                            FromCompany = "FromMerchant";
                        }
                        if (PrimaryCompnayName == "Samsara Shipping Private Limited")
                        {
                            FromCompany = "FromSamsara";
                        }


                        var Msg = new Message(recipients, Subject, Content, FromCompany);
            /*            _toastNotification.AddSuccessToastMessage("Email hase been sent to given Email Address");*/
                        _emailSender.SendEmail(Msg);


                        return View();
                    }
                    else
                    {
                        return View();
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
         }

/*        public async Task<ActionResult> Register()
        {
            return View();
        }*/


        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.SetString("CustID", "");
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Index()
        {
            return View();
        }
   
    }
}
