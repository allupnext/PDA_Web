using Dapper;
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

        #region User Methods
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

                var PrimaryCompanyData = await unitOfWork.Company.GetAllAsync();
                ViewBag.PrimaryCompany = PrimaryCompanyData;

                var SecondryCompanyData = await unitOfWork.Company.GetAllAsync();
                ViewBag.SecondaryCompany = SecondryCompanyData;

                var PortList = await unitOfWork.PortDetails.GetAllAsync();
                if (PortList.Count > 0)
                    PortList = PortList.Where(x => x.Status == true).ToList();
                ViewBag.PortList = PortList;

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

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END
            if (user.SearchedName != null && user.SearchedName != "")
            {
                data = data.Where(u => (u.Salutation.ToLower() + " " + u.FirstName.ToLower() + " " + u.LastName.ToLower()).Contains(user.SearchedName.ToLower())).ToList();
            }

            if (user.EmployCode != null && user.EmployCode != "")
            {
                data = data.Where(u => u.EmployCode.ToLower().Contains(user.EmployCode.ToLower())).ToList();
            }

            if (user.RoleId != null && user.RoleId != 0)
            {
                //data = data.Where(x => x.RoleName.Contains("%"+roles.RoleName+"%")).ToList();
                data = data.Where(x => x.RoleId == user.RoleId).ToList();
            }
            if (user.PrimaryCompanyId != null && user.PrimaryCompanyId != 0)
            {
                data = data.Where(x => x.PrimaryCompany == user.PrimaryCompany).ToList();
            }
            return PartialView("partial/_ViewAll", data);
        }

        public async Task<ActionResult> UserSave(User user)
        {
            var data = await unitOfWork.User.GetAlllistAsync();
            user.PortIds = user.SelectedPortIds.Split(',').Select(x => int.Parse(x)).ToArray();
            if (user.ID > 0)
            {
                var EmployeeCodeupdate = data.Where(x => x.EmployCode.ToUpper() == user.EmployCode.ToUpper() && x.ID != user.ID).ToList();
                var EmployeeNumberupdate = data.Where(x => x.EmployCode.ToUpper() == user.EmployCode.ToUpper() && x.ID != user.ID).ToList();
                if (EmployeeCodeupdate != null && EmployeeCodeupdate.Count > 0 || EmployeeNumberupdate != null && EmployeeNumberupdate.Count > 0)
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
            var data = unitOfWork.User.GetAllUsersById(user.ID).Result;
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

        #endregion User Methods

        #region User Role
        public async Task<IActionResult> UserRolePermission()
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
                //Roles RolesM = new Roles();
                //RolesM = await unitOfWork.Roles.GetAllAsync().Result.Where(x => x.RoleID == RolesM.RoleID).ToList();
                var RoleData = await unitOfWork.Roles.GetAllAsync();
                //Roles RoleId = 

                ViewBag.Roles = RoleData.Where(x=>x.RoleName != "Admin");
                //ViewBag.Roles = RolesM.RoleID.ToString();
                
                
                var UserRolePermissionMenuData = await unitOfWork.User.GetAllUserRolePermissionMenuAsync();
                ViewBag.UserRolePermissionMenu = UserRolePermissionMenuData;

                var UserRolePermissionsData = await unitOfWork.User.GetAllUserRolePermissionsAsync();
                ViewBag.UserRolePermissions = UserRolePermissionsData;

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }

        public async Task<IActionResult> RolePermission_click(UserPemissionRole_Role_Mapping userPemissionRole_Role_Mapping)
        {
            if (userPemissionRole_Role_Mapping.IsPermission)
                unitOfWork.User.AddUserPemissionRole_Role_MappingAsync(userPemissionRole_Role_Mapping);
            else
                unitOfWork.User.DeleteUserPemissionRole_Role_MappingAsync(userPemissionRole_Role_Mapping.RoleID, userPemissionRole_Role_Mapping.UserRolePermissionId);

            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<IActionResult> PermissionLoadAll(int RoleId)
        {
            //var UserRolePermissionMenuData = await unitOfWork.User.GetAllUserRolePermissionMenuAsync();
            //ViewBag.UserRolePermissionMenu = UserRolePermissionMenuData;

            //var UserRolePermissionsData = await unitOfWork.User.GetAllUserRolePermissionsAsync();
            //ViewBag.UserRolePermissions = UserRolePermissionsData;

            //ViewBag.RollId = RoleId;

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            var UserPemissionRole_Role_MappingData = await unitOfWork.User.GetAllUserPemissionRole_Role_Mapping();
            UserPemissionRole_Role_MappingData =  UserPemissionRole_Role_MappingData.Where(x => x.RoleID == RoleId).ToList();
            string UserRolePermissionIds = "";
            if (UserPemissionRole_Role_MappingData.Count > 0)
            {
                var UserRolePermissionIdList = UserPemissionRole_Role_MappingData.Select(x => x.UserRolePermissionId).ToList();
                UserRolePermissionIds = string.Join(",", UserRolePermissionIdList);
            }
            return Json(new
            {
                UserRolePermissionIds = UserRolePermissionIds,
                proceed = true,
                msg = ""
            });
        }
        #endregion
    }
}
