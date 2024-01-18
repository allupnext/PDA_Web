using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CurrencyController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public CurrencyController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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

        public async Task<IActionResult> LoadAll(Currency currency)
        {
            var data = await unitOfWork.Currencys.GetAllAsync();
            if (currency.CurrencyName != null && currency.CurrencyName != "" /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CurrencyName.ToUpper().Contains(currency.CurrencyName.ToUpper())).ToList();
            }
            if (currency.CurrencyCode != null && currency.CurrencyCode != "" /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CurrencyCode.ToUpper().Contains(currency.CurrencyCode.ToUpper())).ToList();
            }
            return PartialView("partial/_ViewAll", data);

        }

        public async Task<ActionResult> CurrencySave(Currency currency)
        {
            if (currency.ID > 0)
            {
                await unitOfWork.Currencys.UpdateAsync(currency);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                    await unitOfWork.Currencys.AddAsync(currency);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editCurrency(Currency currency)
        {
            var data = await unitOfWork.Currencys.GetByIdAsync(currency.ID);
            return Json(new
            {
                Currencys = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteCurrency(Currency Currency)
        {
            var data = await unitOfWork.Currencys.DeleteAsync(Currency.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
