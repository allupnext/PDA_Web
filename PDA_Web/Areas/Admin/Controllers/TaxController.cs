using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDA_Web.Models;
using System.Globalization;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaxController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public TaxController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                var data = await unitOfWork.Taxs.GetAllAsync();
                ViewBag.Taxs = data;
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }
        public async Task<IActionResult> LoadAll(Tax tax)
        {
            var data = await unitOfWork.Taxs.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (tax.TaxName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.TaxName.Contains(tax.TaxName.ToLower())|| x.TaxName.Contains(tax.TaxName.ToUpper())).ToList();
            }
            return PartialView("partial/_ViewAll", data);
        }

        public async Task<ActionResult> TaxSave(Tax tax)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            string ToDate_String = tax.ToDate_String + " " + "12:00:00 AM";
            string FromDate_String = tax.FromDate_String + " " + "12:00:00 AM";

            DateTime Validity_To = DateTime.ParseExact(ToDate_String, new string[] { "dd.M.yyyy hh:mm:ss tt", "dd-M-yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt" }, provider, DateTimeStyles.None);
            DateTime Validity_Froms = DateTime.ParseExact(FromDate_String, new string[] { "dd.M.yyyy hh:mm:ss tt", "dd-M-yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt" }, provider, DateTimeStyles.None);

            tax.FromDate = Validity_Froms;
            tax.ToDate = Validity_To;
            if (tax.ID > 0)
            {
                await unitOfWork.Taxs.UpdateAsync(tax);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.Taxs.AddAsync(tax);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditTax(Tax tax)
        {
            var data = await unitOfWork.Taxs.GetByIdAsync(tax.ID);
            return Json(new
            {
                Taxs = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteTax(Tax tax)
        {
            var data = await unitOfWork.Taxs.DeleteAsync(tax.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
