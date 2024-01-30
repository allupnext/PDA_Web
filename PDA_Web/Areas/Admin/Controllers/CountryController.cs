using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;

using PDAEstimator_Domain.Entities;
namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CountryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public CountryController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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

        public async Task<IActionResult> LoadAll(Country country)
        {
            var data = await unitOfWork.Countrys.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (country.CountryName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CountryName.ToUpper().Contains(country.CountryName.ToUpper())).ToList();
            }
            if (country.CountryCode != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CountryCode.Contains(country.CountryCode)).ToList();
            }
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //}
        }
        public async Task<ActionResult> countrySave(Country country)
        {
            var data = await unitOfWork.Countrys.GetAllAsync();

            if (country.ID > 0)
            {
                var CountryName = data.Where(x => x.CountryName.ToUpper() == country.CountryName.ToUpper() && x.ID != country.ID).ToList();
                var CountryCode = data.Where(x => x.CountryCode == country.CountryCode && x.ID != country.ID).ToList(); 
                if (CountryName != null && CountryName.Count > 0 || CountryCode != null && CountryCode.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("CountryName or CountryCode already exists.");
                }
                else
                {
                    await unitOfWork.Countrys.UpdateAsync(country);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }

            }

            else
            {
                var CountryName = data.Where(x => x.CountryName.ToUpper() == country.CountryName.ToUpper()).ToList();
                var CountryCode = data.Where(x => x.CountryCode == country.CountryCode && x.ID != country.ID).ToList();
                if (CountryName != null && CountryName.Count > 0 || CountryCode != null && CountryCode.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("CountryName or CountryCode already exists.");
                }
                else
                {
                    await unitOfWork.Countrys.AddAsync(country);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editCountry(Country country)
        {
            var data = await unitOfWork.Countrys.GetByIdAsync(country.ID);
            return Json(new
            {
                Countrys = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deletecountry(Country country)
        {
            var data = await unitOfWork.Countrys.DeleteAsync(country.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

    }
}
