using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NToastNotify;
using PDAEstimator_Application.Interfaces;

using PDAEstimator_Domain.Entities;
using System.Net.Http.Headers;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CargoDetailsController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        private readonly IUnitOfWork unitOfWork;
        //[HttpPost]
        //public ActionResult Save(PropertyImageModel propertyImageModel)
        //{
        // return View();

        //}



        public CargoDetailsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
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
            return $"{_hostEnvironment.WebRootPath}\\uploads\\{fileName}";
        }

        public IActionResult CargoTypeOnChange(CargoDetails cargoDetails)
        {
            var CargoFamilysData = unitOfWork.CargoFamilys.GetAllAsync().Result.Where(x => x.CargoTypeID == cargoDetails.CargoTypeID);

            ViewBag.CargoFamilys = CargoFamilysData;
            return PartialView("partial/cargoType");
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
                var CargoFamilyData = await unitOfWork.CargoFamilys.GetAllAsync();
                ViewBag.CargoFamilys = CargoFamilyData;
                var data = await unitOfWork.CargoTypes.GetAllAsync();
                ViewBag.CargoType = data;
                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }

        public async Task<IActionResult> LoadAll(CargoDetails cargoDetails)
        {
            var data = await unitOfWork.CargoDetails.GetAlllistAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (cargoDetails.CargoName != null)
            {
                //data = data.Where(x => x.RoleName.Contains("%"+roles.RoleName+"%")).ToList();
                data = data.Where(x => x.CargoName.ToUpper().Contains(cargoDetails.CargoName.ToUpper())).ToList();
            }
            if (cargoDetails.CargoTypeID != null && cargoDetails.CargoTypeID != 0)
            {
                data = data.Where(x => x.CargoTypeID == cargoDetails.CargoTypeID).ToList();
            }
            if (cargoDetails.CargoFamilyID != null && cargoDetails.CargoFamilyID != 0)
            {
                data = data.Where(x => x.CargoFamilyID == cargoDetails.CargoFamilyID).ToList();
            }
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //}
        }
        public async Task<ActionResult> CargoDetailsSave(CargoDetails cargoDetails)
        {
            if (cargoDetails.ID > 0)
            {
                await unitOfWork.CargoDetails.UpdateAsync(cargoDetails);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                await unitOfWork.CargoDetails.AddAsync(cargoDetails);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditCargoDetails(CargoDetails cargoDetails)
        {
            var data = await unitOfWork.CargoDetails.GetByIdAsync(cargoDetails.ID);
            return Json(new
            {
                CargoDetails = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> DeleteCargoDetails(CargoDetails cargoDetails)
        {
            var data = await unitOfWork.CargoDetails.DeleteAsync(cargoDetails.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

    }
}
