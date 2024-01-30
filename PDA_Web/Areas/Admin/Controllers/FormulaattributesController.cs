using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FormulaattributesController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public FormulaattributesController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }


        public async Task<IActionResult> LoadAll(FormulaAttributes formulaAttributes)
        {
            var data = await unitOfWork.FormatAttribute.GetAllAsync();
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END
            if (formulaAttributes.FormulaName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.FormulaName.ToUpper().Contains(formulaAttributes.FormulaName.ToUpper())).ToList();
            }
            return PartialView("partial/_ViewAll", data);
        }

        public async Task<ActionResult> FormulaattributesSave(FormulaAttributes formulaAttributes)
        {
            if (formulaAttributes.FormulaId > 0)
            {
                await unitOfWork.FormatAttribute.UpdateAsync(formulaAttributes);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.FormatAttribute.AddAsync(formulaAttributes);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editFormulaattributes(FormulaAttributes formulaAttributes)
        {
            var data = await unitOfWork.FormatAttribute.GetByIdAsync(formulaAttributes.FormulaId);
            return Json(new
            {
                FormatAttribute = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteFormulaattributes(FormulaAttributes formulaAttributes)
        {
            var data = await unitOfWork.FormatAttribute.DeleteAsync(formulaAttributes.FormulaId);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
