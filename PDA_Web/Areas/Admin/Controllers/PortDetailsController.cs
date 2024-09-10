
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PortDetailsController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public PortDetailsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFiles(IList<IFormFile> files, IList<string> filenames)
        {
            try
            {
                string fileName = null;
                int count = 0;
                foreach (IFormFile source in files)
                {
                    // Get original file name to get the extension from it.
                    string orgFileName = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName;

                    // Create a new file name to avoid existing files on the server with the same names.
                    fileName = filenames[count].ToString();
                    fileName = fileName + "." + orgFileName.Split(".")[1];
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

        //[HttpPost]
        //public async Task<ActionResult> UploadFilesTanker(IList<IFormFile> files, IList<string> filenames)
        //{
        //    try
        //    {
        //        string fileName = null;
        //        int count = 0;
        //        foreach (IFormFile source in files)
        //        {
        //            // Get original file name to get the extension from it.
        //            string orgFileName = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName;
        //            fileName = fileName + "." + orgFileName.Split(".")[1];
        //            // Create a new file name to avoid existing files on the server with the same names.
        //            fileName = filenames[count].ToString();

        //            string fullPath = GetFullPathOfFile(fileName.Replace("\"", ""));



        //            // Create the directory.
        //            Directory.CreateDirectory(Directory.GetParent(fullPath).FullName);

        //            // Save the file to the server.
        //            await using FileStream output = System.IO.File.Create(fullPath);
        //            await source.CopyToAsync(output);

                    



        //        }

        //        var response = new { FileName = fileName.Replace("\"", "") };

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private string GetFullPathOfFile(string fileName)
        {
            return $"{_hostEnvironment.WebRootPath}\\uploads\\{fileName}";
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


                var StateData = await unitOfWork.States.GetAllAsync();
                ViewBag.State = StateData;

                var CityData = await unitOfWork.Citys.GetAllAsync();
                ViewBag.City = CityData;

                //var data = await unitOfWork.PortDetails.GetAllAsync();

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
           
        }

        public IActionResult CountryOnchange(PortDetails portDetails)
        {
            var StateData = unitOfWork.States.GetAllAsync().Result.Where(x => x.CountryId == portDetails.Country);

            ViewBag.State = StateData;
            return PartialView("partial/StatesList");
        }
        public IActionResult StateOnchange(PortDetails portDetails)
        {
            var CityData = unitOfWork.Citys.GetAllAsync().Result.Where(x => x.StateId == portDetails.State);

            ViewBag.City = CityData;
            return PartialView("partial/CityList");
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


        public async Task<IActionResult> LoadAll(PortDetails portDetails)
        {
            var data = await unitOfWork.PortDetails.GetAllAsync();

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            if (portDetails.PortName!= null /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.PortName.ToUpper().Contains(portDetails.PortName.ToUpper())).ToList();
            }
            if (portDetails.Country!= null && portDetails.Country != 0 /*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.Country == portDetails.Country).ToList();
            }
            if (portDetails.State!= null && portDetails.State != 0/*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.State == portDetails.State).ToList();
            }
            if (portDetails.City!= null && portDetails.City != 0/*&& customer.FirstName != 0*/)
            {
                data = data.Where(x => x.City == portDetails.City).ToList();
            }
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_ViewAll", data);
            //}
        }


        public async Task<ActionResult> portdetailsSave(PortDetails portDetails)
        {
            var data = await unitOfWork.PortDetails.GetAllAsync();

            if (portDetails.ID > 0)
            {
                var PortName = data.Where(x => x.PortName.ToUpper() == portDetails.PortName.ToUpper() && x.ID != portDetails.ID).ToList();
                var PortCode = data.Where(x => x.PortCode == portDetails.PortCode && x.ID != portDetails.ID).ToList();
                if (PortName != null && PortName.Count > 0 || PortCode != null && PortCode.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("PortName or PortCode already exists.");
                }
                else 
                {
                    await unitOfWork.PortDetails.UpdateAsync(portDetails);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var PortName = data.Where(x => x.PortName.ToUpper() == portDetails.PortName.ToUpper()).ToList();
                var PortCode = data.Where(x => x.PortCode == portDetails.PortCode && x.ID != portDetails.ID).ToList();
                if (PortName != null && PortName.Count > 0 || PortCode != null && PortCode.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("PortName or PortCode already exists.");
                }
                else 
                {
                    await unitOfWork.PortDetails.AddAsync(portDetails);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<IActionResult> LoadTerminalAll()
        {
            var data = await unitOfWork.TerminalDetails.GetAllAsync();
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProjectViewModel>>(response.Data);
            return PartialView("partial/_Terminals", data);
            //}
        }
        public async Task<ActionResult> terminaldetailsSave(TerminalDetails terminalDetails)
        {
            var data = await unitOfWork.TerminalDetails.GetAllAsync();
            if (terminalDetails.ID > 0)
            {
                var TName = data.Where(x => x.TerminalName == terminalDetails.TerminalName && x.PortID == terminalDetails.PortID && x.ID != terminalDetails.ID).ToList();
                var TCode = data.Where(x => x.TerminalCode == terminalDetails.TerminalCode && x.PortID == terminalDetails.PortID && x.ID != terminalDetails.ID).ToList();
                
                if (TName.Count > 0 && TName != null)
                {
                    _toastNotification.AddWarningToastMessage("TerminalName already exists.");
                }
                else if (TCode != null && TCode.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("TerminalCode already exists.");
                }
                else
                {
                    await unitOfWork.TerminalDetails.UpdateAsync(terminalDetails);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var TName = data.Where(x => x.TerminalName == terminalDetails.TerminalName && x.PortID == terminalDetails.PortID).ToList();
                var TCode = data.Where(x => x.TerminalCode == terminalDetails.TerminalCode && x.PortID == terminalDetails.PortID).ToList();
               
                if (TName != null && TName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("TerminalName already exists.");
  
                }
                else if (TCode != null && TCode.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("TerminalCode already exists.");
                }
                else
                {
                    await unitOfWork.TerminalDetails.AddAsync(terminalDetails);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> berthdetailsSave(BerthDetails berthDetails)
        {
            var data = await unitOfWork.BerthDetails.GetAllAsync();
            if (berthDetails.ID > 0)
            {
                var berthName = data.Where(x => x.BerthName.ToUpper() == berthDetails.BerthName.ToUpper() && x.TerminalID == berthDetails.TerminalID && x.ID != berthDetails.ID).ToList();
                var berthCode = data.Where(x => x.BerthCode.ToUpper() == berthDetails.BerthCode.ToUpper() && x.TerminalID == berthDetails.TerminalID && x.ID != berthDetails.ID).ToList();
                var MaxLoa = data.Where(x => x.MaxLoa > berthDetails.MaxLoa).ToList();
                if (berthName.Count > 0 && berthName != null)
                {
                    _toastNotification.AddWarningToastMessage("BerthName already exist.");
                }
                else if (berthCode.Count > 0 && berthCode != null)
                {
                    _toastNotification.AddWarningToastMessage("BerthCode already exist.");
                }
                else
                {
                    await unitOfWork.BerthDetails.UpdateAsync(berthDetails);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var berthName = data.Where(x => x.BerthName.ToUpper() == berthDetails.BerthName.ToUpper() && x.TerminalID == berthDetails.TerminalID).ToList();
                var berthCode = data.Where(x => x.BerthCode.ToUpper() == berthDetails.BerthCode.ToUpper() && x.TerminalID == berthDetails.TerminalID).ToList();
                if (berthName.Count > 0 && berthName != null)
                {
                    _toastNotification.AddWarningToastMessage("BerthName already exist.");
                }
                else if (berthCode.Count > 0 && berthCode != null)
                {
                    _toastNotification.AddWarningToastMessage("BerthCode already exist.");
                }
                else 
                {
                    await unitOfWork.BerthDetails.AddAsync(berthDetails);
                    _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editPortDetails(PortDetails portDetails)
        {
            var data = await unitOfWork.PortDetails.GetByIdAsync(portDetails.ID);
            return Json(new
            {
                portDetails = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editTerminalDetails(TerminalDetails terminalDetails)
        {
            var data = await unitOfWork.TerminalDetails.GetByIdAsync(terminalDetails.ID);
            return Json(new
            {
                terminalDetails = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> editBerthDetails(BerthDetails berthDetails)
        {
            var data = await unitOfWork.BerthDetails.GetByIdAsync(berthDetails.ID);
            return Json(new
            {
                berthdetails = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deletePortDetails(PortDetails portDetails)
        {
            var data = await unitOfWork.PortDetails.DeleteAsync(portDetails.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteTerminalDetails(TerminalDetails terminalDetails)
        {
            var data = await unitOfWork.TerminalDetails.DeleteAsync(terminalDetails.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteBerthDetails(BerthDetails berthDetails)
        {
            var data = await unitOfWork.BerthDetails.DeleteAsync(berthDetails.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> OpenTerminalDetails(PortDetails portDetails)
        {
            ViewBag.PortSelected = portDetails.ID;
            ViewBag.Port = await unitOfWork.PortDetails.GetAllAsync();
            var data = await unitOfWork.TerminalDetails.GetByPortIdAsync(portDetails.ID);
            var Terminals = PartialView("partial/_Terminals", data);
            return Terminals;

        }

        public async Task<ActionResult> OpenBerthDetails(PortDetails portDetails)
        {
            ViewBag.PortSelected = portDetails.ID;
            //ViewBag.TerminalSelected = portDetails.ID;
            ViewBag.Port = await unitOfWork.PortDetails.GetAllAsync();
            ViewBag.Terminals = await unitOfWork.TerminalDetails.GetByPortIdAsync(portDetails.ID);
            var data = await unitOfWork.BerthDetails.GetByPortIdAsync(portDetails.ID);
            var Terminals = PartialView("partial/_Berths", data);
            return Terminals;

        }

    }
}
