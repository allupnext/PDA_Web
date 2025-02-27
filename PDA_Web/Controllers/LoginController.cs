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
using System.Net.NetworkInformation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PDA_Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<LoginController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IToastNotification _toastNotification;
        private readonly IConfiguration _configuration;

        public LoginController(ILogger<LoginController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification, IEmailSender emailSender, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> Index(CustomerAuth customerAuth)
        {
            string CookieskeyMacAddress = "MacAddress";

            //var MachineName = Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName;

            if (customerAuth != null)
            {
             //   var macAddress = NetworkInterface
             //.GetAllNetworkInterfaces()
             //            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
             //            .Select(nic => nic.GetPhysicalAddress().ToString())
             //            .FirstOrDefault();


                CustomerUserMaster isAuthenticated = await unitOfWork.Customer.Authenticate(customerAuth.Email, customerAuth.CustomerPassword);
                if (isAuthenticated != null)
                {
                    var Companydata = await unitOfWork.Customer.GetByIdAsync(isAuthenticated.CustomerId);
                    if (Companydata.Status != "Active")
                    {
                        _toastNotification.AddErrorToastMessage("Your company is not active. Please contact to Admin (Email on: bulkopsindia@samsarashipping.com).");
                        return Json(new
                        {
                            proceed = false,
                            msg = "",
                            otp = ""
                        });
                    }
                    else if (!isAuthenticated.Status)
                    {
                        _toastNotification.AddErrorToastMessage("You are not active user. Please contact to Admin (Email on: bulkopsindia@samsarashipping.com).");
                        return Json(new
                        {
                            proceed = false,
                            msg = "",
                            otp = ""
                        });

                    }
                    //if (isAuthenticated.MacAddress != "" || isAuthenticated.MacAddress != null)
                    //{
                    //    var Res = await unitOfWork.Customer.UpdateMacAddress(customerAuth.MacAddress, isAuthenticated.ID);
                    //}

                    //CustomerUserMaster isMacAddress = await unitOfWork.Customer.Authenticate(customerAuth.Email, customerAuth.CustomerPassword);

                    ////if (isMacAddress.MacAddress == null || isMacAddress.MacAddress == "")
                    //{
                    //    _toastNotification.AddErrorToastMessage("Enter Your MacAddress");
                    //    return Json(new
                    //    {
                    //        proceed = false,
                    //        msg = "",
                    //        otp = ""
                    //    });
                    //}


                    if ((string.IsNullOrEmpty(customerAuth.MacAddress)) && (string.IsNullOrEmpty(isAuthenticated.MacAddress)))
                    {
                        _toastNotification.AddErrorToastMessage("Enter Your MacAddress");
                        return Json(new
                        {
                            proceed = false,
                            msg = "MacAddress",
                            otp = ""
                        });
                    }
                    else if (!string.IsNullOrEmpty(customerAuth.MacAddress))
                    {

                        var ResMacAddress = await unitOfWork.CustomerUserMaster.AddMacAddress(customerAuth.MacAddress, isAuthenticated.ID);
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(30), // Expires in 30 days
                            IsEssential = true, // Necessary for the application to function
                            HttpOnly = true, // Accessible only by the server
                            Secure = true // Only sent over HTTPS
                        };
                        Response.Cookies.Append(CookieskeyMacAddress, customerAuth.MacAddress, cookieOptions);
                    }

                    var isMacIDCheck = _configuration.GetValue<bool>("MacIDCheck");
                    if (isMacIDCheck)
                    {
                        var CookieValueMacAddress = Request.Cookies[CookieskeyMacAddress];

                        if (string.IsNullOrEmpty(isAuthenticated.MacAddress))
                        {
                            //var AddMacAddress = await unitOfWork.CustomerUserMaster.AddMacAddress(macAddress, isAuthenticated.ID);
                            string otp = await SendOTPEmail(isAuthenticated.Email, isAuthenticated.ID);
                            return Json(new
                            {
                                proceed = true,
                                msg = "otpsent",
                                otp = otp
                            });

                        }
                        else if (CookieValueMacAddress != isAuthenticated.MacAddress)
                        {
                            //var LoginMachineName = MachineName;
                            //if (isAuthenticated.LoginMachineName != null && isAuthenticated.LoginMachineName == LoginMachineName)
                            //{
                            //    string otp = await SendOTPEmail(isAuthenticated.Email, isAuthenticated.ID);
                            //    return Json(new
                            //    {
                            //        proceed = true,
                            //        msg = "",
                            //        otp = otp
                            //    });
                            //}
                            //else
                            //{
                            if (CookieValueMacAddress != null)
                            {
                                _toastNotification.AddErrorToastMessage("This device is not registered. Please login through your registered device Or reset your password");
                                return Json(new
                                {
                                    proceed = false,
                                    msg = "",
                                    otp = ""
                                });
                                //}
                            }
                            else
                            {
                                _toastNotification.AddErrorToastMessage("This device is not registered. Please login through your registered device (" + CookieValueMacAddress + ") Or reset your password");
                                return Json(new
                                {
                                    proceed = false,
                                    msg = "",
                                    otp = ""
                                });
                            }
                        }
                        else
                        {
                            string otp = await SendOTPEmail(isAuthenticated.Email, isAuthenticated.ID);
                            return Json(new
                            {
                                proceed = true,
                                msg = "otpsent",

                                otp = otp
                            });
                        }

                    }
                    else
                    {
                        string otp = await SendOTPEmail(isAuthenticated.Email, isAuthenticated.ID);
                        return Json(new
                        {
                            proceed = true,
                            msg = "otpsent",
                            otp = otp
                        });
                    }
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("You have entered an invalid username or password.");
                    return Json(new
                    {
                        proceed = false,
                        msg = "",
                        otp = ""
                    });
                }
            }
            else
            {
                _toastNotification.AddErrorToastMessage("You have entered an invalid username or password.");
                return Json(new
                {
                    proceed = false,
                    msg = "",
                    otp = ""
                });
            }
        }

        public async Task<string> SendOTPEmail(string Email, int Id)
        {
            int PrimaryCompanyId = 0;

            var CustomerUserData = await unitOfWork.CustomerUserMaster.GetCustomerUserByEmailAsync(Email);
            if (CustomerUserData != null && CustomerUserData.Count > 0)
            {
                var CustomerId = CustomerUserData.Select(x => x.CustomerId).First();
                
                var corecustomerdata = await unitOfWork.Customer.GetByIdAsync(Convert.ToInt32(CustomerId));

                PrimaryCompanyId = Convert.ToInt32(corecustomerdata.PrimaryCompany);
                //var FromPrimaryCompany = await unitOfWork.Company.GetByIdAsync(PrimaryCompanyId);
                //var PrimaryCompnayName = FromPrimaryCompany.CompanyName;
            }
            string otp = (GenerateOTP.NextInt() % 1000000).ToString("000000");
            await unitOfWork.CustomerUserMaster.UpdateOTP(otp, DateTime.UtcNow, Id);

            List<string> recipients = new List<string>
            {
                Email
            };
            string Content = "<div style='font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2'>"
   + "<div style='margin:50px auto;width:70%;padding:20px 0'>"
   + "<div style='border-bottom:1px solid #eee'>"
   + "  <a href='' style='font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600'>PDA Portal</a>"
   + "</div>"
   + "<p style='font-size:1.1em'>Hi,</p>"
   + "<p>Thank you for choosing PDA Portal. Use the following OTP to complete your Sign Up procedures. OTP is valid for 5 minutes</p>"
   + "<h2 style='background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;'>" + otp + "</h2>"
   + "<p style='font-size:0.9em;'>Regards,<br />PDA Portal Team</p>"
   + "<hr style='border:none;border-top:1px solid #eee' />"
   + "<div style='float:right;padding:8px 0;color:#aaa;font-size:0.8em;line-height:1;font-weight:300'>"
   + "<p></p>"
    + "</div> </div></div> ";
            string Subject = "EPDA Portal: Login OTP";
            List<string> ccrecipients = new List<string>();
            string FromCompany = "";
            string ToEmail = "";
            var emailconfig = await unitOfWork.EmailNotificationConfigurations.GetByCompanyandProcessNameAsync(PrimaryCompanyId, "Customer OTP Sent Email");
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
            _emailSender.SendEmail(Msg);

            return otp;
        }

        [HttpPost]
        public async Task<ActionResult> LoginwitOTP(CustomerAuth customerAuth)
        {
            var isAuthenticated = await unitOfWork.Customer.Authenticate(customerAuth.Email, customerAuth.CustomerPassword);
            if (isAuthenticated.OTP == customerAuth.OTP && isAuthenticated.OTPSentDate != null && isAuthenticated.OTPSentDate.Value.AddMinutes(5) > DateTime.UtcNow)
            {
                var LoginMachineName = System.Environment.MachineName;
                DateTime LoginDateTime = DateTime.UtcNow;
                await unitOfWork.CustomerUserMaster.UpdateLoginDetails(LoginMachineName, LoginDateTime, isAuthenticated.ID);

                HttpContext.Session.SetString("CustID", isAuthenticated.CustomerId.ToString());
                HttpContext.Session.SetString("ID", isAuthenticated.ID.ToString());

                return Json(new
                {
                    proceed = true,
                    msg = ""
                });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("You have entered an invalid OTP.");
                return Json(new
                {
                    proceed = false,
                    msg = ""
                });
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
                        string Content = "<html> <body>   <p>Hello, <br> You recently requested to reset the password for your PDA Portal account. Click the button below to proceed.    </p> <div> <a  href=" + confirmationLink + "> <button style='height:30px; margin-bottom:30px; font-size:14px;' type='button'> Reset Password </button> </a> </div> </body> </html> ";
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
