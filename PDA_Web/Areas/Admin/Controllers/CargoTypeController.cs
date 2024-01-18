using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;

using PDAEstimator_Domain.Entities;


namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CargoTypeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public CargoTypeController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;

        }

        public IActionResult Index()
        {
            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }


        public async Task<IActionResult> LoadAll(CargoType cargoType)
        {
            var data = await unitOfWork.CargoTypes.GetAllAsync();
            if (cargoType.CargoTypeName!= null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CargoTypeName.ToUpper().Contains(cargoType.CargoTypeName.ToUpper())).ToList();
            }
            return PartialView("partial/_ViewAll", data);
            
        }

        public async Task<ActionResult> CargoTypeSave(CargoType cargoType)
        {
            if (cargoType.ID > 0)
            {
                await unitOfWork.CargoTypes.UpdateAsync(cargoType);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.CargoTypes.AddAsync(cargoType);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editCargoType(CargoType cargoType)
        {
            var data = await unitOfWork.CargoTypes.GetByIdAsync(cargoType.ID);
            return Json(new
            {
                CargoTypes = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteCargoType(CargoType cargoType)
        {
            var data = await unitOfWork.CargoTypes.DeleteAsync(cargoType.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}   
