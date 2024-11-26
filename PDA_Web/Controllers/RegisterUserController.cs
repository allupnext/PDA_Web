using iTextSharp.tool.xml.html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;
using PDAEstimator_Infrastructure_Shared;
using PDAEstimator_Infrastructure_Shared.Services;

namespace PDA_Web.Controllers
{
    public class RegisterUserController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<RegisterUserController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IToastNotification _toastNotification;

        public RegisterUserController(ILogger<RegisterUserController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Designations = await unitOfWork.Designation.GetAllAsync();

            var CountryData = await unitOfWork.Countrys.GetAllAsync();
            ViewBag.Country = CountryData;
            ViewBag.CountryCode = CountryData.Select(x => x.CountryCode).ToList();

            var StateData = await unitOfWork.States.GetAllAsync();
            ViewBag.State = StateData;

            var CityData = await unitOfWork.Citys.GetAllAsync();
            ViewBag.City = CityData;


            //var PrimaryCompanyData = await unitOfWork.Company.GetAllAsync();
            //ViewBag.PrimaryCompany = PrimaryCompanyData;

            //var SecondryCompanyData = await unitOfWork.Company.GetAllAsync();
            //ViewBag.SecondaryCompany = SecondryCompanyData;

            //var BankData = await unitOfWork.BankMaster.GetAllBankDetailsAsync();
            //ViewBag.BankData = BankData;
            return View();
        }

        public async Task<ActionResult> RegisterUserSave(Customer customer)
        {
            string mailcontent = "";
            string emailsubject = "";
            var customerdata = await unitOfWork.CustomerUserMaster.GetAllAsync();

            var CMobileNumber = customerdata.Where(x => x.Mobile == customer.Mobile).ToList();
            var CEmailId = customerdata.Where(x => x.Email.ToUpper() == customer.Email.ToUpper()).ToList();
            if (CMobileNumber.Count > 0 && CMobileNumber != null || CEmailId.Count > 0 && CEmailId != null)
            {
                _toastNotification.AddWarningToastMessage("MobileNumber Or Email Exist!..");
                return Json(new
                {
                    proceed = false,
                    msg = ""
                });
            }

            var companyexist = customerdata.Where(x => x.Company == customer.Company).ToList();
            if (companyexist.Count > 0)
            {
                _toastNotification.AddWarningToastMessage("Your company is already registered. Please conact to admin.");
                return Json(new
                {
                    proceed = false,
                    msg = ""
                });

            }

            //var userid = HttpContext.Session.GetString("UserID");
            customer.CreatedDate = DateTime.UtcNow;

            var custId = await unitOfWork.Customer.AddAsync(customer);
            if (!string.IsNullOrEmpty(custId))
            {
                if (customer.PrimaryCompanyId != null)
                {
                    Company_Customer_Mapping company_Customer_Mapping = new Company_Customer_Mapping();

                    company_Customer_Mapping.CustomerID = Convert.ToInt32(custId);
                    company_Customer_Mapping.CompanyID = (int)customer.PrimaryCompanyId;
                    company_Customer_Mapping.IsPrimary = true;
                    await unitOfWork.Customer.AddCustomer_Company_MappingAsync(company_Customer_Mapping);
                }
                if (customer.SecondaryCompanyId != null)
                {
                    Company_Customer_Mapping company_Customer_Mapping1 = new Company_Customer_Mapping();

                    foreach (int i in customer.SecondaryCompanyId)
                    {
                        company_Customer_Mapping1 = new Company_Customer_Mapping();
                        company_Customer_Mapping1.CustomerID = Convert.ToInt32(custId);
                        company_Customer_Mapping1.CompanyID = i;
                        company_Customer_Mapping1.IsPrimary = false;
                        await unitOfWork.Customer.AddCustomer_Company_MappingAsync(company_Customer_Mapping1);
                    }
                }

                CustomerUserMaster customerUserMaster = new CustomerUserMaster();
                customerUserMaster.CustomerId = Convert.ToInt32(custId);
                customerUserMaster.Address1 = customer.Address1;
                customerUserMaster.City = customer.City;
                customerUserMaster.State = customer.State != null ? Convert.ToInt32(customer.State) : 0;
                customerUserMaster.Country = customer.Country;
                customerUserMaster.CountryCode = customer.CountryCode;
                customerUserMaster.Designation = customer.Designation;
                customerUserMaster.Email = customer.Email;
                customerUserMaster.Mobile = customer.Mobile;
                customerUserMaster.Salutation = customer.Salutation;
                customerUserMaster.IsDeleted = false;
                customerUserMaster.FirstName = customer.FirstName;
                customerUserMaster.LastName = customer.LastName;
                customerUserMaster.Password = PasswordGenerator.GeneratePassword(8);
                await unitOfWork.CustomerUserMaster.AddAsync(customerUserMaster);
                string customerfullname = string.Concat(customerUserMaster.FirstName, ' ', customerUserMaster.LastName);
                string customerphone = string.Concat(customer.CountryCode, ' ', customer.Mobile);
                mailcontent = "<html> <head> <title> Registration request received from.</title> </head><body><p> Dear Team,<br> Registration request received from " + customer.Company + ".</p><ul><li>Registered Email/Login: - " + customer.Email + "</li><li>Mobile - " + customerphone + "  </li></ul> </br> <p> Kindly approve request in portal.<br><br> <b>Best Regards <br> PDA Portal Team</b></p> </body> </html>";
                emailsubject = "EPDA Portal: New Registration Request";

                var companydata = await unitOfWork.Company.GetAlllistAsync();
                int Samsaracompanyid = 0;
                if (companydata != null && companydata.Count > 0)
                {
                    var samsaracompanydata = companydata.Where(x => x.CompanyName.ToLower() == "samsara shipping private limited");
                    if(samsaracompanydata.Count() > 0)
                    {
                        Samsaracompanyid = samsaracompanydata.FirstOrDefault().CompanyId;
                    }
                }


                if(Samsaracompanyid > 0)
                    CustomerRegisterEmail("Customer Register", mailcontent, emailsubject, "", Samsaracompanyid);

                mailcontent = "<html><head><title> Thank you for registering on PDA Portal.</title></head><body><p> Dear "+ customerfullname + ",<br> We have received your Registration request with below detail.</br> </p><ul><li>Registered Email/Login: - " + customer.Email + "</li><li>Mobile - " + customerphone + "  </li></ul> </br> <p>  <br> You'll shortly receive your Login Credentials.<br><br> <b>Best Regards <br> PDA Portal Team</b> </p></body></html>";
                emailsubject = "EPDA Portal: New Registration Request";

                if (Samsaracompanyid > 0)
                    CustomerRegisterThankYouEmail("Customer Register", mailcontent, emailsubject, customer.Email, Samsaracompanyid);

                _toastNotification.AddSuccessToastMessage("Register request sent successfully");
                return Json(new
                {
                    proceed = true,
                    msg = ""
                });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Issue on register request. Please contact to Admin.");
                return Json(new
                {
                    proceed = false,
                    msg = ""
                });

            }

        }

