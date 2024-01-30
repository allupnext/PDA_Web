﻿using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Diagnostics;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public CustomerController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;

        }

        public async Task<IActionResult> Index()
        {
            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                // Temp Solution START
                var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
                ViewBag.UserPermissionModel = UserPermissionModel;
                var Currentuser = HttpContext.Session.GetString("UserID");

                var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
                ViewBag.UserRoleName = UserRole;
                // Temp Solution END
                var CountryData = await unitOfWork.Countrys.GetAllAsync();
                ViewBag.Country = CountryData;
                ViewBag.CountryCode = CountryData.Select(x => x.CountryCode).ToList();

                var DesignationData = await unitOfWork.Designation.GetAllAsync();
                ViewBag.Designations = DesignationData;

                var StateData = await unitOfWork.States.GetAllAsync();
                ViewBag.State = StateData;

                var CityData = await unitOfWork.Citys.GetAllAsync();
                ViewBag.City = CityData;

                var PrimaryCompanyData = await unitOfWork.Company.GetAllAsync();
                ViewBag.PrimaryCompany = PrimaryCompanyData;

                var SecondryCompanyData = await unitOfWork.Company.GetAllAsync();
                ViewBag.SecondaryCompany = SecondryCompanyData;

                var BankData = await unitOfWork.BankMaster.GetAllAsync();
                ViewBag.BankData = BankData;

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }

        public async Task<IActionResult> LoadAll(CustomerList customer)
        {
            // var customerdata = await unitOfWork.Customer.GetAlllistCustomerAsync(customer.CustomerId);
            var customerdata = await unitOfWork.Customer.GetAlllistAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (customer.FirstName != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.FirstName == customer.FirstName).ToList();
            }
            if (customer.Country != null && customer.Country != 0)
            {
                customerdata = customerdata.Where(x => x.Country == customer.Country).ToList();
            }
            if (customer.Address1 != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.Address1 == customer.Address1).ToList();
            }
            if (customer.Email != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.Email == customer.Email).ToList();
            }
            if (customer.Company != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.Company == customer.Company).ToList();
            }
            return PartialView("partial/_ViewAll", customerdata);

        }

        public async Task<ActionResult> CustomerSave(Customer customer)
        {
            var customerdata = await unitOfWork.Customer.GetAlllistAsync();
            if (customer.CustomerId > 0)
            {
                var CMobileNumber = customerdata.Where(x => x.Mobile == customer.Mobile && x.CustomerId != customer.CustomerId).ToList();
                var CEmailId = customerdata.Where(x => x.Email.ToUpper() == customer.Email.ToUpper() && x.CustomerId != customer.CustomerId).ToList();
                if (CMobileNumber.Count > 0 && CMobileNumber != null || CEmailId.Count > 0 && CEmailId != null)
                {
                    _toastNotification.AddWarningToastMessage("MobileNumber Or Email Exist!..");
                }
                else
                {
                    await unitOfWork.Customer.UpdateAsync(customer);
                    await unitOfWork.Customer.DeleteCustomer_Company_MappingAsync(customer.CustomerId);
                    if (customer.PrimaryCompanyId != null)
                    {
                        Company_Customer_Mapping company_Customer_Mapping = new Company_Customer_Mapping();

                        company_Customer_Mapping.CustomerID = Convert.ToInt32(customer.CustomerId);
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
                            company_Customer_Mapping1.CustomerID = Convert.ToInt32(customer.CustomerId);
                            company_Customer_Mapping1.CompanyID = i;
                            company_Customer_Mapping1.IsPrimary = false;
                            await unitOfWork.Customer.AddCustomer_Company_MappingAsync(company_Customer_Mapping1);
                        }
                    }
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var CMobileNumber = customerdata.Where(x => x.Mobile == customer.Mobile).ToList();
                var CEmailId = customerdata.Where(x => x.Email.ToUpper() == customer.Email.ToUpper()).ToList();
                if (CMobileNumber.Count > 0 && CMobileNumber != null || CEmailId.Count > 0 && CEmailId != null)
                {
                    _toastNotification.AddWarningToastMessage("MobileNumber Or Email Exist!..");
                }
                else
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
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
        public IActionResult CountryOnchange(Customer customer)
        {
            var StateData = unitOfWork.States.GetAllAsync().Result.Where(x => x.CountryId == customer.Country);
            var CityData = unitOfWork.Citys.GetCitylistByCountry(customer.Country, 0);

            ViewBag.City = CityData;
            ViewBag.State = StateData;
            return PartialView("partial/StatesList");
        }

        public IActionResult CountryOnchangeforCity(Customer customer)
        {
            var CityData = unitOfWork.Citys.GetCitylistByCountry(customer.Country, 0).Result;

            ViewBag.City = CityData;
            return PartialView("partial/CityList");
        }

        public IActionResult StateOnchange(Customer customer)
        {
            var CityData = unitOfWork.Citys.GetAllAsync().Result.Where(x => x.StateId == customer.State);

            ViewBag.City = CityData;
            return PartialView("partial/CityList");
        }

        public IActionResult PrimaryCompneySelected(Customer customer)
        {
            var BankData = unitOfWork.BankMaster.GetAllAsync().Result.Where(x => x.CompanyId == customer.PrimaryCompanyId);

            ViewBag.BankData = BankData;
            return PartialView("partial/BankList");
        }
        public async Task<ActionResult> editCustomer(Customer customer)
        {
            var data = await unitOfWork.Customer.GetByIdAsync(customer.CustomerId);
            if (data.SecondaryCompany != null)
                data.SecondaryCompanyId = Array.ConvertAll(data.SecondaryCompany.Split(','), int.Parse);

            return Json(new
            {
                Customer = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteCustomer(Customer customer)
        {
            var data = await unitOfWork.Customer.DeleteAsync(customer.CustomerId);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
