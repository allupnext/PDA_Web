using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VesselSizeTypeMasterController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public VesselSizeTypeMasterController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                return RedirectToAction("index", "login");
            }
        }
        public async Task<IActionResult> LoadAll(VesselSizeTypeMaster model)
        {
            var data = await unitOfWork.VesselSizeTypeMaster.GetAllAsync();

            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;

            if (!string.IsNullOrEmpty(model.VesselSizeTypeName))
            {
                data = data
                    .Where(x => x.VesselSizeTypeName
                    .ToUpper()
                    .Contains(model.VesselSizeTypeName.ToUpper()))
                    .ToList();
            }

            return PartialView("partial/_ViewAll", data);
        }

        [HttpPost]
        public async Task<IActionResult> VesselSizeTypeMasterSave(
            VesselSizeTypeMaster vessel)
        {
            if (vessel.VesselSizeTypeId > 0)
            {
                
                await unitOfWork.VesselSizeTypeMaster.UpdateAsync(vessel);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
               
                await unitOfWork.VesselSizeTypeMaster.AddAsync(vessel);
                _toastNotification.AddSuccessToastMessage("Inserted Successfully");
            }

            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<IActionResult> VesselSizeTypeMasterEdit(
            VesselSizeTypeMaster vessel)
        {
            var data = await unitOfWork
                .VesselSizeTypeMaster
                .GetByIdAsync(vessel.VesselSizeTypeId);

            return Json(new
            {
                states = data,
                proceed = true,
                msg = ""
            });
        }


        public async Task<IActionResult> GetById(long id)
        {
            var vessel = await unitOfWork.VesselSizeTypeMaster.GetByIdAsync(id);

            return Json(new
            {
                dwt = vessel.DWT,
                grt = vessel.GRT,
                nrt = vessel.NRT,
                loa = vessel.LOA,
                beam = vessel.Beam
            });
        }


        public async Task<IActionResult> VesselSizeTypeMasterDelete(
            VesselSizeTypeMaster vessel)
        {
            await unitOfWork
                .VesselSizeTypeMaster
                .DeleteAsync(vessel.VesselSizeTypeId);

            _toastNotification.AddSuccessToastMessage("Deleted Successfully");

            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
