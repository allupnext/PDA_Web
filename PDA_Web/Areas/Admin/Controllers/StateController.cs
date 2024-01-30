using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Diagnostics.Metrics;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StateController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public StateController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                var CountryData = await unitOfWork.Countrys.GetAllAsync();
                ViewBag.Country = CountryData;

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }
        public async Task<IActionResult> LoadAll(State state)
        {
            var data = await unitOfWork.States.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END
            if (state.StateName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.StateName.Contains(state.StateName.ToUpper())|| x.StateName.Contains(state.StateName.ToLower())).ToList();
            }
            if (state.CountryId != null && state.CountryId != 0 /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CountryId == state.CountryId).ToList();
            }
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //}
        }
       
        public async Task<ActionResult> stateSave(State state)
        {
            var data = await unitOfWork.States.GetAllAsync();
            if (state.ID > 0)
            {
                var StateName = data.Where(x => x.StateName.ToUpper() == state.StateName.ToUpper() && x.ID != state.ID).ToList();
                if (StateName != null && StateName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("StateName already exists.");
                }
                else
                {
                    await unitOfWork.States.UpdateAsync(state);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var StateName = data.Where(x => x.StateName.ToUpper() == state.StateName.ToUpper()).ToList();
                if (StateName != null && StateName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("StateName already exists.");
                }
                else
                {
                    await unitOfWork.States.AddAsync(state);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
        
        public async Task<ActionResult> editState(State state)
        {
            var data = await unitOfWork.States.GetByIdAsync(state.ID);
            return Json(new
            {
                states = data,
                proceed = true,
                msg = ""
            });
        }
        
        public async Task<ActionResult> deletestate(State state)
        {
            var data = await unitOfWork.States.DeleteAsync(state.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