        public async Task<bool> CustomerRegisterEmail(string processname, string mailcontent, string emailsubject, string customeremail, int companyid)
        {
          

            string Content = mailcontent;
            string Subject = emailsubject;

            List<string> ccrecipients = new List<string>();
            string FromCompany = "";
            string ccEmail = "";
            var emailconfig = await unitOfWork.EmailNotificationConfigurations.GetByCompanyandProcessNameAsync(companyid, processname);

            if (emailconfig != null)
            {
                ccEmail = emailconfig.ToEmail;
                FromCompany = emailconfig.FromEmail;
                if (emailconfig.ToEmail != null)
                {
                    ccrecipients = ccEmail.Split(',').ToList();
                }
                FromCompany = emailconfig.FromEmail;

            }

            List<string> recipients = new List<string>
                {
                    customeremail
                };
            var Msg = new Message(recipients, ccrecipients, Subject, Content, FromCompany);

            _emailSender.SendEmail(Msg);
            return true;
        }

        public async Task<bool> CustomerRegisterThankYouEmail(string processname, string mailcontent, string emailsubject, string customeremail, int companyid)
        {
            List<string> recipients = new List<string>
                {
                    customeremail
                };

            string Content = mailcontent;
            string Subject = emailsubject;

            List<string> ccrecipients = new List<string>();
            string FromCompany = "";
            string ccEmail = "";
            var emailconfig = await unitOfWork.EmailNotificationConfigurations.GetByCompanyandProcessNameAsync(companyid, processname);

            if (emailconfig != null)
            {
                ccEmail = emailconfig.ToEmail;
                FromCompany = emailconfig.FromEmail;
                if (emailconfig.ToEmail != null)
                {
                    ccrecipients = ccEmail.Split(',').ToList();
                }
            }

            var Msg = new Message(recipients, ccrecipients, Subject, Content, FromCompany);

            _emailSender.SendEmail(Msg);
            return true;
        }

        public IActionResult CountryOnchange(CustomerUserMaster customer)
        {
            var StateData = unitOfWork.States.GetAllAsync().Result.Where(x => x.CountryId == customer.Country);
            //var CityData = unitOfWork.Citys.GetCitylistByCountry(customer.Country, 0);

            //ViewBag.City = CityData;
            ViewBag.State = StateData;
            return PartialView("partial/StatesList");
        }
        public IActionResult StateOnchange(PortDetails portDetails)
        {
            var CityData = unitOfWork.Citys.GetAllAsync().Result.Where(x => x.StateId == portDetails.State);

            ViewBag.City = CityData;
            return PartialView("partial/CityList");
        }
        public async Task<IActionResult> CountryOnChangeforCountryCode(Customer customer)
        {
            var CountryData = await unitOfWork.Countrys.GetCountryCodeByCountryIdAsync(customer.Country);
            ViewBag.CountryCode = CountryData;
            return Json(new
            {
                code = CountryData.CountryCode,
                proceed = true,
                msg = ""
            });
        }

        /*        public async Task<IActionResult> CountryOnChangeforCountryCode(CustomerUserMaster customer)
                {
                    var CountryData = await unitOfWork.Countrys.GetCountryCodeByCountryIdAsync(customer.Country);
                    ViewBag.CountryCode = CountryData;
                    return Json(new
                    {
                        code = CountryData.CountryCode,
                        proceed = true,
                        msg = ""
                    });
                }
                public IActionResult CountryOnchangeforCity(CustomerUserMaster customer)
                {
                    var CountryData = unitOfWork.Countrys.GetAllAsync();

                    var CityData = unitOfWork.Citys.GetCitylistByCountry(customer.Country, 0).Result;
                    ViewBag.City = CityData;
                    return PartialView("partial/CityList");
                }*/

        public IActionResult PrimaryCompneySelected(Customer customer)
        {
            var BankData = unitOfWork.BankMaster.GetAllAsync().Result.Where(x => x.CompanyId == customer.PrimaryCompanyId);

            ViewBag.BankData = BankData;
            return PartialView("partial/BankList");
        }




    }
}
