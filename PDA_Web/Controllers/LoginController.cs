using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<LoginController> _logger;

        private readonly IToastNotification _toastNotification;
        public LoginController(ILogger<LoginController> logger, IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
            _toastNotification = toastNotification;
            
        }

        [HttpPost]
        public async Task<ActionResult> Index(CustomerAuth customerAuth)
        {
            if (customerAuth != null)
            {
                Customer isAuthenticated = await unitOfWork.Customer.Authenticate(customerAuth.Email, customerAuth.CustomerPassword);
                if (isAuthenticated != null)
                {
                    HttpContext.Session.SetString("CustID", isAuthenticated.CustomerId.ToString());
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.SetString("CustID", "");
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
