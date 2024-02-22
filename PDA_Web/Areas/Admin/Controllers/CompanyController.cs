using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Net.Http.Headers;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;
            _hostEnvironment = hostEnvironment;
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
                var CountryData = await unitOfWork.Countrys.GetAllAsync();
                ViewBag.Country = CountryData;
                ViewBag.CountryCode = CountryData.Select(x => x.CountryCode).ToList();

                var StateData = await unitOfWork.States.GetAllAsync();
                ViewBag.State = StateData;

                var CityData = await unitOfWork.Citys.GetAllAsync();
                ViewBag.City = CityData;
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }

        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            //string path = Path.Combine(this.Environment.WebRootPath, "Files/") + fileName;
            string fullPath = GetFullPathOfFile(fileName.Replace("\"", ""));
            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(fullPath);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
        public async Task<IActionResult> LoadAll(CompanyMaster companyMaster,Customer customer)
        {
            var data = await unitOfWork.Company.GetAlllistAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (companyMaster.CompanyName != null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.CompanyName.ToUpper().Contains(companyMaster.CompanyName.ToUpper())).ToList();
            }
            if (customer.Country != null && customer.Country !=0 /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.Country == customer.Country).ToList();
            }
            if (customer.State != null && customer.State != 0 /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.State == customer.State).ToList();
            }
            if (customer.City != null && customer.City != 0 /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.City == customer.City).ToList();
            }
            return PartialView("partial/_ViewAll", data);
        }
        public IActionResult CountryOnchange(Customer customer)
        {
            var StateData = unitOfWork.States.GetAllAsync().Result.Where(x => x.CountryId == customer.Country);
            var CityData = unitOfWork.Citys.GetCitylistByCountry(customer.Country, 0);

            ViewBag.City = CityData;
            ViewBag.State = StateData;
            return PartialView("partial/StatesList");
        }

        public IActionResult CountryOnchangeforCity(Customer customer)
        {
            var CityData = unitOfWork.Citys.GetCitylistByCountry(customer.Country, 0).Result;

            ViewBag.City = CityData;
            return PartialView("partial/CityList");
        }
        public async Task<IActionResult> CountryOnChangeforCountryCode(Customer customer)
        {
            var CountryData = await unitOfWork.Countrys.GetCountryCodeByCountryIdAsync(customer.Country);
            ViewBag.CountryCode = CountryData;
            return Json(new
            {
                code = CountryData.CountryCode,
                proceed = true,
                msg = ""
            });
        }

        public IActionResult StateOnchange(Customer customer)
        {
            var CityData = unitOfWork.Citys.GetAllAsync().Result.Where(x => x.StateId == customer.State);

            ViewBag.City = CityData;
            return PartialView("partial/CityList");
        }
        public async Task<ActionResult> CompanyMasterSave(CompanyMaster companyMaster)
        {
            var data = await unitOfWork.Company.GetAlllistAsync();
            if (companyMaster.CompanyId > 0)
            {
                var CompanyName = data.Where(x => x.CompanyName.ToUpper() == companyMaster.CompanyName.ToUpper() && x.CompanyId != companyMaster.CompanyId).ToList();
                var TelePhone = data.Where(x => x.Telephone.Substring(4) == companyMaster.Telephone && x.CompanyId != companyMaster.CompanyId).ToList();
                var Email = data.Where(x => x.Email.ToUpper() == companyMaster.Email.ToUpper() && x.CompanyId != companyMaster.CompanyId).ToList();
                if (CompanyName != null && CompanyName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("CompanyName already exist.");
                }
                else if (TelePhone != null && TelePhone.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Telephone already exist.");
                }
                else if (Email != null && Email.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Email already exist.");
                }
                else
                {
                    await unitOfWork.Company.UpdateAsync(companyMaster);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }

            }
            else
            {
                var numberWithCOuntryCOde = companyMaster.CountryCode + "-" + companyMaster.Telephone;
                var CompanyName = data.Where(x => x.CompanyName.ToUpper() == companyMaster.CompanyName.ToUpper()).ToList();
                var TelePhone = data.Where(x => x.Telephone == numberWithCOuntryCOde).ToList();
                var Email = data.Where(x => x.Email.ToUpper() == companyMaster.Email.ToUpper()).ToList();
                if (CompanyName != null && CompanyName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("CompanyName already exist.");
                }
                else if (TelePhone != null && TelePhone.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Telephone already exist.");
                }
                else if (Email != null && Email.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Email already exist.");
                }
                else
                {
                    await unitOfWork.Company.AddAsync(companyMaster);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }

            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditCompanyMaster(CompanyMaster companyMaster)
        {
            var data = await unitOfWork.Company.GetByIdAsync(companyMaster.CompanyId);
            return Json(new
            {
                Company = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteCompanyMaster(CompanyMaster companyMaster)
        {
            var data = await unitOfWork.Company.DeleteAsync(companyMaster.CompanyId);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");

            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        [HttpPost]
        public async Task<ActionResult> UploadFiles(IList<IFormFile> files)
        {
            try
            {
                string fileName = null;

                foreach (IFormFile source in files)
                {
                    // Get original file name to get the extension from it.
                    string orgFileName = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName;

                    // Create a new file name to avoid existing files on the server with the same names.
                    fileName = DateTime.Now.ToFileTime() + Path.GetExtension(orgFileName);

                    string fullPath = GetFullPathOfFile(fileName.Replace("\"", ""));

                    // Create the directory.
                    Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);

                    // Save the file to the server.
                    await using FileStream output = System.IO.File.Create(fullPath);
                    await source.CopyToAsync(output);
                }

                var response = new { FileName = fileName.Replace("\"", "") };

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetFullPathOfFile(string fileName)
        {
            return $"{_hostEnvironment.WebRootPath}\\companylogo\\{fileName}";
        }

    }
}
