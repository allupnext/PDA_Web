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
        public async Task<ActionResult> Index(UserAuth user)
        {
            if (user != null)
            {
                User isAuthenticated = await unitOfWork.User.Authenticate(user.EmployCode, user.UserPassword);
                if (isAuthenticated != null)
                {
                    HttpContext.Session.SetString("UserID", isAuthenticated.ID.ToString());
                    return RedirectToAction("Index", "Home");
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
            HttpContext.Session.SetString("UserID", "");
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
