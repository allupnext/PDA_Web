using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NotesController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;

        public NotesController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
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
        public async Task<ActionResult> NoteSave(Notes notes)
        {

            if (notes.ID > 0)
            {
                var Notsdata = await unitOfWork.Notes.GetAllAsync();
                var NoteName = Notsdata.Where(x => x.Note.ToUpper() == notes.Note.ToUpper() && x.ID != notes.ID).ToList();
                if (NoteName.Count > 0 && NoteName != null)
                {
                    _toastNotification.AddWarningToastMessage("Note is Already is exist.");
                }
                else
                {
                    if (Notsdata.Count > 0 && Notsdata.Where(x => x.sequnce == notes.sequnce && x.ID != notes.ID).Count() > 0)
                    {
                        _toastNotification.AddErrorToastMessage("Sequnce is Already is exist.");
                        return Json(new
                        {
                            proceed = true,
                            msg = ""
                        });
                    }
                    await unitOfWork.Notes.UpdateAsync(notes);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
            }
            else
            {
                var Notsdata = await unitOfWork.Notes.GetAllAsync();
                var NoteName = Notsdata.Where(x => x.Note.ToUpper() == notes.Note.ToUpper()).ToList();
                if (NoteName != null && NoteName.Count > 0)
                {
                    _toastNotification.AddWarningToastMessage("Note is Already is exist.");
                }
                else
                {
                if (Notsdata.Count > 0 && Notsdata.Where(x => x.sequnce == notes.sequnce).Count() > 0)
                {
                    _toastNotification.AddErrorToastMessage("Sequnce is Already is exist.");
                    return Json(new
                    {
                        proceed = true,
                        msg = ""
                    });
                }
                await unitOfWork.Notes.AddAsync(notes);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
        public async Task<IActionResult> LoadAll()
        {
            var data = await unitOfWork.Notes.GetAllAsync();

            return PartialView("partial/_ViewAll", data);
        }
        public async Task<ActionResult> editNote(Notes notes)
        {
            var data = await unitOfWork.Notes.GetByIdAsync(notes.ID);
            return Json(new
            {
                Notes = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteNote(Notes notes)
        {
            var data = await unitOfWork.Notes.DeleteAsync(notes.ID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
    }
}
