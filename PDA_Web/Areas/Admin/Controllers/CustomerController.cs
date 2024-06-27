﻿using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure_Shared;
using PDAEstimator_Infrastructure_Shared.Services;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        private readonly IEmailSender _emailSender;
        public CustomerController(IUnitOfWork unitOfWork, IToastNotification toastNotification, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index(string Parm2)
        {
            if (Parm2 == "pending")
            {
                ViewBag.CustomerStatus = "2";
            }
            else
            {
                ViewBag.CustomerStatus = "1";
            }
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

                /*           var DesignationData = await unitOfWork.Designation.GetAllAsync();
                         ViewBag.Designations = DesignationData;

                         var StateData = await unitOfWork.States.GetAllAsync();
                         ViewBag.State = StateData;

                         var CityData = await unitOfWork.Citys.GetAllAsync();
                         ViewBag.City = CityData;*/

                var PrimaryCompanyData = await unitOfWork.Company.GetAllAsync();
                ViewBag.PrimaryCompany = PrimaryCompanyData;

                var SecondryCompanyData = await unitOfWork.Company.GetAllAsync();
                ViewBag.SecondaryCompany = SecondryCompanyData;

                var BankData = await unitOfWork.BankMaster.GetAllBankDetailsAsync();
                ViewBag.BankData = BankData;

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }

        public async Task<ActionResult> OpenCustomerUserMaster(CustomerList customer)
        {
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            ViewBag.CustomerSelected = customer.CustomerId;
            ViewBag.Customer = await unitOfWork.Customer.GetAllAsync();

            ViewBag.Designations = await unitOfWork.Designation.GetAllAsync();

            var CountryData = await unitOfWork.Countrys.GetAllAsync();
            ViewBag.Country = CountryData;
            ViewBag.CountryCode = CountryData.Select(x => x.CountryCode).ToList();

            var StateData = await unitOfWork.States.GetAllAsync();
            ViewBag.State = StateData;

            var CityData = await unitOfWork.Citys.GetAllAsync();
            ViewBag.City = CityData;


            var data = await unitOfWork.CustomerUserMaster.GetByCustomerIdAsync(customer.CustomerId);
            var Terminals = PartialView("partial/_CustomerUserDetails", data);
            return Terminals;

        }

        public async Task<ActionResult> CustomerUserSave(CustomerUserMaster customer)
        {
            var customerdata = await unitOfWork.CustomerUserMaster.GetAllAsync();

            var corecustomerdata = await unitOfWork.Customer.GetByIdAsync(customer.CustomerId);

            Int64 PrimaryCompanyId = Convert.ToInt64(corecustomerdata.PrimaryCompany);

            var FromPrimaryCompany = await unitOfWork.Company.GetByIdAsync(PrimaryCompanyId);
            var PrimaryCompnayName = FromPrimaryCompany.CompanyName;

            if (customer.ID > 0)
            {
                var CMobileNumber = customerdata.Where(x => x.Mobile == customer.Mobile && x.ID != customer.ID).ToList();
                var CEmailId = customerdata.Where(x => x.Email.ToUpper() == customer.Email.ToUpper() && x.ID != customer.ID).ToList();
                if (CMobileNumber.Count > 0 && CMobileNumber != null || CEmailId.Count > 0 && CEmailId != null)
                {
                    _toastNotification.AddWarningToastMessage("MobileNumber Or Email Exist!..");
                    return Json(new
                    {
                        proceed = false,
                        msg = ""
                    });
                }
                else
                {
                    await unitOfWork.CustomerUserMaster.UpdateAsync(customer);

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
                    return Json(new
                    {
                        proceed = false,
                        msg = ""
                    });
                }
                else
                {
                    var custId = await unitOfWork.CustomerUserMaster.AddAsync(customer);
                    List<string> recipients = new List<string>
                    {
                       customer.Email
                    };
                    //string Content = "You are added in PDA Estimator and you can access our platform using below login credentials: </br> UserName :" + customer.Email + " User Password:" + customer.Password;
                    string Content = "<html> <head>   <title>PDA Estimator Login Credentials</title> </head> <body>   <p> Dear User,<br>    Thanks for registering on the PDA portal.Your login details are as follows:     <br/>     <b>UserName:</b> " + customer.Email + "     <br/>     <b>User Password:</b> " + customer.Password + " <br><br> <b>Regards <br> PDA Portal</b>  </p> </body> </html> ";
                    string Subject = "Welcome to PDAEstimator";
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

                    _emailSender.SendEmail(Msg);

                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editCustomerUserDetails(CustomerUserMaster customer)
        {
            var data = await unitOfWork.CustomerUserMaster.GetByIdAsync(customer.ID);
            return Json(new
            {
                Customer = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteCustomerUserDetails(CustomerUserMaster customer)
        {
            var data = await unitOfWork.CustomerUserMaster.DeleteAsync(customer.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
        public IActionResult CountryOnchange(CustomerUserMaster customer)
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
        }

        public IActionResult StateOnchange(CustomerUserMaster customer)
        {
            var CityData = unitOfWork.Citys.GetAllAsync().Result.Where(x => x.StateId == customer.State);

            ViewBag.City = CityData;
            return PartialView("partial/CityList");
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

            //if (customer.FirstName != null /*&& customer.FirstName != 0*/)
            //{
            //    customerdata = customerdata.Where(x => x.FirstName.Contains(customer.FirstName)).ToList();
            //}
            //if (customer.Country != null && customer.Country != 0)
            //{
            //    customerdata = customerdata.Where(x => x.Country == customer.Country).ToList();
            //}
            //if (customer.Address1 != null /*&& customer.FirstName != 0*/)
            //{
            //    customerdata = customerdata.Where(x => x.Address1.Contains(customer.Address1)).ToList();
            //}
            //if (customer.Email != null /*&& customer.FirstName != 0*/)
            //{
            //    customerdata = customerdata.Where(x => x.Email.Contains(customer.Email)).ToList();
            //}
            if (customer.Company != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.Company == customer.Company).ToList();
            }
            if (customer.Status != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.Status == customer.Status).ToList();
            }
            return PartialView("partial/_ViewAll", customerdata);

        }

        public async Task<IActionResult> loadcustomeruserData(CustomerList customer)
        {
            // var customerdata = await unitOfWork.Customer.GetAlllistCustomerAsync(customer.CustomerId);
            //var customerdata = await unitOfWork.Customer.GetAlllistAsync();
            var customerdata = await unitOfWork.CustomerUserMaster.GetByCustomerIdAsync(customer.CustomerId);
            //ViewBag.Customer = await unitOfWork.Customer.GetAllAsync();
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (customer.FirstName != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.FirstName.Contains(customer.FirstName)).ToList();
            }
            if (customer.Country != null && customer.Country != 0)
            {
                customerdata = customerdata.Where(x => x.Country == customer.Country).ToList();
            }
            if (customer.Address1 != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.Address1.Contains(customer.Address1)).ToList();
            }
            if (customer.Email != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.Email.Contains(customer.Email)).ToList();
            }
            if (customer.Company != null /*&& customer.FirstName != 0*/)
            {
                customerdata = customerdata.Where(x => x.Company.Contains(customer.Company)).ToList();
            }
            return PartialView("partial/_ViewAllCustomerUserDetails", customerdata);

        }

        public async Task<ActionResult> CustomerSave(Customer customer)
        {
            var customerdata = await unitOfWork.Customer.GetAlllistAsync();
            if (customer.CustomerId > 0)
            {
                var userid = HttpContext.Session.GetString("UserID");
                customer.ModifyDate = DateTime.UtcNow;
                customer.ModifyBy = Convert.ToInt32(userid);
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
            else
            {
                var userid = HttpContext.Session.GetString("UserID");
                customer.CreatedDate = DateTime.UtcNow;
                customer.CreatedBy = Convert.ToInt32(userid);
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
            return Json(new
            {
                proceed = true,
                msg = ""
            });
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
