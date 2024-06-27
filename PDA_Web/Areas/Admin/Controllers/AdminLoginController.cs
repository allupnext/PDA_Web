using Humanizer;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure_Shared;
using PDAEstimator_Infrastructure_Shared.Services;
using System.Configuration;
using System.Net.NetworkInformation;
using static System.Net.WebRequestMethods;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLoginController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IToastNotification _toastNotification;
        private readonly IConfiguration _configuration;

        public AdminLoginController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification, IEmailSender emailSender, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> Index(UserAuth user)
        {
            var macAddress = NetworkInterface
                .GetAllNetworkInterfaces()
                            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                            .Select(nic => nic.GetPhysicalAddress().ToString())
                            .FirstOrDefault();


            if (user != null)
            {
                User isAuthenticated = await unitOfWork.User.Authenticate(user.EmployCode, user.UserPassword);
                if (isAuthenticated.EmailID == null || isAuthenticated.EmailID == "")
                {
                    _toastNotification.AddErrorToastMessage("We did't not found User Email Id. Please conact to admin.");
                    return Json(new
                    {
                        proceed = false,
                        msg = "",
                        otp = ""
                    });
                }

                if (isAuthenticated != null)
                {

                    var isMacIDCheck = _configuration.GetValue<bool>("MacIDCheck");
                    if (isMacIDCheck)
                    {
                        //var macAddressa = unitOfWork.User.GetAllAsync().Result.Where(x => x.MacAddress == macAddress && x.EmployCode == user.EmployCode);

                        if (string.IsNullOrEmpty(isAuthenticated.MacAddress))
                        {
                            var AddMacAddress = await unitOfWork.User.AddMacAddress(macAddress, isAuthenticated.ID);
                            string otp = await SendOTPEmail(isAuthenticated.EmailID, isAuthenticated.ID);
                            return Json(new
                            {
                                proceed = true,
                                msg = "",
                                otp = otp
                            });

                        }
                        else if (isAuthenticated.MacAddress != macAddress)
                        {
                            _toastNotification.AddErrorToastMessage("Your MACAddress does not match with old MACAddress.");
                            return Json(new
                            {
                                proceed = false,
                                msg = "",
                                otp = ""
                            });
                        }
                        else
                        {
                            string otp = await SendOTPEmail(isAuthenticated.EmailID, isAuthenticated.ID);
                            return Json(new
                            {
                                proceed = true,
                                msg = "",
                                otp = otp
                            });
                        }
                    }
                    else
                    {
                        string otp = await SendOTPEmail(isAuthenticated.EmailID, isAuthenticated.ID);
                        return Json(new
                        {
                            proceed = true,
                            msg = "",
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

        [HttpPost]
        public async Task<ActionResult> LoginwitOTP(UserAuth user)
        {
            User isAuthenticated = await unitOfWork.User.Authenticate(user.EmployCode, user.UserPassword);
            if (isAuthenticated.OTP == user.OTP && isAuthenticated.OTPSentDate != null && isAuthenticated.OTPSentDate.Value.AddMinutes(5) > DateTime.UtcNow)
            {
                var LoginMachineName = System.Environment.MachineName;
                DateTime LoginDateTime = DateTime.UtcNow;
                await unitOfWork.User.UpdateLoginDetails(LoginMachineName, LoginDateTime, isAuthenticated.ID);

                HttpContext.Session.SetString("UserID", isAuthenticated.ID.ToString());
                //return RedirectToAction("Index", "Home");

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

        public async Task<string> SendOTPEmail(string Email, int Id)
        {
            //var CustomerUserData = await unitOfWork.CustomerUserMaster.GetCustomerUserByEmailAsync(Email);
            //var CustomerId = CustomerUserData.Select(x => x.CustomerId).First();
            //var corecustomerdata = await unitOfWork.Customer.GetByIdAsync(Convert.ToInt32(CustomerId));
            //Int64 PrimaryCompanyId = Convert.ToInt64(corecustomerdata.PrimaryCompany);
            //var FromPrimaryCompany = await unitOfWork.Company.GetByIdAsync(PrimaryCompanyId);
            //var PrimaryCompnayName = FromPrimaryCompany.CompanyName;

            string otp = (GenerateOTP.NextInt() % 1000000).ToString("000000");
            await unitOfWork.User.UpdateOTP(otp, DateTime.UtcNow, Id);

            List<string> recipients = new List<string>
            {
                Email
            };
            string Content = "<div style='font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2'>"
   + "<div style='margin:50px auto;width:70%;padding:20px 0'>"
   + "<div style='border-bottom:1px solid #eee'>"
   + "  <a href='' style='font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600'>samsara PDA Estimation</a>"
   + "</div>"
   + "<p style='font-size:1.1em'>Hi,</p>"
   + "<p>Thank you for choosing samsara PDA Estimation. Use the following OTP to complete your Sign Up procedures. OTP is valid for 5 minutes</p>"
   + "<h2 style='background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;'>" + otp + "</h2>"
   + "<p style='font-size:0.9em;'>Regards,<br />samsara PDA Estimation Team</p>"
   + "<hr style='border:none;border-top:1px solid #eee' />"
   + "<div style='float:right;padding:8px 0;color:#aaa;font-size:0.8em;line-height:1;font-weight:300'>"
   + "<p>Samsara</p>"
    + "</div> </div></div> ";
            string Subject = "Login in PDA Estimation";
            string FromCompany = "";
            //if (PrimaryCompnayName == "Merchant Shipping Services Private Limited")
            //{
            //    FromCompany = "FromMerchant";
            //}
            //if (PrimaryCompnayName == "Samsara Shipping Private Limited")
            //{
            FromCompany = "FromSamsara";
            //}


            var Msg = new Message(recipients, Subject, Content, FromCompany);
            _emailSender.SendEmail(Msg);

            return otp;
        }

        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (Email != null)
            {
                var EmailExist = await unitOfWork.User.CheckEmailExist(Email);
                if (EmailExist != null)
                {
                    var CustomerUserData = await unitOfWork.CustomerUserMaster.GetCustomerUserByEmailAsync(Email);

                    var CustomerId = CustomerUserData.Select(x => x.CustomerId).First();

                    var corecustomerdata = await unitOfWork.Customer.GetByIdAsync(Convert.ToInt32(CustomerId));

                    Int64 PrimaryCompanyId = Convert.ToInt64(corecustomerdata.PrimaryCompany);

                    var FromPrimaryCompany = await unitOfWork.Company.GetByIdAsync(PrimaryCompanyId);
                    var PrimaryCompnayName = FromPrimaryCompany.CompanyName;

                    /*                    Int64 PrimaryCompanyId = Convert.ToInt64(CustomerUserData.);*/


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
                    /*                    string Content = confirmationLink;*/
                    /*                    string Content = "You recently requested to reset the password for your PDAEstimator account. Click the button below to proceed. ";*/
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
