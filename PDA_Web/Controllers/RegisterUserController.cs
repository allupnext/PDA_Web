using Microsoft.AspNetCore.Mvc;
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
            var CountryData = await unitOfWork.Countrys.GetAllAsync();
            ViewBag.Country = CountryData;
            ViewBag.CountryCode = CountryData.Select(x => x.CountryCode).ToList();

            var PrimaryCompanyData = await unitOfWork.Company.GetAllAsync();
            ViewBag.PrimaryCompany = PrimaryCompanyData;

            var SecondryCompanyData = await unitOfWork.Company.GetAllAsync();
            ViewBag.SecondaryCompany = SecondryCompanyData;

            var BankData = await unitOfWork.BankMaster.GetAllBankDetailsAsync();
            ViewBag.BankData = BankData;
            return View();
        }

        public async Task<ActionResult> RegisterUserSave(Customer customer)
        {
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
            }
            _toastNotification.AddSuccessToastMessage("Your Company register request sent successfully.");
            return Json(new
            {
                proceed = true,
                msg = ""
            });

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
