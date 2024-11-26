using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDA_Web.Models;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;
using PDAEstimator_Infrastructure_Shared;
using PDAEstimator_Infrastructure_Shared.Services;
using Rotativa.AspNetCore;
using SelectPdf;
using System.Data;
using System.Globalization;

namespace PDA_Web.Controllers
{
    public class PDAController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        private readonly IEmailSender _emailSender;

        public PDAController(IUnitOfWork unitOfWork, IToastNotification toastNotification, IWebHostEnvironment hostEnvironment, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;

        }
        public IActionResult OpenBerthDetailsPopUp(PDAEstimator PDAEstimitor)
        {
            var BearthDetailData = unitOfWork.BerthDetails.GetAllAsync().Result.Where(x => x.TerminalID == PDAEstimitor.TerminalID);
            if (BearthDetailData != null && BearthDetailData.Count() > 0)
                BearthDetailData = BearthDetailData.Where(x => x.BerthStatus == true).ToList();
            var Terminals = PartialView("partial/_Berths", BearthDetailData);
            return PartialView("partial/_Berths", BearthDetailData);
        }

        public async Task<IActionResult> CurrencyOnchange(int Currency)
        {
            var roedata = await unitOfWork.Rates.GetAlllistbyLoadAllforDefaultRoEName();
            var roeRate = roedata.Where(x => x.ID == Currency).FirstOrDefault();


            return Json(new
            {
                roeRate = roeRate == null ? 0 : roeRate.ROERate,
                proceed = true,
                msg = ""
            });
        }

        public async Task<IActionResult> PDAEstimatorDonwload(int id)
        {
            PDAEstimatorOutPutView pDAEstimatorOutPut = new PDAEstimatorOutPutView();
            pDAEstimatorOutPut = await GetPDA(id);
            //string PDAfilename = "PDA_" + pDAEstimatorOutPut.PortName + "_" + pDAEstimatorOutPut.PDAEstimatorID + ".pdf";
            //string path = $"{_hostEnvironment.WebRootPath}\\PDAFile\\";
            //var rootpath = Path.Combine(path, PDAfilename);
            //rootpath = Path.GetFullPath(rootpath);
            return new ViewAsPdf("PDAEstimator", pDAEstimatorOutPut)
            {
                //FileName = "MyPdf.pdf";
                FileName = "PDA_" + pDAEstimatorOutPut.PortName + "_" + pDAEstimatorOutPut.ActivityType + "_ " + pDAEstimatorOutPut.CargoName + "_" + pDAEstimatorOutPut.PDAEstimatorID + ".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A3
            };
        }

        public async Task<IActionResult> PDAEstimatorDownloadandSave(int id)
        {
            PDAEstimatorOutPutView pDAEstimatorOutPut = new PDAEstimatorOutPutView();
            pDAEstimatorOutPut = await GetPDA(id);
            string PDAfilename = "PDA_" + pDAEstimatorOutPut.PortName + "_" + pDAEstimatorOutPut.ActivityType + "_ " + pDAEstimatorOutPut.CargoName + "_" + pDAEstimatorOutPut.PDAEstimatorID + ".pdf";
            string path = $"{_hostEnvironment.WebRootPath}\\PDAFile\\";
            var rootpath = Path.Combine(path, PDAfilename);
            rootpath = Path.GetFullPath(rootpath);
            return new ViewAsPdf("PDAEstimator", pDAEstimatorOutPut)
            {
                //FileName = "MyPdf.pdf";
                FileName = "PDA_" + pDAEstimatorOutPut.PortName + "_" + pDAEstimatorOutPut.ActivityType + "_ " + pDAEstimatorOutPut.CargoName + "_" + pDAEstimatorOutPut.PDAEstimatorID + ".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A3,
                SaveOnServerPath = rootpath
            };
        }

