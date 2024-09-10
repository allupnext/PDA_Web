using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CargoHandledController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public CargoHandledController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                var data2 = await unitOfWork.PortDetails.GetAllAsync();
                if (data2.Count > 0)
                    data2 = data2.Where(x => x.Status == true).ToList();
                ViewBag.Port = data2;

                var data = await unitOfWork.TerminalDetails.GetAllAsync();
                ViewBag.Terminal = data;

                var data1 = await unitOfWork.BerthDetails.GetAllAsync();
                ViewBag.Berth = data1;

                var data3 = await unitOfWork.CargoDetails.GetAllAsync();
                ViewBag.Cargo = data3;

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
            
        }

/*        public IActionResult PortNameOnchange(PDAEstimator PDAEstimitor)
        {
            var TerminalDetailData = unitOfWork.TerminalDetails.GetAllAsync().Result.Where(x => x.PortID == PDAEstimitor.PortID);

            ViewBag.Terminal = TerminalDetailData;
            return PartialView("partial/TerminalList");
        }
        public IActionResult TerminalNameOnchange(PDAEstimator PDAEstimitor)
        {
            var BearthDetailData = unitOfWork.BerthDetails.GetAllAsync().Result.Where(x => x.TerminalID == PDAEstimitor.TerminalID);
            ViewBag.Berth = BearthDetailData;
            return PartialView("partial/BerthList");
        }*/


        public IActionResult PortNameOnchange(CargoHandleds cargoHandleds)
        {
            var TerminalDetailData = unitOfWork.TerminalDetails.GetAllAsync().Result.Where(x => x.PortID == cargoHandleds.PortID);

            ViewBag.Terminal = TerminalDetailData;
            return PartialView("partial/TerminalList");
        }

        public IActionResult PortNameFilterOnchange(CargoHandleds cargoHandleds)
        {
            var TerminalDetailData = unitOfWork.TerminalDetails.GetAllAsync().Result.Where(x => x.PortID == cargoHandleds.PortID);

            ViewBag.Terminal = TerminalDetailData;
            return PartialView("partial/TerminalFilterList");
        }

        public IActionResult TerminalNameOnchange(CargoHandleds cargoHandleds)
        {
            var BerthDetailData = unitOfWork.BerthDetails.GetAllAsync().Result.Where(x => x.TerminalID == cargoHandleds.TerminalID).ToList();
            ViewBag.Berth = BerthDetailData;
            return PartialView("partial/BerthList");
        }

        public IActionResult TerminalNameFilterOnchange(CargoHandleds cargoHandleds)
        {
            var BerthDetailData = unitOfWork.BerthDetails.GetAllAsync().Result.Where(x => x.TerminalID == cargoHandleds.TerminalID).ToList();
            ViewBag.Berth = BerthDetailData;
            return PartialView("partial/BerthFilterList");
        }

        public async Task<IActionResult> LoadAll(CargoHandleds cargoHandleds)
        {
            var data = await unitOfWork.cargoHandled.GetAlllistAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (cargoHandleds.PortID != null && cargoHandleds.PortID != 0/*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.PortID==cargoHandleds.PortID).ToList();
            }
            if (cargoHandleds.BerthID != null && cargoHandleds.BerthID != 0/*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.BerthID == cargoHandleds.BerthID).ToList();
            }
            if (cargoHandleds.TerminalID != null && cargoHandleds.TerminalID != 0/*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.TerminalID == cargoHandleds.TerminalID).ToList();
            }
            if (cargoHandleds.CargoID != null && cargoHandleds.CargoID != 0/*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CargoID == cargoHandleds.CargoID).ToList();
            }
            return PartialView("partial/_ViewAll", data);

        }
        public async Task<ActionResult> CargoHandledSave(CargoHandleds cargoHandleds)
        {
            if (cargoHandleds.ID > 0)
            {
                await unitOfWork.cargoHandled.UpdateAsync(cargoHandleds);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                foreach (var id in cargoHandleds.CargoIDs)
                {
                    cargoHandleds.CargoID = id;
                    await unitOfWork.cargoHandled.AddAsync(cargoHandleds);
                }
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditCargoHandled(CargoHandleds cargoHandleds)
        {
            var data = await unitOfWork.cargoHandled.GetByIdAsync(cargoHandleds.ID);
            int[] cid = new int[1];
            cid[0] = data.CargoID;
            data.CargoIDs = cid;
            return Json(new
            {
                cargoHandled = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteCargoHandled(CargoHandleds cargoHandleds)
        {
            var data = await unitOfWork.cargoHandled.DeleteAsync(cargoHandleds.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
