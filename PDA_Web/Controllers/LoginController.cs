    using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using NToastNotify;
using NuGet.Common;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
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
                Customer isAuthenticated = await unitOfWork.Customer.Authenticate(customerAuth.Email, customerAuth.CustomerPassword);
                if (isAuthenticated != null)
                {
                    HttpContext.Session.SetString("CustID", isAuthenticated.CustomerId.ToString());
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
                        Random random = new Random();
                        int length = Email.Length; // Desired string length
                        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                        char[] randomString = new char[length];

                        for (int i = 0; i < length; i++)
                        {
                            randomString[i] = characters[random.Next(characters.Length)];
                        }

                        string token = new string(randomString);
                        await unitOfWork.Customer.GenerateEmailConfirmationTokenAsync(token, EmailExist.CustomerId);


                        var confirmationLink = Url.Action("ForgotPasswordIndex", "ResetPassword",
                       new { userId = EmailExist.CustomerId, token = token }, Request.Scheme);
                        _logger.Log(Microsoft.Extensions.Logging.LogLevel.Warning, confirmationLink);
/*
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.From = new MailAddress("sonikeval8511@gmail.com");
                        mailMessage.To.Add(Email);
                        mailMessage.Subject = "Test The Link";
                        mailMessage.Body = "this is my mail body";


                        SmtpClient smtpClient = new SmtpClient();
                        smtpClient.Host = "h1d.f62.myftpupload.com";
                        smtpClient.Port = 22;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new System.Net.NetworkCredential("LTIxHJtGZI8cXv", "EJNnFJ6bVUza6r");
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mailMessage);*/


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
