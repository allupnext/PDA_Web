using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DesignationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToastNotification _toastNotification;

        public DesignationController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            _toastNotification = toastNotification;
            _unitOfWork = unitOfWork;   
        }
        public async Task<IActionResult> Index()
        {
            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                // Temp Solution START
                var UserPermissionModel = await _unitOfWork.Roles.GetUserPermissionRights();
                ViewBag.UserPermissionModel = UserPermissionModel;
                var Currentuser = HttpContext.Session.GetString("UserID");

                var UserRole = await _unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
                ViewBag.UserRoleName = UserRole;
                // Temp Solution END
                var StateData = await _unitOfWork.States.GetAllAsync();
                ViewBag.State = StateData;

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }


        public async Task<ActionResult> DesignationSave(Designation DesignationList)
        {
            var data = await _unitOfWork.Designation.GetAllAsync();

            if (DesignationList.DesignationId > 0)
            {
                var DesignationName = data.Where(x => x.DesignationName.ToUpper() == DesignationList.DesignationName.ToUpper() && x.DesignationId != DesignationList.DesignationId).ToList();
                if (DesignationName != null && DesignationName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("DesignationName already exists.");
                }
                else
                {
                    await _unitOfWork.Designation.UpdateAsync(DesignationList);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var DesignationName = data.Where(x => x.DesignationName.ToUpper() == DesignationList.DesignationName.ToUpper()).ToList();
                if (DesignationName != null && DesignationName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("DesignationName already exists.");
                }
                else
                {
                    var savevalue = await _unitOfWork.Designation.AddAsync(DesignationList);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
        public async Task<ActionResult> editDesignation(Designation DesignationList)
        {
            var data = await _unitOfWork.Designation.GetByIdAsync(DesignationList.DesignationId);
            return Json(new
            {
                Citys = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteDesignation(Designation DesignationList)
        {
            var data = await _unitOfWork.Designation.DeleteAsync(DesignationList.DesignationId);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
        public async Task<IActionResult> LoadAll(Designation DesignationList)
        {

            // Temp Solution START
            var UserPermissionModel = await _unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await _unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            var data = await _unitOfWork.Designation.GetAllAsync();
            if (DesignationList.DesignationName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.DesignationName.ToUpper().Contains(DesignationList.DesignationName.ToUpper())).ToList();
            }
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //} 
        }


    }
}
