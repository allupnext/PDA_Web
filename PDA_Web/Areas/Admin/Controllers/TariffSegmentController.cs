using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TariffSegmentController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public TariffSegmentController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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


        public async Task<IActionResult> LoadAll(TariffSegment tariffSegment)
        {
            var data = await unitOfWork.tariffSegment.GetAllAsync();
            if (tariffSegment.TariffSegmentName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.TariffSegmentName.Contains(tariffSegment.TariffSegmentName.ToUpper())|| x.TariffSegmentName.Contains(tariffSegment.TariffSegmentName.ToLower())).ToList();
            }
            return PartialView("partial/_ViewAll", data);

        }

        public async Task<ActionResult> TariffSegmentSave(TariffSegment tariffSegment)
        {
            if (tariffSegment.TariffSegmentID > 0)
            {
                await unitOfWork.tariffSegment.UpdateAsync(tariffSegment);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.tariffSegment.AddAsync(tariffSegment);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editTariffSegment(TariffSegment tariffSegment)
        {
            var data = await unitOfWork.tariffSegment.GetByIdAsync(tariffSegment.TariffSegmentID);
            return Json(new
            {
                tariffSegment = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteTariffSegment(TariffSegment tariffSegment)
        {
            var data = await unitOfWork.tariffSegment.DeleteAsync(tariffSegment.TariffSegmentID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
