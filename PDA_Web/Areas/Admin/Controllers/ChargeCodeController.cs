using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Linq;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChargeCodeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public ChargeCodeController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;

        }

        public async Task<IActionResult> Index()
        {
            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                var data = await unitOfWork.Expenses.GetAllAsync();
                ViewBag.Expenses = data;
                // Temp Solution START
                var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
                ViewBag.UserPermissionModel = UserPermissionModel;
                var Currentuser = HttpContext.Session.GetString("UserID");

                var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
                ViewBag.UserRoleName = UserRole;
                // Temp Solution END
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }

        }

        public async Task<IActionResult> LoadAll(ChargeCode chargeCode)
        {
            var data = await unitOfWork.ChargeCodes.GetAlllistAsync();
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (chargeCode.ChargeCodeName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.ChargeCodeName.ToUpper().Contains(chargeCode.ChargeCodeName.ToUpper())).ToList();
            }

            if (chargeCode.ExpenseCategoryID != null && chargeCode.ExpenseCategoryID != 0/*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.ExpenseCategoryID == chargeCode.ExpenseCategoryID).ToList();
            }
            return PartialView("partial/_ViewAll", data);

        }
        public async Task<ActionResult> ChargeCodeSave(ChargeCode chargeCodes)
        {
            if (chargeCodes.ID > 0)
            {
                await unitOfWork.ChargeCodes.UpdateAsync(chargeCodes);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.ChargeCodes.AddAsync(chargeCodes);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditChargeCode(ChargeCode chargeCodes)
        {
            var data = await unitOfWork.ChargeCodes.GetByIdAsync(chargeCodes.ID);
            return Json(new
            {
                ChargeCodes = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteChargeCode(ChargeCode chargeCodes)
        {
            var data = await unitOfWork.ChargeCodes.DeleteAsync(chargeCodes.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