        public async Task<IActionResult> PDAEstimatorEmail(int id)
        {
            PDAEstimatorOutPutView pDAEstimatorOutPut = new PDAEstimatorOutPutView();
            pDAEstimatorOutPut = await GetPDA(id);
            string PDAfilename = "PDA_" + pDAEstimatorOutPut.PortName + "_"+ pDAEstimatorOutPut.ActivityType + "_ " + pDAEstimatorOutPut.CargoName + "_" + pDAEstimatorOutPut.PDAEstimatorID + ".pdf";
            string path = $"{_hostEnvironment.WebRootPath}\\PDAFile\\";
            var rootpath = Path.Combine(path, PDAfilename);
            rootpath = Path.GetFullPath(rootpath);

            var custid = HttpContext.Session.GetString("ID");

            if (!string.IsNullOrEmpty(custid))
            {
                var custuser = unitOfWork.CustomerUserMaster.GetByIdAsync(Convert.ToInt32(custid)).Result;

                if (custuser != null)
                {
                    string customerfullname = string.Concat(custuser.FirstName, ' ', custuser.LastName);

                    string Content = "<html><head><title> PDA .</title></head><body><p> Dear "+ customerfullname + ", <br><br>  Please find attached PDA "+ id + " for Port "+ pDAEstimatorOutPut.PortName + " For further details/query on the attached PDA, you may connect/email us on bulkopsindia@samsarashipping.com <br> Please email your feedback/suggestions to EPDA.Support@samsaragroup.com  <br><br> <b>Regards <br> EPDA Portal Team</b> </p></body></html>";
                    string Subject = "EPDA Portal: PDA "+ id + " for Port "+ pDAEstimatorOutPut.PortName;
                    List<string> ccrecipients = new List<string>();

                    List<string> recipients = new List<string>
                    {
                        custuser.Email
                    };
                    string FromCompany = "";
                    string ToEmail = "";

                   var customer = await unitOfWork.Customer.GetByIdAsync(custuser.CustomerId);
                    var companydata = await unitOfWork.Company.GetAlllistAsync();
                    int Samsaracompanyid = 0;
                    if (companydata != null && companydata.Count > 0)
                    {
                        var samsaracompanydata = companydata.Where(x => x.CompanyName.ToLower() == "samsara shipping private limited");
                        if (samsaracompanydata.Count() > 0)
                        {
                            Samsaracompanyid = samsaracompanydata.FirstOrDefault().CompanyId;
                        }
                    }

                    int companyid = customer.PrimaryCompanyId != null ? (int)customer.PrimaryCompanyId : Samsaracompanyid;

                    var emailconfig = await unitOfWork.EmailNotificationConfigurations.GetByCompanyandProcessNameAsync(companyid, "PDA Generate");
                    if (emailconfig != null)
                    {
                        ToEmail = emailconfig.ToEmail;
                        FromCompany = emailconfig.FromEmail;
                        if (emailconfig.ToEmail != null)
                        {
                            ccrecipients = ToEmail.Split(',').ToList();
                        }
                    }

                    var Msg = new Message(recipients, ccrecipients, Subject, Content, FromCompany);
                    /*            _toastNotification.AddSuccessToastMessage("Email hase been sent to given Email Address");*/
                    _emailSender.SendEmail(Msg, rootpath);
                }
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<PDAEstimatorOutPutView> GetPDA(int id)
        {
            //PDAEstimatorOutPutView pDAEstimatorOutPut = new PDAEstimatorOutPutView();
            //pDAEstimatorOutPut = await PDAModelPrePared(pDAEstimatorOutPut, id);

            PDAEstimatorOutPutView pDAEstimatorOutPutView = new PDAEstimatorOutPutView();

            var pDAEstimatorOutPutdata = await unitOfWork.PDAEstimitorOUTPUT.GetByIdAsync(Convert.ToInt64(id));
            if (pDAEstimatorOutPutdata != null)
            {
                var PDAData = unitOfWork.PDAEstimitor.GetAlllistAsync().Result.Where(x => x.PDAEstimatorID == id).FirstOrDefault();
                var TaxData = unitOfWork.Taxs.GetAllAsync().Result;

                pDAEstimatorOutPutView = new PDAEstimatorOutPutView()
                {
                    PDAEstimatorID = PDAData.PDAEstimatorID,
                    CustomerID = PDAData.CustomerID,
                    FirstName = PDAData.CustomerCompanyName,
                    VesselName = PDAData.VesselName,
                    PortName = PDAData.PortName,
                    PortID = PDAData.PortID,
                    ActivityTypeId = PDAData.ActivityTypeId,
                    BerthStayDay = PDAData.BerthStayDay,
                    TerminalName = PDAData.TerminalName,
                    ETA = PDAData.ETA,
                    ActivityType = PDAData.ActivityType,
                    TerminalID = PDAData.TerminalID,
                    CallTypeName = PDAData.CallTypeName,
                    CallTypeID = PDAData.CallTypeID,
                    CurrencyName = PDAData.CurrencyName,
                    CargoID = PDAData.CargoID,
                    CargoQty = PDAData.CargoQty,
                    CargoQtyCBM = PDAData.CargoQtyCBM,
                    CargoUnitofMasurement = PDAData.CargoUnitofMasurement,
                    LoadDischargeRate = PDAData.LoadDischargeRate,
                    CargoName = PDAData.CargoName,
                    CurrencyID = PDAData.CurrencyID,
                    ROE = PDAData.ROE,
                    DWT = PDAData.DWT,
                    ArrivalDraft = PDAData.ArrivalDraft,
                    GRT = PDAData.GRT,
                    RGRT = PDAData.RGRT,
                    NRT = PDAData.NRT,
                    BerthStay = PDAData.BerthStay,
                    AnchorageStay = PDAData.AnchorageStay,
                    LOA = PDAData.LOA,
                    Beam = PDAData.Beam,
                    IsDeleted = PDAData.IsDeleted,
                    InternalCompanyID = PDAData.InternalCompanyID,
                    BerthStayShift = PDAData.BerthStayShift,
                    BerthStayShiftCoastal = PDAData.BerthStayShiftCoastal,
                    BerthStayDayCoastal = PDAData.BerthStayDayCoastal,
                    BerthStayHoursCoastal = PDAData.BerthStayHoursCoastal,
                    VesselBallast = PDAData.VesselBallast,
                    BerthID = PDAData.BerthId,
                    BerthName = PDAData.BerthName,
                    CompanyName = pDAEstimatorOutPutdata.CompanyName,
                    CompanyAddress1 = pDAEstimatorOutPutdata.CompanyAddress1.ToUpper(),
                    CompanyAddress2 = pDAEstimatorOutPutdata.CompanyAddress2.ToUpper(),
                    CompanyTelephone = pDAEstimatorOutPutdata.CompanyTelephone.ToUpper(),
                    CompanyAlterTel = pDAEstimatorOutPutdata.CompanyAlterTel.ToUpper(),
                    CompanyEmail = pDAEstimatorOutPutdata.CompanyEmail.ToUpper(),
                    CompanyLogo = pDAEstimatorOutPutdata.CompanyLogo,
                    BaseCurrencyCode = pDAEstimatorOutPutdata.BaseCurrencyCode,
                    BaseCurrencyCodeID = pDAEstimatorOutPutdata.BaseCurrencyCodeID,
                    DefaultCurrencyCode = pDAEstimatorOutPutdata.DefaultCurrencyCode,
                    DefaultCurrencyCodeID = pDAEstimatorOutPutdata.DefaultCurrencyCodeID,
                    Disclaimer = pDAEstimatorOutPutdata.Disclaimer

                };

                string Companylogo = pDAEstimatorOutPutView.CompanyLogo;


                string fullPath = GetFullPathOfFile(pDAEstimatorOutPutView.CompanyLogo.Replace("\"", ""));
                var test = !System.IO.File.Exists(fullPath);
                //Read the File data into Byte Array.
                byte[] bytes;

                bytes = new byte[1024];

                if (System.IO.File.Exists(fullPath))
                {
                    bytes = System.IO.File.ReadAllBytes(fullPath);
                    string file = Convert.ToBase64String(bytes);
                    pDAEstimatorOutPutView.CompanyLogoBase64 = "data:image/png;base64, " + file;
                }
                else
                {
                    pDAEstimatorOutPutView.CompanyLogoBase64 = "";
                }

                BankMaster bankMaster = new BankMaster()
                {
                    NameofBeneficiary = pDAEstimatorOutPutdata.NameofBeneficiary,
                    BeneficiaryAddress = pDAEstimatorOutPutdata.BeneficiaryAddress,
                    AccountNo = pDAEstimatorOutPutdata.AccountNo,
                    Beneficiary_Bank_Name = pDAEstimatorOutPutdata.Beneficiary_Bank_Name,
                    Beneficiary_Bank_Address = pDAEstimatorOutPutdata.Beneficiary_Bank_Address,
                    Beneficiary_RTGS_NEFT_IFSC_Code = pDAEstimatorOutPutdata.Beneficiary_RTGS_NEFT_IFSC_Code,
                    Beneficiary_Bank_Swift_Code = pDAEstimatorOutPutdata.Beneficiary_Bank_Swift_Code,
                    Intermediary_Bank = pDAEstimatorOutPutdata.Intermediary_Bank,
                    Intermediary_Bank_Swift_Code = pDAEstimatorOutPutdata.Intermediary_Bank_Swift_Code,
                };

                pDAEstimatorOutPutView.BankMaster = bankMaster;
                pDAEstimatorOutPutView.Expenses = unitOfWork.Expenses.GetAllAsync().Result.OrderBy(x => x.sequnce).ToList();

                var Notesdata = await unitOfWork.PDAEstimitorOUTNote.GetAllNotesByPDAEstimatorOutPutIDAsync(pDAEstimatorOutPutdata.PDAEstimatorOutPutID);
                List<Notes> notes = new List<Notes>();
                if (Notesdata != null)
                {
                    foreach (var noteitem in Notesdata)
                    {
                        Notes note = new Notes
                        {
                            Note = noteitem.Note,
                            sequnce = noteitem.sequnce
                        };

                        notes.Add(note);
                    }
                }

                pDAEstimatorOutPutView.NotesData = notes;

                var pDAEstimatorOutPutTariffdata = await unitOfWork.PDAEstimatorOutPutTariff.GetAllByPDAEstimatorIDAsync(pDAEstimatorOutPutdata.PDAEstimatorOutPutID);
                List<PDATariffRateList> pDATariffRateLists = new List<PDATariffRateList>();
                if (pDAEstimatorOutPutTariffdata != null && pDAEstimatorOutPutTariffdata.Count() > 0)
                {
                    foreach (var pDAEstimatorOutPutTariff in pDAEstimatorOutPutTariffdata)
                    {
                        PDATariffRateList pDATariffRateList = new PDATariffRateList
                        {
                            Amount = pDAEstimatorOutPutTariff.Amount,
                            Remark = pDAEstimatorOutPutTariff.Remark,
                            Rate = pDAEstimatorOutPutTariff.Rate,
                            ExpenseCategoryID = pDAEstimatorOutPutTariff.ExpenseCategoryID,
                            ChargeCodeName = pDAEstimatorOutPutTariff.ChargeCodeName,
                            UNITS = pDAEstimatorOutPutTariff.UNITS,
                            TaxID = pDAEstimatorOutPutTariff.TaxID,
                            CurrencyID = pDAEstimatorOutPutTariff.CurrencyID

                        };
                        pDAEstimatorOutPutView.Taxrate = pDAEstimatorOutPutTariff.Taxrate;
                        pDATariffRateLists.Add(pDATariffRateList);
                    }
                }
                pDAEstimatorOutPutView.TariffRateList = pDATariffRateLists;
            }
            else
            {
                pDAEstimatorOutPutView = await PDAModelPrePared(pDAEstimatorOutPutView, id);

            }
            return pDAEstimatorOutPutView;
        }

        public async Task<IActionResult> PDAEstimator(int id)
        {
            PDAEstimatorOutPutView pDAEstimatorOutPut = new PDAEstimatorOutPutView();
            pDAEstimatorOutPut = await GetPDA(id);

            return View(pDAEstimatorOutPut);
            //return new ViewAsPdf(pDAEstimatorOutPut);
        }

        public async Task<PDAEstimatorOutPutView> PDAModelPrePared(PDAEstimatorOutPutView pDAEstimatorOutPut, int id)
        {
            if (id > 0)
            {
                var PDAData = unitOfWork.PDAEstimitor.GetAlllistAsync().Result.Where(x => x.PDAEstimatorID == id).FirstOrDefault();
                var TaxData = unitOfWork.Taxs.GetAllAsync().Result;
                pDAEstimatorOutPut = new PDAEstimatorOutPutView()
                {
                    PDAEstimatorID = PDAData.PDAEstimatorID,
                    CustomerID = PDAData.CustomerID,
                    FirstName = PDAData.CustomerCompanyName,
                    VesselName = PDAData.VesselName,
                    PortName = PDAData.PortName,
                    PortID = PDAData.PortID,
                    ActivityTypeId = PDAData.ActivityTypeId,
                    BerthStayDay = PDAData.BerthStayDay,
                    TerminalName = PDAData.TerminalName,
                    ETA = PDAData.ETA,
                    ActivityType = PDAData.ActivityType,
                    TerminalID = PDAData.TerminalID,
                    CallTypeName = PDAData.CallTypeName,
                    CallTypeID = PDAData.CallTypeID,
                    CurrencyName = PDAData.CurrencyName,
                    CargoID = PDAData.CargoID,
                    CargoQty = PDAData.CargoQty,
                    CargoQtyCBM = PDAData.CargoQtyCBM,
                    CargoUnitofMasurement = PDAData.CargoUnitofMasurement,
                    LoadDischargeRate = PDAData.LoadDischargeRate,
                    CargoName = PDAData.CargoName,
                    CurrencyID = PDAData.CurrencyID,
                    ROE = PDAData.ROE,
                    DWT = PDAData.DWT,
                    ArrivalDraft = PDAData.ArrivalDraft,
                    GRT = PDAData.GRT,
                    RGRT = PDAData.RGRT,
                    NRT = PDAData.NRT,
                    BerthStay = PDAData.BerthStay,
                    AnchorageStay = PDAData.AnchorageStay,
                    LOA = PDAData.LOA,
                    Beam = PDAData.Beam,
                    IsDeleted = PDAData.IsDeleted,
                    InternalCompanyID = PDAData.InternalCompanyID,
                    CompanyName = PDAData.InternalCompanyName,
                    BerthStayShift = PDAData.BerthStayShift,
                    BerthStayShiftCoastal = PDAData.BerthStayShiftCoastal,
                    BerthStayDayCoastal = PDAData.BerthStayDayCoastal,
                    BerthStayHoursCoastal = PDAData.BerthStayHoursCoastal,
                    VesselBallast = PDAData.VesselBallast,
                    BerthID = PDAData.BerthId,
                    BerthName = PDAData.BerthName
                };

                var CompanyData = unitOfWork.Company.GetAlllistAsync().Result.Where(x => x.CompanyId == PDAData.InternalCompanyID).FirstOrDefault();
                string Addressline2 = "";

                if (CompanyData != null)
                {
                    if (CompanyData.Address2 != null)
                    {
                        Addressline2 = CompanyData.Address1.ToUpper() + ", " + CompanyData.Address2.ToUpper() + ", ";
                    }
                    else
                    {
                        Addressline2 = CompanyData.Address1.ToUpper() + ", ";
                    }

                    pDAEstimatorOutPut.CompanyAddress1 = Addressline2;

                }
                else
                {
                    pDAEstimatorOutPut.CompanyAddress1 = "";
                }
                pDAEstimatorOutPut.CompanyAddress2 = CompanyData.CityName.ToUpper() + ", " + CompanyData.StateName.ToUpper() + ", " + CompanyData.CountryName.ToUpper();
                pDAEstimatorOutPut.CompanyTelephone = CompanyData.Telephone;
                if (CompanyData.AlterTel.Split("-")[1] != "")
                {
                    pDAEstimatorOutPut.CompanyAlterTel = CompanyData.AlterTel;
                }
                else
                {
                    pDAEstimatorOutPut.CompanyAlterTel = "";
                }
                pDAEstimatorOutPut.CompanyEmail = CompanyData.Email.ToUpper();
                pDAEstimatorOutPut.CompanyLogo = CompanyData.CompanyLog;
                string Companylogo = pDAEstimatorOutPut.CompanyLogo;


                string fullPath = GetFullPathOfFile(pDAEstimatorOutPut.CompanyLogo.Replace("\"", ""));
                var test = !System.IO.File.Exists(fullPath);
                //Read the File data into Byte Array.
                byte[] bytes;

                bytes = new byte[1024];

                if (System.IO.File.Exists(fullPath))
                {
                    bytes = System.IO.File.ReadAllBytes(fullPath);
                    string file = Convert.ToBase64String(bytes);
                    pDAEstimatorOutPut.CompanyLogoBase64 = "data:image/png;base64, " + file;
                }
                else
                {
                    pDAEstimatorOutPut.CompanyLogoBase64 = "";
                }


                //image.Dispose();
                /*                byte[] bytes = File.ReadAllBytes(@"image.png");*/


                //string base64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Companylogo));




                var custdata = unitOfWork.Customer.GetByIdAsync(PDAData.CustomerID).Result;
                if (custdata != null && custdata.BankID != null)
                {
                    pDAEstimatorOutPut.BankMaster = unitOfWork.BankMaster.GetByIdAsync(custdata.BankID).Result;
                }

                pDAEstimatorOutPut.Expenses = unitOfWork.Expenses.GetAllAsync().Result.OrderBy(x => x.sequnce).ToList();

                pDAEstimatorOutPut.ChargeCodes = unitOfWork.ChargeCodes.GetAlllistAsync().Result.OrderBy(x => x.Sequence).ToList();

                pDAEstimatorOutPut.NotesData = unitOfWork.PDAEstimitor.GetNotes().Result.ToList();
                var currencyData = unitOfWork.Currencys.GetAllAsync().Result;
                pDAEstimatorOutPut.BaseCurrencyCode = currencyData.Where(x => x.BaseCurrency == true) != null ? currencyData.Where(x => x.BaseCurrency == true).FirstOrDefault().CurrencyCode : "";
                pDAEstimatorOutPut.DefaultCurrencyCode = currencyData.Where(x => x.DefaultCurrecny == true) != null ? currencyData.Where(x => x.DefaultCurrecny == true).FirstOrDefault().CurrencyCode : "";
                pDAEstimatorOutPut.BaseCurrencyCodeID = currencyData.Where(x => x.BaseCurrency == true) != null ? currencyData.Where(x => x.BaseCurrency == true).FirstOrDefault().ID : 0;
                pDAEstimatorOutPut.DefaultCurrencyCodeID = currencyData.Where(x => x.DefaultCurrecny == true) != null ? currencyData.Where(x => x.DefaultCurrecny == true).FirstOrDefault().ID : 0;

                //var triffdata = unitOfWork.PDAEstimitor.GetAllPDA_Tariff(pDAEstimatorOutPut.PortID).Result.Where(x => (x.CallTypeID == pDAEstimatorOutPut.CallTypeID || x.CallTypeID == null) && (x.SlabFrom == null || x.SlabFrom <= pDAEstimatorOutPut.GRT)) ;
                var triffdata = unitOfWork.PDAEstimitor.GetAllPDA_Tariff(pDAEstimatorOutPut.PortID, pDAEstimatorOutPut.ETA != null ? (DateTime)pDAEstimatorOutPut.ETA : DateTime.Now.Date).Result.Where(x => (x.CallTypeID == pDAEstimatorOutPut.CallTypeID || x.CallTypeID == null) && (x.TerminalID == pDAEstimatorOutPut.TerminalID || x.TerminalID == null) && (x.BerthID == pDAEstimatorOutPut.BerthID || x.BerthID == null || x.BerthID == 0) && (x.CargoID == pDAEstimatorOutPut.CargoID || x.CargoID == null) && (x.OperationTypeID == pDAEstimatorOutPut.ActivityTypeId || x.OperationTypeID == null) && (x.VesselBallast == pDAEstimatorOutPut.VesselBallast || x.VesselBallast == 0)).OrderBy(o => o.ChargeCodeSequence).ThenBy(o => o.SlabFrom).ThenBy(o => o.TariffRateID);
                List<PDATariffRateList> pDATariffRateList = new List<PDATariffRateList>();
                decimal taxrate = 0;
                foreach (var triff in triffdata)
                {
                    if (triff.FormulaID != null && triff.FormulaID > 0)
                    {
                        string formulastring = string.Empty;
                        var formulatransdata = await unitOfWork.FormulaTransaction.GetAllTransAsync((int)triff.FormulaID);
                        if (formulatransdata.Count > 0)
                        {
                            long? slabattributvalue = 0;
                            if (triff.SlabName == "GRT")
                                slabattributvalue = pDAEstimatorOutPut.GRT;
                            else if (triff.SlabName == "RGRT")
                                slabattributvalue = pDAEstimatorOutPut.RGRT;
                            else if (triff.SlabName == "NRT")
                                slabattributvalue = pDAEstimatorOutPut.NRT;
                            else if (triff.SlabName == "BSTH" || triff.SlabName == "BSTHF")
                                slabattributvalue = pDAEstimatorOutPut.BerthStay;
                            else if (triff.SlabName == "BSTS" || triff.SlabName == "BSTSF")
                                slabattributvalue = pDAEstimatorOutPut.BerthStayShift;
                            else if (triff.SlabName == "BSTD" || triff.SlabName == "BSTDF")
                                slabattributvalue = pDAEstimatorOutPut.BerthStayDay;
                            else if (triff.SlabName == "BSTHC")
                                slabattributvalue = pDAEstimatorOutPut.BerthStayHoursCoastal;
                            else if (triff.SlabName == "BSTSC")
                                slabattributvalue = pDAEstimatorOutPut.BerthStayShiftCoastal;
                            else if (triff.SlabName == "BSTDC")
                                slabattributvalue = pDAEstimatorOutPut.BerthStayDayCoastal;
                            else if (triff.SlabName == "AST")
                                slabattributvalue = pDAEstimatorOutPut.AnchorageStay;
                            else if (triff.SlabName == "QTYMT")
                                slabattributvalue = pDAEstimatorOutPut.CargoQty;
                            else if (triff.SlabName == "QTYCBM")
                                slabattributvalue = pDAEstimatorOutPut.CargoQtyCBM;
                            UnitCalculation(triff, pDAEstimatorOutPut.GRT, (long)slabattributvalue);

                            foreach (var formularTransList in formulatransdata)
                            {
                                if (formularTransList.formulaAttributeID > 0)
                                {
                                    string FormulaAttributedata = formularTransList.formulaAttributeName;
                                    if (FormulaAttributedata.Contains("GRT"))
                                    {
                                        if (triff.SlabID == null || triff.SlabID == 0)
                                        {
                                            triff.UNITS = pDAEstimatorOutPut.GRT;
                                        }

                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.GRT.ToString() : pDAEstimatorOutPut.GRT.ToString();
                                        }

                                        //UnitCalculation(triff, pDAEstimatorOutPut.GRT, pDAEstimatorOutPut);
                                    }
                                    else if (FormulaAttributedata.Contains("RGRT"))
                                    {
                                        if (triff.SlabID == null || triff.SlabID == 0)
                                        {
                                            triff.UNITS = pDAEstimatorOutPut.RGRT;
                                        }

                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.RGRT.ToString() : pDAEstimatorOutPut.RGRT.ToString();
                                        }

                                        //UnitCalculation(triff, pDAEstimatorOutPut.GRT, pDAEstimatorOutPut);
                                    }
                                    else if (FormulaAttributedata.Contains("NRT"))
                                    {
                                        if (triff.SlabID == null || triff.SlabID == 0)
                                        {
                                            triff.UNITS = pDAEstimatorOutPut.NRT;
                                        }
                                        //UnitCalculation(triff, pDAEstimatorOutPut.NRT, pDAEstimatorOutPut);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.NRT.ToString() : pDAEstimatorOutPut.NRT.ToString();
                                        }
                                    }
                                    else if (FormulaAttributedata == "BSTH" || FormulaAttributedata == "BSTHF")
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStay);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStay.ToString() : pDAEstimatorOutPut.BerthStay.ToString();
                                        }
                                    }
                                    else if (FormulaAttributedata == "BSTS" || FormulaAttributedata == "BSTSF")
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayShift);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayShift.ToString() : pDAEstimatorOutPut.BerthStayShift.ToString();
                                    }
                                    else if (FormulaAttributedata == "BSTD" || FormulaAttributedata == "BSTDF")
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayDay);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayDay.ToString() : pDAEstimatorOutPut.BerthStayDay.ToString();
                                        }
                                    }
                                    else if (FormulaAttributedata.Contains("BSTHC"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayHoursCoastal);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayHoursCoastal.ToString() : pDAEstimatorOutPut.BerthStayHoursCoastal.ToString();
                                        }
                                    }
                                    else if (FormulaAttributedata.Contains("BSTSC"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayShiftCoastal);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayShiftCoastal.ToString() : pDAEstimatorOutPut.BerthStayShiftCoastal.ToString();
                                        }
                                    }
                                    else if (FormulaAttributedata.Contains("BSTDC"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayDayCoastal);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayDayCoastal.ToString() : pDAEstimatorOutPut.BerthStayDayCoastal.ToString();
                                        }
                                    }
                                    else if (FormulaAttributedata.Contains("AST"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.AnchorageStay);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.AnchorageStay.ToString() : pDAEstimatorOutPut.AnchorageStay.ToString();
                                        }
                                    }
                                    else if (FormulaAttributedata.Contains("QTYMT"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.CargoQty);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.CargoQty.ToString() : pDAEstimatorOutPut.CargoQty.ToString();
                                        }
                                    }
                                    else if (FormulaAttributedata.Contains("QTYCBM"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.CargoQtyCBM);
                                        if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                        }
                                        else
                                        {
                                            formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.CargoQtyCBM.ToString() : pDAEstimatorOutPut.CargoQtyCBM.ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (triff.UNITS == null || triff.UNITS == 0)
                                        {
                                            triff.UNITS = 1;
                                        }
                                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaAttributeName : formularTransList.formulaAttributeName;
                                    }
                                }
                                if (formularTransList.formulaSlabID > 0)
                                    formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaSlabName : formularTransList.formulaSlabName;
                                if (formularTransList.formulaOperatorID > 0)
                                    formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaOperatorName : formularTransList.formulaOperatorName;
                                if (formularTransList.formulaValue > 0)
                                    formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaValue : formularTransList.formulaValue.ToString();
                            }
                        }
                        else
                        {
                            triff.UNITS = 1;

                        }
                        triff.Formula = formulastring;
                        triff.FormulaID = (int)triff.FormulaID;
                    }
                    else
                    {
                        triff.UNITS = 1;
                    }

                    if (triff.Formula != "")
                    {
                        DataTable dt = new DataTable();
                        var amount = new object();
                        try
                        {
                            amount = dt.Compute(triff.Formula, "");
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                        decimal amt = Convert.ToDecimal(amount);
                        amt = Math.Abs(amt);
                        amt = amt * triff.Rate;
                        triff.Amount = amt;
                    }
                    else
                    {
                        triff.Amount = 0;
                    }

                    if (triff.CurrencyID == pDAEstimatorOutPut.BaseCurrencyCodeID)
                    {
                        if (triff.TaxID != null && triff.TaxID != 0)
                        {
                            var tax = TaxData.Where(x => x.ID == triff.TaxID).Select(x => x.TaxRate).FirstOrDefault();
                            triff.GSTBase = (triff.Amount * tax) / 100;
                            taxrate = tax;
                        }
                        else
                        {
                            triff.GSTBase = 0;
                        }
                    }
                    if (triff.CurrencyID == pDAEstimatorOutPut.DefaultCurrencyCodeID)
                    {
                        if (triff.TaxID != null && triff.TaxID != 0)
                        {
                            var tax = TaxData.Where(x => x.ID == triff.TaxID).Select(x => x.TaxRate).FirstOrDefault();
                            triff.GSTDefault = (triff.Amount * tax) / 100;
                            taxrate = tax;
                        }
                        else
                        {
                            triff.GSTDefault = 0;
                        }
                    }

                    if (triff.NonIncreemental)
                    {
                        pDATariffRateList.Add(triff);
                    }
                }
                pDAEstimatorOutPut.TariffRateList = pDATariffRateList;
                pDAEstimatorOutPut.Taxrate = taxrate;

            }

            return pDAEstimatorOutPut;
        }

        private string GetFullPathOfFile1(string fileName)
        {
            return $"{_hostEnvironment.WebRootPath}\\uploads\\{fileName}";
        }
        private string GetFullPathOfFile(string fileName)
        {
            return $"{_hostEnvironment.WebRootPath}\\companylogo\\{fileName}";
        }

        public PDATariffRateList UnitCalculation(PDATariffRateList triff, long? attributvalue, long slabattributvalue, bool Range_Tariff = false)
        {
            decimal? units = 0;


            if (triff.SlabID != null && triff.SlabID > 0)
            {
                if (triff.SlabIncreemental == 1)
                {
                    if (triff.SlabTo != 0 && slabattributvalue >= triff.SlabTo && triff.SlabFrom == 1)
                    {
                        units = triff.SlabTo;
                    }
                    else if (triff.SlabTo != 0 && slabattributvalue >= triff.SlabTo && triff.SlabFrom != 1)
                    {
                        units = (triff.SlabTo - triff.SlabFrom) + 1;
                        units = Math.Abs(Convert.ToDecimal(units));
                    }
                    else if (slabattributvalue >= triff.SlabFrom)
                    {
                        units = (slabattributvalue - triff.SlabFrom) + 1;
                        units = Math.Abs(Convert.ToDecimal(units));
                    }
                    triff.NonIncreemental = true;
                }
                else
                {
                    if (triff.SlabFrom <= slabattributvalue && (triff.SlabTo == 0 || triff.SlabTo >= slabattributvalue))
                    {
                        if (Range_Tariff)
                        {
                            units = (slabattributvalue - triff.SlabFrom) + 1;
                            units = Math.Abs(Convert.ToDecimal(units));
                        }
                        else
                        {
                            units = slabattributvalue;
                            units = Math.Abs(Convert.ToDecimal(units));
                        }
                        triff.NonIncreemental = true;
                    }
                    else
                    {

                        triff.NonIncreemental = false;
                    }
                }
            }
            else
            {
                units = slabattributvalue;
            }

            triff.UNITS = units;
            return triff;
        }

        public async Task<IActionResult> Index()
        {

            var userid = HttpContext.Session.GetString("CustID");
            if (!string.IsNullOrEmpty(userid))
            {
                var CustomerData = await unitOfWork.Customer.GetAllAsync();
                ViewBag.Customers = CustomerData;
                // Temp Solution START
                var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
                ViewBag.UserPermissionModel = UserPermissionModel;
                var Currentuser = HttpContext.Session.GetString("CustID");

                var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
                ViewBag.UserRoleName = UserRole;
                // Temp Solution END

                var CallTypeData = await unitOfWork.CallTypes.GetAllAsync();
                if (CallTypeData.Count > 0)
                    CallTypeData = CallTypeData.Where(x => x.Status == true).ToList();
                ViewBag.CallType = CallTypeData;

                var EmployeeCode = await unitOfWork.User.GetAllAsync();
                ViewBag.EmployeeCode = EmployeeCode;

                var PortActivityData = await unitOfWork.PortActivities.GetAllAsync();
                ViewBag.PortActivity = PortActivityData;

                var PortCallData = await unitOfWork.PortDetails.GetAllAsync();
                if (PortCallData.Count > 0)
                    PortCallData = PortCallData.Where(x => x.Status == true).ToList();
                ViewBag.Port = PortCallData;


                var TerminalData = await unitOfWork.TerminalDetails.GetAllAsync();
                if (TerminalData.Count > 0)
                    TerminalData = TerminalData.Where(x => x.Status == true).ToList();
                ViewBag.Terminal = TerminalData;

                var data1 = await unitOfWork.BerthDetails.GetAllAsync();
                if (data1.Count > 0)
                    data1 = data1.Where(x => x.BerthStatus == true).ToList();
                ViewBag.Berth = data1;

                var CargoType = await unitOfWork.CargoDetails.GetAllAsync();
                if (CargoType.Count > 0)
                    CargoType = CargoType.Where(x => x.CargoStatus == true).ToList();
                ViewBag.Cargo = CargoType;

                var dataCurrency = await unitOfWork.Currencys.GetAllwithoutBaseCurrencyAsync();
                if (dataCurrency.Count > 0)
                    dataCurrency = dataCurrency.Where(x => x.Status == true).ToList();
                ViewBag.Currency = dataCurrency;

                var ROEData = await unitOfWork.ROENames.GetAllAsync();
                 if (ROEData.Count > 0)
                    ROEData = ROEData.Where(x => x.Status == true).ToList();
                ViewBag.ROEName = ROEData;

                var CompanyData = await unitOfWork.Company.GetAllAsync();
                if (CompanyData.Count > 0)
                    CompanyData = CompanyData.Where(x => x.Status == true).ToList();
                ViewBag.Companys = CompanyData;

                var DefaultCurrecnydata = dataCurrency.Where(x => x.DefaultCurrecny == true);
                if (DefaultCurrecnydata != null && DefaultCurrecnydata.Count() > 0)
                {
                    ViewBag.CurrencyDefault = DefaultCurrecnydata.FirstOrDefault().ID;
                }
                return View();
            }
            else
            {
                return RedirectToAction("index", "Login");
            }
        }
        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            //string path = Path.Combine(this.Environment.WebRootPath, "Files/") + fileName;
            string fullPath = GetFullPathOfFile1(fileName.Replace("\"", ""));
            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(fullPath);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
        public async Task<IActionResult> TempData()
        {

            return PartialView("partial/_ViewAll");
        }

        public async Task<IActionResult> CargoLoad(int selectedTerminalId, int selectedPortId)
        {
            var cargoList = await unitOfWork.PDAEstimitor.GetCargoByTerminalAndPortAsync(selectedTerminalId, selectedPortId);
            ViewBag.Cargo = cargoList;
            return PartialView("partial/_CargoList");
        }

        public async Task<IActionResult> CustomerOnchange(int CustomerId)
        {
            var CustomerList = await unitOfWork.Customer.GetByIdAsync(CustomerId);
            if (CustomerList != null)
            {
                return Json(new
                {
                    primarycustomer = CustomerList.PrimaryCompany,
                    proceed = true,
                    msg = ""
                });
            }
            else
            {
                return Json(new
                {
                    primarycustomer = "",
                    proceed = true,
                    msg = ""
                });
            }

        }

        public async Task<IActionResult> LoadAll(PDAEstimator pDAEstimator)
        {
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("CustID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            List<PDAEstimatorList> pDAEstimatorLists = new List<PDAEstimatorList>();
            var custID = HttpContext.Session.GetString("CustID");

            int customerID = string.IsNullOrEmpty(custID) ? 0 : Convert.ToInt32(custID);
            pDAEstimatorLists = await unitOfWork.PDAEstimitor.GetAlllistByCustIdAsync(customerID);

            if (pDAEstimatorLists.Count() > 0)
            {
                if (pDAEstimator.CustomerID != null && pDAEstimator.CustomerID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.CustomerID == pDAEstimator.CustomerID).ToList();
                }
                if (pDAEstimator.PortID != null && pDAEstimator.PortID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.PortID == pDAEstimator.PortID).ToList();
                }
                if (pDAEstimator.TerminalID != null && pDAEstimator.TerminalID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.TerminalID == pDAEstimator.TerminalID).ToList();
                }
                if (pDAEstimator.CallTypeID != null && pDAEstimator.CallTypeID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.CallTypeID == pDAEstimator.CallTypeID).ToList();
                }
                if (pDAEstimator.CreatedBy != null && pDAEstimator.CreatedBy != "")
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => Convert.ToInt64(x.UserId) == Convert.ToInt64(pDAEstimator.CreatedBy)).ToList();

                }
            }
            return PartialView("partial/_ViewAll", pDAEstimatorLists);
        }

        public async Task<IActionResult> LoadAllReport(PDAEstimator pDAEstimator)
        {

            List<PDAEstimatorList> pDAEstimatorLists = new List<PDAEstimatorList>();
            var userid = HttpContext.Session.GetString("UserID");

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END

            var userdata = await unitOfWork.User.GetAllUsersById(Convert.ToInt64(userid));
            var userwithRole = await unitOfWork.User.GetByIdAsync(Convert.ToInt64(userid));
            if (userwithRole.RoleName == "Admin")
            {
                pDAEstimatorLists = await unitOfWork.PDAEstimitor.GetAlllistAsync();
            }
            else
            {
                if (userdata.Ports != null && userdata.Ports != "")
                {
                    List<int> PortIds = userdata.Ports.Split(',').Select(int.Parse).ToList();
                    pDAEstimatorLists = await unitOfWork.PDAEstimitor.GetAlllistAsync();
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => PortIds.Contains(x.PortID)).ToList();

                }
            }

            if (pDAEstimatorLists.Count() > 0)
            {
                if (pDAEstimator.CustomerID != null && pDAEstimator.CustomerID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.CustomerID == pDAEstimator.CustomerID && x.PortID == pDAEstimator.PortID).ToList();
                }
                if (pDAEstimator.PortID != null && pDAEstimator.PortID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.PortID == pDAEstimator.PortID).ToList();
                }
                if (pDAEstimator.TerminalID != null && pDAEstimator.TerminalID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.TerminalID == pDAEstimator.TerminalID).ToList();
                }
                if (pDAEstimator.CallTypeID != null && pDAEstimator.CallTypeID != 0)
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => x.CallTypeID == pDAEstimator.CallTypeID).ToList();
                }
                if (pDAEstimator.CreatedBy != null && pDAEstimator.CreatedBy != "")
                {
                    pDAEstimatorLists = pDAEstimatorLists.Where(x => Convert.ToInt64(x.UserId) == Convert.ToInt64(pDAEstimator.CreatedBy)).ToList();

                }
            }
            return PartialView("partial/_ReportList", pDAEstimatorLists);
        }


        public IActionResult PortNameOnchange(PDAEstimator PDAEstimitor)
        {
            var TerminalDetailData = unitOfWork.TerminalDetails.GetAllAsync().Result.Where(x => x.PortID == PDAEstimitor.PortID);
            if (TerminalDetailData != null && TerminalDetailData.Count() > 0)
                TerminalDetailData = TerminalDetailData.Where(x => x.Status == true).ToList();
            ViewBag.Terminal = TerminalDetailData;
            return PartialView("partial/TerminalList");
        }
        public IActionResult TeaminalLoad(int selectedCargoId, int selectedPortId)
        {

            var TerminalDetailData = unitOfWork.PDAEstimitor.GetTerminalByCargoIdAndPortAsync(selectedCargoId, selectedPortId).Result;
            if (TerminalDetailData != null && TerminalDetailData.Count() > 0)
                TerminalDetailData = TerminalDetailData.Where(x => x.Status == true).ToList();
            ViewBag.Terminal = TerminalDetailData;
            return PartialView("partial/TerminalList");
        }

        public IActionResult TerminalNameOnchange(PDAEstimator PDAEstimitor)
        {
            var BearthDetailData = unitOfWork.BerthDetails.GetAllAsync().Result.Where(x => x.TerminalID == PDAEstimitor.TerminalID);
            if (BearthDetailData != null && BearthDetailData.Count() > 0)
                BearthDetailData = BearthDetailData.Where(x => x.BerthStatus == true).ToList();
            ViewBag.Berth = BearthDetailData;
            return PartialView("partial/BerthList");
        }

        public IActionResult BerthNameOnchange(PDAEstimator PDAEstimitor)
        {
            if (PDAEstimitor.BerthId != 0)
            {
                var BearthDetailData = unitOfWork.BerthDetails.GetByIdAsync(PDAEstimitor.BerthId).Result;
                return Json(new
                {
                    loa = BearthDetailData.MaxLoa,
                    beam = BearthDetailData.MaxBeam,
                    arrivalDraft = BearthDetailData.MaxArrivalDraft,
                    dwt = BearthDetailData.DWT,
                    proceed = true,
                    msg = ""
                });
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        [Obsolete]
        public async Task<ActionResult> PDAEstimitorSave(PDAEstimator PDAEstimitor)
        {
            var CustID = HttpContext.Session.GetString("CustID");


            if (!string.IsNullOrEmpty(CustID))
            {
                PDAEstimitor.CustomerID = Convert.ToInt32(CustID);
                var internalCmpnayId = unitOfWork.PDAEstimitor.GetbyCustIdasync(Convert.ToInt32(CustID));
                PDAEstimitor.InternalCompanyID = internalCmpnayId.Result.CompanyID;
                var BearthDetailData = unitOfWork.BerthDetails.GetAllAsync().Result.Where(x => x.ID == PDAEstimitor.BerthId);

                var maxLoa = BearthDetailData.FirstOrDefault().MaxLoa;
                var maxBeam = BearthDetailData.FirstOrDefault().MaxBeam;
                var maxArribvaldraft = BearthDetailData.FirstOrDefault().MaxArrivalDraft;

                CultureInfo provider = CultureInfo.InvariantCulture;
                string ETA_String = PDAEstimitor.ETA_String + " " + "12:00:00 AM";
                DateTime Validity_To = DateTime.ParseExact(ETA_String, new string[] { "dd.M.yyyy hh:mm:ss tt", "dd-M-yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt" }, provider, DateTimeStyles.None);

                PDAEstimitor.ETA = Validity_To;

                decimal berthStayHrs = PDAEstimitor.LoadDischargeRate != 0 ? Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(PDAEstimitor.CargoQty) / Convert.ToDecimal(PDAEstimitor.LoadDischargeRate)) * 24) + 4 : 0;
                decimal berthStayDay = PDAEstimitor.LoadDischargeRate != 0 ? Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(PDAEstimitor.CargoQty) / Convert.ToDecimal(PDAEstimitor.LoadDischargeRate))) : 0;
                decimal berthStayShift = PDAEstimitor.LoadDischargeRate != 0 ? Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(PDAEstimitor.CargoQty) / Convert.ToDecimal(PDAEstimitor.LoadDischargeRate) * 3)) : 0;

                var calltype = unitOfWork.CallTypes.GetByIdAsync(PDAEstimitor.CallTypeID).Result;

                if (calltype.CallTypeName.ToUpper() == "FOREIGN")
                {
                    PDAEstimitor.BerthStay = Convert.ToInt64(berthStayHrs);
                    PDAEstimitor.BerthStayDay = Convert.ToInt64(berthStayDay);
                    PDAEstimitor.BerthStayShift = Convert.ToInt64(berthStayShift);
                }
                else if (calltype.CallTypeName.ToUpper() == "COASTAL IN FOREIGN OUT" || calltype.CallTypeName.ToUpper() == "FOREIGN IN COASTAL OUT")
                {
                    PDAEstimitor.BerthStayHoursCoastal = Convert.ToInt64(berthStayHrs);
                    PDAEstimitor.BerthStayDayCoastal = Convert.ToInt64(berthStayDay);
                    PDAEstimitor.BerthStayShiftCoastal = Convert.ToInt64(berthStayShift);

                    PDAEstimitor.BerthStay = Convert.ToInt64(6);
                    PDAEstimitor.BerthStayDay = Convert.ToInt64(1);
                    PDAEstimitor.BerthStayShift = Convert.ToInt64(1);
                }
                else if (calltype.CallTypeName.ToUpper() == "COASTAL")
                {
                    PDAEstimitor.BerthStayHoursCoastal = Convert.ToInt64(berthStayHrs);
                    PDAEstimitor.BerthStayDayCoastal = Convert.ToInt64(berthStayDay);
                    PDAEstimitor.BerthStayShiftCoastal = Convert.ToInt64(berthStayShift);
                }
                else
                {
                    PDAEstimitor.BerthStay = Convert.ToInt64(berthStayHrs);
                    PDAEstimitor.BerthStayDay = Convert.ToInt64(berthStayDay);
                    PDAEstimitor.BerthStayShift = Convert.ToInt64(berthStayShift);
                }
                PDAEstimitor.IsCustomerCreated = true;


                if (maxLoa != null && maxLoa < PDAEstimitor.LOA)
                {
                    _toastNotification.AddErrorToastMessage("Please enter MaxLOA Less then : " + maxLoa);
                    return Json(new
                    {
                        proceed = false,
                        msg = ""
                    });
                }

                if (maxBeam != null && maxBeam < PDAEstimitor.Beam)
                {
                    _toastNotification.AddErrorToastMessage("Please enter MaxBeam Less then : " + maxBeam);
                    return Json(new
                    {
                        proceed = false,
                        msg = ""
                    });
                }

                if (maxArribvaldraft != null && maxArribvaldraft < PDAEstimitor.ArrivalDraft)
                {
                    _toastNotification.AddErrorToastMessage("Please enter Max Arrival Draft Less then : " + maxArribvaldraft);
                    return Json(new
                    {
                        proceed = false,
                        msg = ""
                    });
                }
                if (PDAEstimitor.PDAEstimatorID > 0)
                {
                    var userid = HttpContext.Session.GetString("CustID");
                    PDAEstimitor.ModifyUserID = userid;
                    await unitOfWork.PDAEstimitor.UpdateAsync(PDAEstimitor);
                    _toastNotification.AddSuccessToastMessage("Updated Successfully");
                }
                else
                {
                    var userid = HttpContext.Session.GetString("CustID");
                    PDAEstimitor.CreatedBy = userid;

                    var id = await unitOfWork.PDAEstimitor.AddAsync(PDAEstimitor);
                    if (id != "" && Convert.ToInt64(id) > 0)
                    {
                        PDAEstimatorOutPut pDAEstimatorOutPut = new PDAEstimatorOutPut();
                        //var PDAData = unitOfWork.PDAEstimitor.GetAlllistAsync().Result.Where(x => x.PDAEstimatorID == Convert.ToInt64(id)).FirstOrDefault();

                        var CompanyData = unitOfWork.Company.GetAlllistAsync().Result.Where(x => x.CompanyId == PDAEstimitor.InternalCompanyID).FirstOrDefault();
                        var TaxData = unitOfWork.Taxs.GetAllAsync().Result;

                        string Addressline2 = "";
                        pDAEstimatorOutPut.PDAEstimatorID = Convert.ToInt64(id);
                        if (CompanyData != null)
                        {
                            if (CompanyData.Address2 != null)
                            {
                                Addressline2 = CompanyData.Address1.ToUpper() + ", " + CompanyData.Address2.ToUpper() + ", ";
                            }
                            else
                            {
                                Addressline2 = CompanyData.Address1.ToUpper() + ", ";
                            }

                            pDAEstimatorOutPut.CompanyName = CompanyData.CompanyName;
                            pDAEstimatorOutPut.CompanyAddress1 = Addressline2;
                            pDAEstimatorOutPut.CompanyAddress2 = CompanyData.CityName.ToUpper() + ", " + CompanyData.StateName.ToUpper() + ", " + CompanyData.CountryName.ToUpper();
                            pDAEstimatorOutPut.CompanyTelephone = CompanyData.Telephone;
                            if (CompanyData.AlterTel.Split("-")[1] != "")
                            {
                                pDAEstimatorOutPut.CompanyAlterTel = CompanyData.AlterTel;
                            }
                            else
                            {
                                pDAEstimatorOutPut.CompanyAlterTel = "";
                            }
                            pDAEstimatorOutPut.CompanyEmail = CompanyData.Email.ToUpper();
                            pDAEstimatorOutPut.CompanyLogo = CompanyData.CompanyLog;
                        }
                        else
                        {
                            pDAEstimatorOutPut.CompanyAddress1 = "";
                            pDAEstimatorOutPut.CompanyAddress2 = "";
                            pDAEstimatorOutPut.CompanyTelephone = "";
                            pDAEstimatorOutPut.CompanyAlterTel = "";
                            pDAEstimatorOutPut.CompanyEmail = "";
                            pDAEstimatorOutPut.CompanyLogo = "";
                        }

                        //var custdata = unitOfWork.Customer.GetByIdAsync(PDAEstimitor.CustomerID).Result;
                        if (PDAEstimitor.InternalCompanyID != null)
                        {
                            var BankMaster = unitOfWork.BankMaster.GetByCompanyIdAsync((int)PDAEstimitor.InternalCompanyID).Result;
                            if (BankMaster != null)
                            {
                                pDAEstimatorOutPut.NameofBeneficiary = BankMaster.NameofBeneficiary;
                                pDAEstimatorOutPut.BeneficiaryAddress = BankMaster.BeneficiaryAddress;
                                pDAEstimatorOutPut.AccountNo = BankMaster.AccountNo;
                                pDAEstimatorOutPut.Beneficiary_Bank_Name = BankMaster.Beneficiary_Bank_Name;
                                pDAEstimatorOutPut.Beneficiary_Bank_Address = BankMaster.Beneficiary_Bank_Address;
                                pDAEstimatorOutPut.Beneficiary_RTGS_NEFT_IFSC_Code = BankMaster.Beneficiary_RTGS_NEFT_IFSC_Code;
                                pDAEstimatorOutPut.Beneficiary_Bank_Swift_Code = BankMaster.Beneficiary_Bank_Swift_Code;
                                pDAEstimatorOutPut.Intermediary_Bank = BankMaster.Intermediary_Bank;
                                pDAEstimatorOutPut.Intermediary_Bank_Swift_Code = BankMaster.Intermediary_Bank_Swift_Code;
                            }
                        }

                        var currencyData = unitOfWork.Currencys.GetAllAsync().Result;
                        pDAEstimatorOutPut.BaseCurrencyCode = currencyData.Where(x => x.BaseCurrency == true) != null ? currencyData.Where(x => x.BaseCurrency == true).FirstOrDefault().CurrencyCode : "";
                        pDAEstimatorOutPut.DefaultCurrencyCode = currencyData.Where(x => x.DefaultCurrecny == true) != null ? currencyData.Where(x => x.DefaultCurrecny == true).FirstOrDefault().CurrencyCode : "";

                        pDAEstimatorOutPut.BaseCurrencyCodeID = currencyData.Where(x => x.BaseCurrency == true) != null ? currencyData.Where(x => x.BaseCurrency == true).FirstOrDefault().ID : 0;
                        pDAEstimatorOutPut.DefaultCurrencyCodeID = currencyData.Where(x => x.DefaultCurrecny == true) != null ? currencyData.Where(x => x.DefaultCurrecny == true).FirstOrDefault().ID : 0;

                        //var taxrate = TaxData.Where(x => x.TaxName.Contains("GST")).Select(x => x.TaxRate).FirstOrDefault();
                        //pDAEstimatorOutPut.Taxrate = taxrate;
                        var Disclaimersdata = await unitOfWork.Disclaimers.GetAllAsync();

                        if (Disclaimersdata != null && Disclaimersdata.Count() > 0)
                        {
                            var ActiveDisclaimersdata = Disclaimersdata.Where(x => x.IsActive == true);
                            if (ActiveDisclaimersdata != null && ActiveDisclaimersdata.Count() > 0)
                            {
                                pDAEstimatorOutPut.Disclaimer = ActiveDisclaimersdata.FirstOrDefault().Disclaimer;
                            }

                        }

                        pDAEstimatorOutPut.PDAEstimatorOutPutDate = DateTime.Now;
                        var PDAEstimitorOUTPUTid = await unitOfWork.PDAEstimitorOUTPUT.AddAsync(pDAEstimatorOutPut);

                        if (PDAEstimitorOUTPUTid != "" && Convert.ToInt64(PDAEstimitorOUTPUTid) > 0)
                        {
                            var NotesData = await unitOfWork.PDAEstimitor.GetNotes();
                            if (NotesData != null)
                            {
                                foreach (var note in NotesData)
                                {
                                    PDAEstimatorOutPutNote pDAEstimatorOutPutNote = new PDAEstimatorOutPutNote();
                                    pDAEstimatorOutPutNote.PDAEstimatorOutPutID = Convert.ToInt64(PDAEstimitorOUTPUTid);
                                    pDAEstimatorOutPutNote.Note = note.Note;
                                    pDAEstimatorOutPutNote.sequnce = note.sequnce;
                                    await unitOfWork.PDAEstimitorOUTNote.AddAsync(pDAEstimatorOutPutNote);
                                }
                            }

                            List<PDAEstimatorOutPutTariff> pDAEstimatorOutPutTariffs = new List<PDAEstimatorOutPutTariff>();
                            var triffdata = unitOfWork.PDAEstimitor.GetAllPDA_Tariff(PDAEstimitor.PortID, PDAEstimitor.ETA != null ? (DateTime)PDAEstimitor.ETA : DateTime.Now.Date).Result.Where(x => (x.CallTypeID == PDAEstimitor.CallTypeID || x.CallTypeID == null) && (x.TerminalID == PDAEstimitor.TerminalID || x.TerminalID == null) && (x.BerthID == PDAEstimitor.BerthId || x.BerthID == null || x.BerthID == 0) && (x.CargoID == PDAEstimitor.CargoID || x.CargoID == null) && (x.OperationTypeID == PDAEstimitor.ActivityTypeId || x.OperationTypeID == null) && (x.VesselBallast == PDAEstimitor.VesselBallast || x.VesselBallast == 0) && (x.Reduced_GRT == PDAEstimitor.IsReducedGRT || x.Reduced_GRT == 0)).OrderBy(o => o.ChargeCodeSequence).ThenBy(o => o.SlabFrom).ThenBy(o => o.TariffRateID);
                            List<PDATariffRateList> pDATariffRateList = new List<PDATariffRateList>();
                            decimal taxrate = 0;
                            foreach (var triff in triffdata)
                            {
                                long? slabattributvalue = 0;
                                if (triff.FormulaID != null && triff.FormulaID > 0)
                                {
                                    string formulastring = string.Empty;
                                    var formulatransdata = await unitOfWork.FormulaTransaction.GetAllTransAsync((int)triff.FormulaID);
                                    if (formulatransdata.Count > 0)
                                    {

                                        if (triff.SlabName == "GRT")
                                            slabattributvalue = PDAEstimitor.GRT;
                                        else if (triff.SlabName == "RGRT")
                                            slabattributvalue = PDAEstimitor.RGRT;
                                        else if (triff.SlabName == "NRT")
                                            slabattributvalue = PDAEstimitor.NRT;
                                        else if (triff.SlabName == "BSTH" || triff.SlabName == "BSTHF")
                                            slabattributvalue = PDAEstimitor.BerthStay;
                                        else if (triff.SlabName == "BSTS" || triff.SlabName == "BSTSF")
                                            slabattributvalue = PDAEstimitor.BerthStayShift;
                                        else if (triff.SlabName == "BSTD" || triff.SlabName == "BSTDF")
                                            slabattributvalue = PDAEstimitor.BerthStayDay;
                                        else if (triff.SlabName == "BSTHC")
                                            slabattributvalue = PDAEstimitor.BerthStayHoursCoastal;
                                        else if (triff.SlabName == "BSTSC")
                                            slabattributvalue = PDAEstimitor.BerthStayShiftCoastal;
                                        else if (triff.SlabName == "BSTDC")
                                            slabattributvalue = PDAEstimitor.BerthStayDayCoastal;
                                        else if (triff.SlabName == "AST")
                                            slabattributvalue = PDAEstimitor.AnchorageStay;
                                        else if (triff.SlabName == "QTYMT")
                                            slabattributvalue = PDAEstimitor.CargoQty;
                                        else if (triff.SlabName == "QTYCBM")
                                            slabattributvalue = PDAEstimitor.CargoQtyCBM;
                                        bool Range_Tariff = triff.Range_TariffID > 0 ? true : false;
                                        UnitCalculation(triff, PDAEstimitor.GRT, (long)slabattributvalue, Range_Tariff);
                                        foreach (var formularTransList in formulatransdata)
                                        {
                                            if (formularTransList.formulaAttributeID > 0)
                                            {
                                                string FormulaAttributedata = formularTransList.formulaAttributeName;
                                                if (FormulaAttributedata.Contains("GRT"))
                                                {
                                                    if (triff.SlabID == null || triff.SlabID == 0)
                                                    {
                                                        triff.UNITS = PDAEstimitor.GRT;
                                                    }

                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.GRT.ToString() : PDAEstimitor.GRT.ToString();
                                                    }

                                                    //UnitCalculation(triff, pDAEstimatorOutPut.GRT, pDAEstimatorOutPut);
                                                }
                                                else if (FormulaAttributedata.Contains("RGRT"))
                                                {
                                                    if (triff.SlabID == null || triff.SlabID == 0)
                                                    {
                                                        triff.UNITS = PDAEstimitor.RGRT;
                                                    }

                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.RGRT.ToString() : PDAEstimitor.RGRT.ToString();
                                                    }

                                                    //UnitCalculation(triff, pDAEstimatorOutPut.GRT, pDAEstimatorOutPut);
                                                }
                                                else if (FormulaAttributedata.Contains("NRT"))
                                                {
                                                    if (triff.SlabID == null || triff.SlabID == 0)
                                                    {
                                                        triff.UNITS = PDAEstimitor.NRT;
                                                    }
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.NRT, pDAEstimatorOutPut);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.NRT.ToString() : PDAEstimitor.NRT.ToString();
                                                    }
                                                }
                                                else if (FormulaAttributedata == "BSTH" || FormulaAttributedata == "BSTHF")
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.BerthStay);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.BerthStay.ToString() : PDAEstimitor.BerthStay.ToString();
                                                    }
                                                }
                                                else if (FormulaAttributedata == "BSTS" || FormulaAttributedata == "BSTSF")
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayShift);
                                                    formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.BerthStayShift.ToString() : PDAEstimitor.BerthStayShift.ToString();
                                                }
                                                else if (FormulaAttributedata == "BSTD" || FormulaAttributedata == "BSTDF")
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayDay);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.BerthStayDay.ToString() : PDAEstimitor.BerthStayDay.ToString();
                                                    }
                                                }
                                                else if (FormulaAttributedata.Contains("BSTHC"))
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayHoursCoastal);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.BerthStayHoursCoastal.ToString() : PDAEstimitor.BerthStayHoursCoastal.ToString();
                                                    }
                                                }
                                                else if (FormulaAttributedata.Contains("BSTSC"))
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayShiftCoastal);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.BerthStayShiftCoastal.ToString() : PDAEstimitor.BerthStayShiftCoastal.ToString();
                                                    }
                                                }
                                                else if (FormulaAttributedata.Contains("BSTDC"))
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayDayCoastal);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.BerthStayDayCoastal.ToString() : PDAEstimitor.BerthStayDayCoastal.ToString();
                                                    }
                                                }
                                                else if (FormulaAttributedata.Contains("AST"))
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.AnchorageStay);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.AnchorageStay.ToString() : PDAEstimitor.AnchorageStay.ToString();
                                                    }
                                                }
                                                else if (FormulaAttributedata.Contains("QTYMT"))
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.CargoQty);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.CargoQty.ToString() : PDAEstimitor.CargoQty.ToString();
                                                    }
                                                }
                                                else if (FormulaAttributedata.Contains("QTYCBM"))
                                                {
                                                    //UnitCalculation(triff, pDAEstimatorOutPut.CargoQty);
                                                    if (triff.SlabID != null && triff.SlabID > 0 && FormulaAttributedata == triff.SlabName)
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                                    }
                                                    else
                                                    {
                                                        formulastring = formulastring != "" ? formulastring + " " + PDAEstimitor.CargoQtyCBM.ToString() : PDAEstimitor.CargoQtyCBM.ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    if (triff.UNITS == null || triff.UNITS == 0)
                                                    {
                                                        triff.UNITS = 1;
                                                    }
                                                    formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaAttributeName : formularTransList.formulaAttributeName;
                                                }
                                            }
                                            if (formularTransList.formulaSlabID > 0)
                                                formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaSlabName : formularTransList.formulaSlabName;
                                            if (formularTransList.formulaOperatorID > 0)
                                                formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaOperatorName : formularTransList.formulaOperatorName;
                                            if (formularTransList.formulaValue > 0)
                                                formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaValue : formularTransList.formulaValue.ToString();
                                        }
                                        //if (triff.UNITS == null || triff.UNITS == 0)
                                        //{
                                        //    triff.UNITS = 1;
                                        //}
                                    }
                                    else
                                    {
                                        triff.UNITS = 1;

                                    }
                                    triff.Formula = formulastring;
                                    triff.FormulaID = (int)triff.FormulaID;
                                }
                                else
                                {
                                    triff.UNITS = 1;
                                }

                                if (triff.Formula != "")
                                {
                                    DataTable dt = new DataTable();
                                    var amount = new object();
                                    try
                                    {
                                        amount = dt.Compute(triff.Formula, "");
                                    }
                                    catch (Exception ex)
                                    {
                                        continue;
                                    }
                                    decimal amt = Convert.ToDecimal(amount);
                                    amt = Math.Abs(amt);
                                    amt = amt * triff.Rate;
                                    if (triff.Range_TariffID > 0)
                                    {
                                        var data = await unitOfWork.TariffRates.GetByIdAsync(triff.Range_TariffID);
                                        var formulatransdata = await unitOfWork.FormulaTransaction.GetAllTransAsync((int)data.FormulaID);
                                        string formulastringref = "";
                                        foreach (var formularTransList in formulatransdata)
                                        {
                                            if (formularTransList.formulaAttributeID > 0)
                                            {
                                                string FormulaAttributedata = formularTransList.formulaAttributeName;
                                                if (FormulaAttributedata.Contains("GRT"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.GRT.ToString() : PDAEstimitor.GRT.ToString();

                                                }
                                                else if (FormulaAttributedata.Contains("RGRT"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.RGRT.ToString() : PDAEstimitor.RGRT.ToString();
                                                }
                                                else if (FormulaAttributedata.Contains("NRT"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.NRT.ToString() : PDAEstimitor.NRT.ToString();

                                                }
                                                else if (FormulaAttributedata == "BSTH" || FormulaAttributedata == "BSTHF")
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.BerthStay.ToString() : PDAEstimitor.BerthStay.ToString();

                                                }
                                                else if (FormulaAttributedata == "BSTS" || FormulaAttributedata == "BSTSF")
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.BerthStayShift.ToString() : PDAEstimitor.BerthStayShift.ToString();
                                                }
                                                else if (FormulaAttributedata == "BSTD" || FormulaAttributedata == "BSTDF")
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.BerthStayDay.ToString() : PDAEstimitor.BerthStayDay.ToString();
                                                }
                                                else if (FormulaAttributedata.Contains("BSTHC"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.BerthStayHoursCoastal.ToString() : PDAEstimitor.BerthStayHoursCoastal.ToString();
                                                }
                                                else if (FormulaAttributedata.Contains("BSTSC"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.BerthStayShiftCoastal.ToString() : PDAEstimitor.BerthStayShiftCoastal.ToString();
                                                }
                                                else if (FormulaAttributedata.Contains("BSTDC"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.BerthStayDayCoastal.ToString() : PDAEstimitor.BerthStayDayCoastal.ToString();
                                                }
                                                else if (FormulaAttributedata.Contains("AST"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.AnchorageStay.ToString() : PDAEstimitor.AnchorageStay.ToString();
                                                }
                                                else if (FormulaAttributedata.Contains("QTYMT"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.CargoQty.ToString() : PDAEstimitor.CargoQty.ToString();
                                                }
                                                else if (FormulaAttributedata.Contains("QTYCBM"))
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + PDAEstimitor.CargoQtyCBM.ToString() : PDAEstimitor.CargoQty.ToString();
                                                }
                                                else
                                                {
                                                    formulastringref = formulastringref != "" ? formulastringref + " " + formularTransList.formulaAttributeName : formularTransList.formulaAttributeName;
                                                }
                                            }
                                            if (formularTransList.formulaSlabID > 0)
                                                formulastringref = formulastringref != "" ? formulastringref + " " + formularTransList.formulaSlabName : formularTransList.formulaSlabName;
                                            if (formularTransList.formulaOperatorID > 0)
                                                formulastringref = formulastringref != "" ? formulastringref + " " + formularTransList.formulaOperatorName : formularTransList.formulaOperatorName;
                                            if (formularTransList.formulaValue > 0)
                                                formulastringref = formulastringref != "" ? formulastringref + " " + formularTransList.formulaValue : formularTransList.formulaValue.ToString();
                                        }

                                        var amountref = dt.Compute(formulastringref, "");
                                        decimal amtref = Convert.ToDecimal(amountref);
                                        amtref = amtref * data.Rate;
                                        amt = amt + amtref;
                                    }
                                    triff.Amount = amt;
                                }
                                else
                                {
                                    triff.Amount = 0;
                                }

                                if (triff.CurrencyID == pDAEstimatorOutPut.BaseCurrencyCodeID)
                                {
                                    if (triff.TaxID != null && triff.TaxID != 0)
                                    {
                                        var tax = TaxData.Where(x => x.ID == triff.TaxID).Select(x => x.TaxRate).FirstOrDefault();
                                        triff.GSTBase = (triff.Amount * tax) / 100;
                                        taxrate = tax;
                                    }
                                    else
                                    {
                                        triff.GSTBase = 0;
                                    }
                                }
                                if (triff.CurrencyID == pDAEstimatorOutPut.DefaultCurrencyCodeID)
                                {
                                    if (triff.TaxID != null && triff.TaxID != 0)
                                    {
                                        var tax = TaxData.Where(x => x.ID == triff.TaxID).Select(x => x.TaxRate).FirstOrDefault();
                                        triff.GSTDefault = (triff.Amount * tax) / 100;
                                        taxrate = tax;
                                    }
                                    else
                                    {
                                        triff.GSTDefault = 0;
                                    }
                                }

                                if (triff.NonIncreemental)
                                {
                                    PDAEstimatorOutPutTariff pDAEstimatorOutPutTariff = new PDAEstimatorOutPutTariff();
                                    if (triff.Range_TariffID > 0)
                                    {
                                        pDAEstimatorOutPutTariff.UNITS = slabattributvalue != null ? Convert.ToDecimal(slabattributvalue) : 0;
                                    }
                                    else
                                    {
                                        pDAEstimatorOutPutTariff.UNITS = triff.UNITS != null ? (decimal)triff.UNITS : 0;
                                    }
                                    pDAEstimatorOutPutTariff.Amount = triff.Amount;
                                    pDAEstimatorOutPutTariff.PDAEstimatorOutPutID = Convert.ToInt64(PDAEstimitorOUTPUTid);
                                    pDAEstimatorOutPutTariff.ExpenseCategoryID = triff.ExpenseCategoryID;
                                    pDAEstimatorOutPutTariff.Remark = triff.Remark;
                                    pDAEstimatorOutPutTariff.Rate = triff.Rate;
                                    pDAEstimatorOutPutTariff.Taxrate = taxrate;
                                    pDAEstimatorOutPutTariff.ChargeCodeName = triff.ChargeCodeName;
                                    pDAEstimatorOutPutTariff.CurrencyID = triff.CurrencyID;
                                    pDAEstimatorOutPutTariff.TaxID = triff.TaxID;
                                    await unitOfWork.PDAEstimatorOutPutTariff.AddAsync(pDAEstimatorOutPutTariff);

                                    pDATariffRateList.Add(triff);
                                }
                            }

                        }
                        _toastNotification.AddSuccessToastMessage("Submitted successfully");

                        //PDAEstimatorOutPutView pDAEstimatorOutPutview = new PDAEstimatorOutPutView();
                        //pDAEstimatorOutPutview = await GetPDA(Convert.ToInt32(id));

                        //string PDAfilename = "PDA_" + pDAEstimatorOutPutview.PortName + "_" + pDAEstimatorOutPutview.PDAEstimatorID + ".pdf";
                        //string path = $"{_hostEnvironment.WebRootPath}\\PDAFile\\";
                        //var rootpath = Path.Combine(path, PDAfilename);
                        //rootpath = Path.GetFullPath(rootpath);
                        //return new ViewAsPdf("PDAEstimator", pDAEstimatorOutPutview)
                        //{
                        //    //FileName = "MyPdf.pdf";
                        //    FileName = PDAfilename,
                        //    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        //    PageSize = Rotativa.AspNetCore.Options.Size.A3,
                        //    SaveOnServerPath = rootpath
                        //};

                        return Json(new
                        {
                            PDAID = Convert.ToInt32(id),
                            proceed = true,
                            msg = ""
                        });
                    }


                    //return RedirectToAction("PDAEstimator", "PDAEstimator", id);
                }
                return Json(new
                {
                    PDAID = 0,
                    proceed = true,
                    msg = ""
                });
            }
            else
            {
                return RedirectToAction("index", "Login");
            }
        }

        public async Task<ActionResult> editPDAEstimator(PDAEstimator PDAEstimitor)
        {
            try
            {
                var data = await unitOfWork.PDAEstimitor.GetByIdAsync(PDAEstimitor.PDAEstimatorID);

                return Json(new
                {
                    PDAEstimitor = data,
                    proceed = true,
                    msg = ""
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> deletePDAEstimitor(PDAEstimator PDAEstimitor)
        {
            var data = await unitOfWork.PDAEstimitor.DeleteAsync(PDAEstimitor.PDAEstimatorID);
            _toastNotification.AddSuccessToastMessage("Deleted Successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> PdaEstimatorRedirect(int id)
        {
            // read parameters from the webpage
            string htmlString = "<html>\r\n <body>\r\n  Hello World from selectpdf.com.\r\n </body>\r\n</html>\r\n";
            string baseUrl = "";

            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            int webPageWidth = 1024;
            try
            {
                webPageWidth = Convert.ToInt32(1024);
            }
            catch { }

            int webPageHeight = 0;
            try
            {
                webPageHeight = Convert.ToInt32(1024);
            }
            catch { }

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;

            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

            // save pdf document
            doc.Save("Sample.pdf");

            // close pdf document
            doc.Close();
            var CustomerList = await unitOfWork.PDAEstimitor.GetByIdAsync(id);
            return PartialView("PDAEstimatorOutput", CustomerList);

        }

        protected void BtnCreatePdf_Click(object sender, EventArgs e)
        {

        }
    }
}
