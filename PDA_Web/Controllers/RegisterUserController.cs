﻿using Microsoft.AspNetCore.Mvc;
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
            var customerdata = await unitOfWork.CustomerUserMaster.GetAllAsync();

            var CMobileNumber = customerdata.Where(x => x.Mobile == customer.Mobile).ToList();
            var CEmailId = customerdata.Where(x => x.Email.ToUpper() == customer.Email.ToUpper()).ToList();
            if (CMobileNumber.Count > 0 && CMobileNumber != null || CEmailId.Count > 0 && CEmailId != null)
            {
                _toastNotification.AddWarningToastMessage("MobileNumber Or Email Exist!..");
            }
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
                customerUserMaster.State = customer.State != null ? Convert.ToInt32(customer.State): 0;
                customerUserMaster.Country = customer.Country;
                customerUserMaster.CountryCode = customer.CountryCode;
                customerUserMaster.Designation = customer.Designation;
                customerUserMaster.Email = customer.Email;
                customerUserMaster.Mobile = customer.Mobile;
                customerUserMaster.Salutation = customer.Salutation;
                customerUserMaster.IsDeleted = false;
                customerUserMaster.FirstName = customer.FirstName;
                customerUserMaster.LastName = customer.LastName;

                await unitOfWork.CustomerUserMaster.AddAsync(customerUserMaster);
                List<string> recipients = new List<string>
                    {
                       customer.Email
                    };
                //string Content = "You are added in PDA Estimator and you can access our platform using below login credentials: </br> UserName :" + customer.Email + " User Password:" + customer.Password;
                string Content = "<html> <head>   <title> Your Company Register Request Successfully Sent to us.</title> </head> <body>   <p> Dear User,<br>    Thanks for registering on the PDA portal. Our Admin will check your request and conact you soon. <br><br> <b>Regards <br> PDA Portal</b>  </p> </body> </html> ";
                string Subject = "Company Register Requiest on PDAEstimator";
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

/*        public IActionResult CountryOnchange(CustomerUserMaster customer)
        {
            var StateData = unitOfWork.States.GetAllAsync().Result.Where(x => x.CountryId == customer.Country);
            var CityData = unitOfWork.Citys.GetCitylistByCountry(customer.Country, 0);

            ViewBag.City = CityData;
            ViewBag.State = StateData;
            return PartialView("partial/StatesList");
        }

        public async Task<IActionResult> CountryOnChangeforCountryCode(CustomerUserMaster customer)
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
