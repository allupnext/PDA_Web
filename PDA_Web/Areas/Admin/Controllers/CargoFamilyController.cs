using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;

using PDAEstimator_Domain.Entities;


namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CargoFamilyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public CargoFamilyController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                var data = await unitOfWork.CargoTypes.GetAllAsync();
                ViewBag.CargoType = data;
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }

        }

        public async Task<IActionResult> LoadAll(CargoFamily cargoFamily)
        {
            var data = await unitOfWork.CargoFamilys.GetAlllistAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (cargoFamily.CargoFamilyName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CargoFamilyName.ToUpper().Contains(cargoFamily.CargoFamilyName.ToUpper())).ToList();
            }
            if (cargoFamily.CargoTypeID != null && cargoFamily.CargoTypeID != 0/*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CargoTypeID == cargoFamily.CargoTypeID).ToList();
            }
            return PartialView("partial/_ViewAll", data);

        }
        public async Task<ActionResult> CargoFamilySave(CargoFamily cargoFamily)
        {
            if (cargoFamily.ID > 0)
            {
                await unitOfWork.CargoFamilys.UpdateAsync(cargoFamily);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.CargoFamilys.AddAsync(cargoFamily);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditCargoFamily(CargoFamily cargoFamily)
        {
            var data = await unitOfWork.CargoFamilys.GetByIdAsync(cargoFamily.ID);
            return Json(new
            {
                CargoFamilys = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteCargoFamily(CargoFamily cargoFamily)
        {
            var data = await unitOfWork.CargoFamilys.DeleteAsync(cargoFamily.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

    }
}
