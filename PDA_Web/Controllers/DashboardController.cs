﻿using Microsoft.AspNetCore.Mvc;
using PDA_Web.Models;
using System.Diagnostics;

namespace PDA_Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userid = HttpContext.Session.GetString("CustID");
            if (!string.IsNullOrEmpty(userid))
            {
                return View();
            }
            else
            {
                return RedirectToAction("index", "Login");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}