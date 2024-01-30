using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using static Azure.Core.HttpHeader;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpenseController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public ExpenseController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
                //var data = await unitOfWork.CargoDetails.GetAllAsync();
                //ViewBag.CargoDetails = data;
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
           
        }

        public async Task<IActionResult> LoadAll(Expense expense)
        {
            var data = await unitOfWork.Expenses.GetAllAsync();
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END
            if (expense.ExpenseName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.ExpenseName.ToUpper().Contains(expense.ExpenseName.ToUpper())).ToList();
            }
            //if (expense.sequnce != null && expense.sequnce != 0/*&& customer.FirstName != 0*/)
            //{
            //    data = data.Where(x => x.sequnce == expense.sequnce).ToList();
            //}
            return PartialView("partial/_ViewAll", data);

        }
        public async Task<ActionResult> ExpenseSave(Expense expenses)
        {
            if (expenses.ID > 0)
            {
                var expensesdata = await unitOfWork.Expenses.GetAllAsync();
                if (expensesdata.Count > 0 && expensesdata.Where(x => x.sequnce == expenses.sequnce && x.ID != expenses.ID).Count() > 0)
                {
                    _toastNotification.AddErrorToastMessage("Sequnce is Already is exist.");
                    return Json(new
                    {
                        proceed = true,
                        msg = ""
                    });
                }
                await unitOfWork.Expenses.UpdateAsync(expenses);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                var expensesdata = await unitOfWork.Expenses.GetAllAsync();
                if (expensesdata.Count > 0 && expensesdata.Where(x => x.sequnce == expenses.sequnce).Count() > 0)
                {
                    _toastNotification.AddErrorToastMessage("Sequnce is Already is exist.");
                    return Json(new
                    {
                        proceed = true,
                        msg = ""
                    });
                }
                await unitOfWork.Expenses.AddAsync(expenses);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditExpense(Expense expenses)
        {
            var data = await unitOfWork.Expenses.GetByIdAsync(expenses.ID);
            return Json(new
            {
                Expenses = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteExpense(Expense expenses)
        {
            var data = await unitOfWork.Expenses.DeleteAsync(expenses.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
