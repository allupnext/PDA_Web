﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using NToastNotify;
using PDAEstimator_Application.Interfaces;

using PDAEstimator_Domain.Entities;
namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PortActivityTypeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public PortActivityTypeController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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

        public async Task<IActionResult> LoadAll(PortActivityType portActivityType)
        {
            var data = await unitOfWork.PortActivities.GetAllAsync();
            if (portActivityType.ActivityType != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.ActivityType.ToUpper().Contains(portActivityType.ActivityType.ToUpper())).ToList();
            }
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //}
        }
        public async Task<ActionResult> PortActivityTypeSave(PortActivityType portactivitytype)
        {
            var data = await unitOfWork.PortActivities.GetAllAsync();
            if (portactivitytype.ID > 0)
            {
                var PortOperationName = data.Where(x => x.ActivityType.ToUpper() == portactivitytype.ActivityType.ToUpper() && x.ID != portactivitytype.ID).ToList();
                if (PortOperationName != null && PortOperationName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Port Operation Type already exist.");
                }
                else
                {
                    await unitOfWork.PortActivities.UpdateAsync(portactivitytype);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var PortOperationName = data.Where(x => x.ActivityType.ToUpper() == portactivitytype.ActivityType.ToUpper()).ToList();
                if (PortOperationName != null && PortOperationName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Port Operation Type already exist.");
                }
                else
                {
                    await unitOfWork.PortActivities.AddAsync(portactivitytype);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editPortActivityType(PortActivityType portactivitytype)
        {
            var data = await unitOfWork.PortActivities.GetByIdAsync(portactivitytype.ID);
            return Json(new
            {
                Countrys = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deletePortActivityType(PortActivityType portactivitytype)
        {
            var data = await unitOfWork.PortActivities.DeleteAsync(portactivitytype.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

    }
}
