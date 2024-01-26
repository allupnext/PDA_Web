﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;
using System.Data;
using System.Data.SqlClient;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IConfiguration configuration;

        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public UserController(IUnitOfWork unitOfWork, IToastNotification toastNotification, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;
            this.configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            var PrimaryCompanyData = await unitOfWork.Company.GetAllAsync();
            ViewBag.PrimaryCompany = PrimaryCompanyData;

            var SecondryCompanyData = await unitOfWork.Company.GetAllAsync();
            ViewBag.SecondaryCompany = SecondryCompanyData;

            var PortList = await unitOfWork.PortDetails.GetAllAsync();
            ViewBag.PortList = PortList;

            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                var RoleData = await unitOfWork.Roles.GetAllAsync();
                ViewBag.Roles = RoleData;
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }

        public async Task<IActionResult> LoadAll(User user)
        {
            //string fullname = user.Salutation + " " + user.FirstName + " " + user.LastName;
            var data = await unitOfWork.User.GetAlllistAsync();
            if (user.RoleId != null && user.RoleId != 0 )
            {
                //data = data.Where(x => x.RoleName.Contains("%"+roles.RoleName+"%")).ToList();
                data = data.Where(x => x.RoleId == user.RoleId).ToList();
            }
            if (user.PrimaryCompanyId != null && user.PrimaryCompanyId != 0 )
            {
                data = data.Where(x => x.PrimaryCompany == user.PrimaryCompany).ToList();
            }
            return PartialView("partial/_ViewAll", data);
        }

        public async Task<ActionResult> UserSave(User user)
        {
            var data = await unitOfWork.User.GetAlllistAsync();

            if (user.ID > 0)
            {
                var EmployeeCodeupdate = data.Where(x => x.EmployCode.ToUpper() == user.EmployCode.ToUpper() && x.ID != user.ID).ToList();
                var EmployeeNumberupdate = data.Where(x => x.EmployCode.ToUpper() == user.EmployCode.ToUpper() && x.ID != user.ID).ToList();
                if (EmployeeCodeupdate != null && EmployeeCodeupdate.Count > 0 || EmployeeNumberupdate != null && EmployeeNumberupdate.Count> 0)
                {
                    _toastNotification.AddWarningToastMessage("EmployeeCode Or MobileNumber Exist!..");
                }
                else
                {
                    await unitOfWork.User.UpdateAsync(user);
                    await unitOfWork.User.DeleteCustomer_User_MappingAsync(user.ID);
                    await unitOfWork.User.DeletePort_User_MappingAsync(user.ID);

                    if (user.PrimaryCompanyId != null)
                    {
                        Company_User_Mapping company_User_Mapping = new Company_User_Mapping();

                        company_User_Mapping.UserID = Convert.ToInt32(user.ID);
                        company_User_Mapping.CompanyID = (int)user.PrimaryCompanyId;
                        company_User_Mapping.IsPrimary = true;
                        await unitOfWork.User.AddCustomer_User_MappingAsync(company_User_Mapping);
                    }
                    if (user.SecondaryCompanyId != null)
                    {
                        Company_User_Mapping company_User_Mapping1 = new Company_User_Mapping();

                        foreach (int i in user.SecondaryCompanyId)
                        {
                            company_User_Mapping1 = new Company_User_Mapping();
                            company_User_Mapping1.UserID = Convert.ToInt32(user.ID);
                            company_User_Mapping1.CompanyID = i;
                            company_User_Mapping1.IsPrimary = false;
                            await unitOfWork.User.AddCustomer_User_MappingAsync(company_User_Mapping1);
                        }
                    }

                    if (user.PortIds != null)
                    {
                        User_Port_Mapping user_Port_Mapping = new User_Port_Mapping();

                        foreach (int i in user.PortIds)
                        {
                            user_Port_Mapping = new User_Port_Mapping();
                            user_Port_Mapping.UserID = Convert.ToInt32(user.ID);
                            user_Port_Mapping.PortID = i;

                            await unitOfWork.User.AddPort_User_MappingAsync(user_Port_Mapping);
                        }
                    }
                    _toastNotification.AddSuccessToastMessage("Updated Successfully..");
                }
            }
            else
            {
                var EmployeeCode = data.Where(x => x.EmployCode.ToUpper() == user.EmployCode.ToUpper()).ToList();
                var EmployeeContactNumber = data.Where(x => x.MobileNo == user.MobileNo).ToList();
                if (EmployeeCode.Count > 0 && EmployeeCode != null || EmployeeContactNumber.Count > 0 && EmployeeContactNumber != null)
                {
                    _toastNotification.AddWarningToastMessage("EmployeeCode Or MobileNumber Exist!..");
                }
                else
                {
                    var userId = await unitOfWork.User.AddAsync(user);

                    if (!string.IsNullOrEmpty(userId))
                    {
                        if (user.PrimaryCompanyId != null)
                        {
                            Company_User_Mapping company_User_Mapping = new Company_User_Mapping();

                            company_User_Mapping.UserID = Convert.ToInt32(userId);
                            company_User_Mapping.CompanyID = (int)user.PrimaryCompanyId;
                            company_User_Mapping.IsPrimary = true;
                            await unitOfWork.User.AddCustomer_User_MappingAsync(company_User_Mapping);
                        }
                        if (user.SecondaryCompanyId != null)
                        {
                            Company_User_Mapping company_User_Mapping1 = new Company_User_Mapping();

                            foreach (int i in user.SecondaryCompanyId)
                            {
                                company_User_Mapping1 = new Company_User_Mapping();
                                company_User_Mapping1.UserID = Convert.ToInt32(userId);
                                company_User_Mapping1.CompanyID = i;
                                company_User_Mapping1.IsPrimary = false;
                                await unitOfWork.User.AddCustomer_User_MappingAsync(company_User_Mapping1);
                            }
                        }
                        if (user.PortIds != null)
                        {
                            User_Port_Mapping user_Port_Mapping = new User_Port_Mapping();

                            foreach (int i in user.PortIds)
                            {
                                user_Port_Mapping = new User_Port_Mapping();
                                user_Port_Mapping.UserID = Convert.ToInt32(userId);
                                user_Port_Mapping.PortID = i;
                               
                                await unitOfWork.User.AddPort_User_MappingAsync(user_Port_Mapping);
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

        public async Task<ActionResult> EditUser(User user)
        {
            var data = await unitOfWork.User.GetByIdAsync(user.ID);
            return Json(new
            {
                User = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditFullUser(User user)
        {
            var data =  unitOfWork.User.GetAllUsersById(user.ID).Result;
            if (data.SecondaryCompany != null)
                data.SecondaryCompanyId = Array.ConvertAll(data.SecondaryCompany.Split(','), int.Parse);

            if (data.Ports != null)
                data.PortIds = Array.ConvertAll(data.Ports.Split(','), int.Parse);
            return Json(new
            {
                User = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteUser(User user)
        {
            var data = await unitOfWork.User.DeleteAsync(user.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
