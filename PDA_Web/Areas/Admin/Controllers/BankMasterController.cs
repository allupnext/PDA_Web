using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;

using PDAEstimator_Domain.Entities;
using System.Data;
using System.Diagnostics.Metrics;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BankMasterController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public BankMasterController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                var data = await unitOfWork.Company.GetAlllistAsync();
                ViewBag.Company = data;
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }

        public async Task<IActionResult> LoadAll(BankMaster bankMaster)
        {
            var data = await unitOfWork.BankMaster.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (bankMaster.CompanyId != null && bankMaster.CompanyId != 0 /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CompanyId == bankMaster.CompanyId).ToList();
            }
            if (bankMaster.NameofBeneficiary != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.NameofBeneficiary.ToUpper().Contains(bankMaster.NameofBeneficiary.ToUpper())).ToList();
            }
            if (bankMaster.Beneficiary_Bank_Name != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.Beneficiary_Bank_Name.ToUpper().Contains(bankMaster.Beneficiary_Bank_Name.ToUpper())).ToList();
            }
            //if (customer.City != null && customer.City != 0 /*&& customer.FirstName != 0*/)
            //{
            //    data = data.Where(x => x.City == customer.City).ToList();
            //}
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //}
        }
        public async Task<ActionResult> BankMasterSave(BankMaster bankMaster)
        {
            var data = await unitOfWork.BankMaster.GetAllAsync();
            if (bankMaster.BankId > 0)
            {
                var AccountNo = data.Where(x => x.AccountNo == bankMaster.AccountNo && x.BankId != bankMaster.BankId).ToList();
                //var Beneficiary_Bank_Swift_Code = data.Where(x => x.Beneficiary_Bank_Swift_Code != bankMaster.Beneficiary_Bank_Swift_Code && x.BankId == bankMaster.BankId).ToList();
                //var Beneficiary_RTGS_NEFT_IFSC_Code = data.Where(x => x.Beneficiary_RTGS_NEFT_IFSC_Code != bankMaster.Beneficiary_RTGS_NEFT_IFSC_Code && x.BankId == bankMaster.BankId).ToList();
                //var Intermediary_Bank_Swift_Code = data.Where(x => x.Intermediary_Bank_Swift_Code != bankMaster.Intermediary_Bank_Swift_Code && x.BankId == bankMaster.BankId).ToList();
                
                if (AccountNo != null && AccountNo.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("AccountNo already exists.");
                }

/*                else if (Beneficiary_Bank_Swift_Code != null && Beneficiary_Bank_Swift_Code.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Beneficiary_Bank_Swift_Code already exists.");
                }
                else if (Beneficiary_RTGS_NEFT_IFSC_Code != null && Beneficiary_RTGS_NEFT_IFSC_Code.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Beneficiary_RTGS_NEFT_IFSC_Code already exists.");
                }
                else if (Intermediary_Bank_Swift_Code != null && Intermediary_Bank_Swift_Code.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Intermediary_Bank_Swift_Code already exists.");
                }*/
                else
                {
                    await unitOfWork.BankMaster.UpdateAsync(bankMaster);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                //var Beneficiary_RTGS_NEFT_IFSC_Code = data.Where(x => x.Beneficiary_RTGS_NEFT_IFSC_Code == bankMaster.Beneficiary_RTGS_NEFT_IFSC_Code).ToList();
                //var Intermediary_Bank_Swift_Code = data.Where(x => x.Intermediary_Bank_Swift_Code == bankMaster.Intermediary_Bank_Swift_Code).ToList();
                var AccountNo = data.Where(x => x.AccountNo == bankMaster.AccountNo).ToList();
               //var Beneficiary_Bank_Swift_Code = data.Where(x => x.Beneficiary_Bank_Swift_Code == bankMaster.Beneficiary_Bank_Swift_Code).ToList();
                
                if (AccountNo != null && AccountNo.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("AccountNo already exists.");
                }
/*                else if (Beneficiary_Bank_Swift_Code != null && Beneficiary_Bank_Swift_Code.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Beneficiary_Bank_Swift_Code already exists.");
                }
                else if (Beneficiary_RTGS_NEFT_IFSC_Code != null && Beneficiary_RTGS_NEFT_IFSC_Code.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Beneficiary_RTGS_NEFT_IFSC_Code already exists.");
                }
                else if (Intermediary_Bank_Swift_Code != null && Intermediary_Bank_Swift_Code.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Intermediary_Bank_Swift_Code already exists.");
                }*/
                else
                {
                    await unitOfWork.BankMaster.AddAsync(bankMaster);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> BankMasteredit(BankMaster bankMaster)
        {
            var data = await unitOfWork.BankMaster.GetByIdAsync(bankMaster.BankId);
            return Json(new
            {
                states = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> BankMasterdelete(BankMaster bankMaster)
        {
            var data = await unitOfWork.BankMaster.DeleteAsync(bankMaster.BankId);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

    }
}
