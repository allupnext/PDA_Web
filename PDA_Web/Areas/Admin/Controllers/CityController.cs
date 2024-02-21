using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CityController : Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly IUnitOfWork unitOfWork;

        public CityController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                var StateData = await unitOfWork.States.GetAllAsync();
                ViewBag.State = StateData;

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }

        }
        public async Task<IActionResult> LoadAll(CityList cityList)
        {

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END
            var data = await unitOfWork.Citys.GetAllAsync();
            if (cityList.CityName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CityName.ToUpper().Contains(cityList.CityName.ToUpper())).ToList();
            }
            if (cityList.StateId != null && cityList.StateId != 0 /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.StateId == cityList.StateId).ToList();
            }
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //} 
        }

        public async Task<ActionResult> citySave(CityList cityList)
        {
            var data = await unitOfWork.Citys.GetAllAsync();

            if (cityList.ID > 0)
            {
                var CityName = data.Where(x => x.CityName.ToUpper() == cityList.CityName.ToUpper() && x.ID != cityList.ID).ToList();
                if (CityName != null && CityName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("CityName already exists.");
                }
                else
                { 
                    await unitOfWork.Citys.UpdateAsync(cityList);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var CityName = data.Where(x => x.CityName.ToUpper() == cityList.CityName.ToUpper()).ToList();
                if(CityName != null && CityName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("CityName already exists.");
                }
                else
                {
                    var savevalue = await unitOfWork.Citys.AddAsync(cityList);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editCity(CityList cityList)
        {
            var data = await unitOfWork.Citys.GetByIdAsync(cityList.ID);
            return Json(new
            {
                Citys = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteCity(CityList cityList)
        {
            var data = await unitOfWork.Citys.DeleteAsync(cityList.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }


    }
}
