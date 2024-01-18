using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class BerthController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public BerthController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
        public async Task<IActionResult> LoadAll()
        {
            var data = await unitOfWork.BerthDetails.GetAllAsync();
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //}
        }

        public async Task<ActionResult> berthSave(BerthDetails berthDetails)
        {
            if (berthDetails.ID > 0)
            {
                await unitOfWork.BerthDetails.UpdateAsync(berthDetails);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.BerthDetails.AddAsync(berthDetails);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");

            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editBerth(BerthDetails berthDetails)
        {
            var data = await unitOfWork.BerthDetails.GetByIdAsync(berthDetails.ID);
            return Json(new
            {
                berths = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteBerth(BerthDetails berthDetails)
        {
            var data = await unitOfWork.BerthDetails.DeleteAsync(berthDetails.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
