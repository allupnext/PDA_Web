using Humanizer;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure_Shared;
using PDAEstimator_Infrastructure_Shared.Services;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLoginController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IToastNotification _toastNotification;
        public AdminLoginController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
            
        }

        [HttpPost]
        public async Task<ActionResult> Index(UserAuth user)
        {
            if (user != null)
            {
                User isAuthenticated = await unitOfWork.User.Authenticate(user.EmployCode, user.UserPassword);
     
                
                if (isAuthenticated != null)
                {
                    HttpContext.Session.SetString("UserID", isAuthenticated.ID.ToString());
                   
                    return RedirectToAction("Index", "Home");
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
            if (Email != null)
            {
                var EmailExist = await unitOfWork.User.CheckEmailExist(Email);
                if (EmailExist != null)
                {
                    Random random = new Random();
                    int length = Email.Length; // Desired string length
                    string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    char[] randomString = new char[length];

                    for (int i = 0; i < length; i++)
                    {
                        randomString[i] = characters[random.Next(characters.Length)];
                    }

                    string token = new string(randomString);
                    await unitOfWork.User.GenerateEmailConfirmationTokenAsync(token, EmailExist.ID);


                    var confirmationLink = Url.Action("ForgotUserPasswordIndex", "UserResetPassword",
                    new { userId = EmailExist.ID, token = token }, Request.Scheme);
                    _logger.Log(Microsoft.Extensions.Logging.LogLevel.Warning, confirmationLink);

                    /*                    Message Msg = new Message();
                                        Msg.Content = confirmationLink;
                                        Msg.Subject = "To_ResetPassword_Link";
                                        Msg.To[0] = new MailboxAddress("Recipient: ", Email);
                                        Msg*/
                    List<string> recipients = new List<string>
                    {
                        Email
                    };
                    string Content = confirmationLink;
                    string Subject = "To_ResetPassword_Link";

                    var Msg = new Message(recipients, Subject, Content);
 /*                   _toastNotification.AddSuccessToastMessage("Email hase been sent to given Email Address");*/
                    _emailSender.SendEmail(Msg);
                   
                }
                else
                {
                    return View();
                }
            }
            return View();
        }




        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.SetString("UserID", "");
            return RedirectToAction("Index", "AdminLogin");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
