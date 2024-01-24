using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using PDA_Web.Models;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using SelectPdf;
using System.Data;
using System.Globalization;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PDAEstimatorController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public PDAEstimatorController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;

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

        public async Task<IActionResult> PDAEstimator(int id)
        {
            PDAEstimatorOutPutView pDAEstimatorOutPut = new PDAEstimatorOutPutView();
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
                    CargoUnitofMasurement = PDAData.CargoUnitofMasurement,
                    LoadDischargeRate = PDAData.LoadDischargeRate,
                    CargoName = PDAData.CargoName,
                    CurrencyID = PDAData.CurrencyID,
                    ROE = PDAData.ROE,
                    DWT = PDAData.DWT,
                    ArrivalDraft = PDAData.ArrivalDraft,
                    GRT = PDAData.GRT,
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
                    VesselBallast = PDAData.VesselBallast
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
                pDAEstimatorOutPut.CompanyAlterTel = CompanyData.AlterTel;
                pDAEstimatorOutPut.CompanyEmail = CompanyData.Email.ToUpper();
                pDAEstimatorOutPut.CompanyLogo = CompanyData.CompanyLog;
                var custdata = unitOfWork.Customer.GetByIdAsync(PDAData.CustomerID).Result;
                if (custdata != null && custdata.BankID != null)
                {
                    pDAEstimatorOutPut.BankMaster = unitOfWork.BankMaster.GetByIdAsync(custdata.BankID).Result;
                }

                pDAEstimatorOutPut.Expenses = unitOfWork.Expenses.GetAllAsync().Result.OrderBy(x => x.sequnce).ToList();

                pDAEstimatorOutPut.ChargeCodes = unitOfWork.ChargeCodes.GetAlllistAsync().Result.OrderBy(x=> x.Sequence).ToList();

                pDAEstimatorOutPut.NotesData = unitOfWork.PDAEstimitor.GetNotes().Result.ToList();
                var currencyData = unitOfWork.Currencys.GetAllAsync().Result;
                pDAEstimatorOutPut.BaseCurrencyCode = currencyData.Where(x => x.BaseCurrency == true) != null ? currencyData.Where(x => x.BaseCurrency == true).FirstOrDefault().CurrencyCode : "";
                pDAEstimatorOutPut.DefaultCurrencyCode = currencyData.Where(x => x.DefaultCurrecny == true) != null ? currencyData.Where(x => x.DefaultCurrecny == true).FirstOrDefault().CurrencyCode : "";
                pDAEstimatorOutPut.BaseCurrencyCodeID = currencyData.Where(x => x.BaseCurrency == true) != null ? currencyData.Where(x => x.BaseCurrency == true).FirstOrDefault().ID : 0;
                pDAEstimatorOutPut.DefaultCurrencyCodeID = currencyData.Where(x => x.DefaultCurrecny == true) != null ? currencyData.Where(x => x.DefaultCurrecny == true).FirstOrDefault().ID : 0;

                //var triffdata = unitOfWork.PDAEstimitor.GetAllPDA_Tariff(pDAEstimatorOutPut.PortID).Result.Where(x => (x.CallTypeID == pDAEstimatorOutPut.CallTypeID || x.CallTypeID == null) && (x.SlabFrom == null || x.SlabFrom <= pDAEstimatorOutPut.GRT)) ;
                var triffdata = unitOfWork.PDAEstimitor.GetAllPDA_Tariff(pDAEstimatorOutPut.PortID).Result.Where(x => (x.CallTypeID == pDAEstimatorOutPut.CallTypeID || x.CallTypeID == null) && (x.TerminalID == pDAEstimatorOutPut.TerminalID || x.TerminalID == null) && (x.CargoID == pDAEstimatorOutPut.CargoID || x.CargoID == null) && (x.VesselBallast == pDAEstimatorOutPut.VesselBallast || x.VesselBallast == 0)).OrderBy(o => o.SlabFrom).ThenBy(o => o.TariffRateID);
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
                            foreach (var formularTransList in formulatransdata)
                            {
                                if (formularTransList.formulaAttributeID > 0)
                                {
                                    string FormulaAttributedata = formularTransList.formulaAttributeName;
                                    if (FormulaAttributedata.Contains("GRT"))
                                    {
                                         UnitCalculation(triff, pDAEstimatorOutPut.GRT);
                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                    }
                                    else if (FormulaAttributedata.Contains("NRT"))
                                    {
                                        UnitCalculation(triff, pDAEstimatorOutPut.NRT);
                                        formulastring = formulastring != "" ? formulastring + " " + triff.UNITS.ToString() : triff.UNITS.ToString();
                                    }
                                    else if (FormulaAttributedata == "BSTH" || FormulaAttributedata == "BSTHF")
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStay);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStay.ToString() : pDAEstimatorOutPut.BerthStay.ToString();
                                    }
                                    else if (FormulaAttributedata == "BSTS" || FormulaAttributedata == "BSTSF")
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayShift);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayShift.ToString() : pDAEstimatorOutPut.BerthStayShift.ToString();
                                    }
                                    else if (FormulaAttributedata == "BSTD" || FormulaAttributedata == "BSTDF")
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayDay);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayDay.ToString() : pDAEstimatorOutPut.BerthStayDay.ToString();
                                    }
                                    else if (FormulaAttributedata.Contains("BSTHC"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayHoursCoastal);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayHoursCoastal.ToString() : pDAEstimatorOutPut.BerthStayHoursCoastal.ToString();
                                    }
                                    else if (FormulaAttributedata.Contains("BSTSC"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayShiftCoastal);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayShiftCoastal.ToString() : pDAEstimatorOutPut.BerthStayShiftCoastal.ToString();
                                    }
                                    else if (FormulaAttributedata.Contains("BSTDC"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.BerthStayDayCoastal);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.BerthStayDayCoastal.ToString() : pDAEstimatorOutPut.BerthStayDayCoastal.ToString();
                                    }
                                    else if (FormulaAttributedata.Contains("AST"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.AnchorageStay);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.AnchorageStay.ToString() : pDAEstimatorOutPut.AnchorageStay.ToString();
                                    }
                                    else if (FormulaAttributedata.Contains("CQTY"))
                                    {
                                        //UnitCalculation(triff, pDAEstimatorOutPut.CargoQty);
                                        formulastring = formulastring != "" ? formulastring + " " + pDAEstimatorOutPut.CargoQty.ToString() : pDAEstimatorOutPut.CargoQty.ToString();
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
                        var amount = dt.Compute(triff.Formula, "");
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


            //// Create the converter object
            //HtmlToPdf converter = new HtmlToPdf();

            //// Convert the HTML page from URL to memory
            //var htmlvaleu = View("PDAEstimator", pDAEstimatorOutPut);
            var viewHtml = await this.RenderViewAsync("PDAEstimator", pDAEstimatorOutPut);
            //using (MemoryStream stream = new System.IO.MemoryStream())
            //{
            //    StringReader sr = new StringReader(viewHtml);
            //    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
            //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
            //    pdfDoc.Open();
            //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
            //    pdfDoc.Close();
            //    return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            //}
            //byte[] pdfData = converter.ConvertUrlToMemory(viewHtml.ToString());

            ////// Save the PDF data to a file
            //System.IO.File.WriteAllBytes("output.pdf", pdfData);

            ////// Alternatively convert and save to a file in one step
            ////converter.ConvertUrlToFile(UrlToConvert, "output.pdf");

            ////// Send the PDF data for download in ASP.NET Core applications
            //FileResult fileResult = new FileContentResult(pdfData, "application/pdf");
            //fileResult.FileDownloadName = "Output.pdf";
            //return fileResult;

            //// Send the PDF data for download in ASP.NET Web Forms applications
            //HttpResponse httpResponse = HttpContext.Current.Response;
            //httpResponse.AddHeader("Content-Type", "application/pdf");
            //httpResponse.AddHeader("Content-Disposition",
            //           String.Format("attachment; filename=ConvertHtmlPart.pdf; size={0}",
            //           pdfData.Length.ToString()));
            //httpResponse.BinaryWrite(pdfData);
            //httpResponse.End();
            return View(pDAEstimatorOutPut);
        }

        public PDATariffRateList UnitCalculation(PDATariffRateList triff, long? attributvalue)
        {
            decimal? units = 0;
            if (triff.SlabID != null && triff.SlabID > 0)
            {
                if (triff.SlabIncreemental == 1)
                {
                    if (triff.SlabTo != 0 && attributvalue >= triff.SlabTo && triff.SlabFrom == 1)
                    {
                        units = triff.SlabTo;
                    }
                    else if (triff.SlabTo != 0 && attributvalue >= triff.SlabTo && triff.SlabFrom != 1)
                    {
                        units = (triff.SlabTo - triff.SlabFrom) + 1;
                        units = Math.Abs(Convert.ToDecimal(units));
                    }
                    else if(attributvalue >= triff.SlabFrom)
                    {
                        units = (attributvalue - triff.SlabFrom) + 1;
                        units = Math.Abs(Convert.ToDecimal(units));
                    }
                    triff.NonIncreemental = true;
                }
                else
                {
                    if (triff.SlabFrom <= attributvalue && (triff.SlabTo == 0 || triff.SlabTo >= attributvalue))
                    {
                        units = attributvalue;
                        units = Math.Abs(Convert.ToDecimal(units));
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
                units = attributvalue;
            }

            triff.UNITS = units;
            return triff;
        }

        public async Task<IActionResult> Index()
        {
            var userid = HttpContext.Session.GetString("UserID");
            if (!string.IsNullOrEmpty(userid))
            {
                var CustomerData = await unitOfWork.Customer.GetAllAsync();
                ViewBag.Customers = CustomerData;

                var CallTypeData = await unitOfWork.CallTypes.GetAllAsync();
                ViewBag.CallType = CallTypeData;

                var PortActivityData = await unitOfWork.PortActivities.GetAllAsync();
                ViewBag.PortActivity = PortActivityData;

                var PortCallData = await unitOfWork.PortDetails.GetAllAsync();
                ViewBag.Port = PortCallData;

                var TerminalData = await unitOfWork.TerminalDetails.GetAllAsync();
                ViewBag.Terminal = TerminalData;

                var data1 = await unitOfWork.BerthDetails.GetAllAsync();
                ViewBag.Berth = data1;

                var CargoType = await unitOfWork.CargoDetails.GetAllAsync();
                 ViewBag.Cargo = CargoType;

                var dataCurrency = await unitOfWork.Currencys.GetAllwithoutBaseCurrencyAsync();
                ViewBag.Currency = dataCurrency;

                var ROEData = await unitOfWork.ROENames.GetAllAsync();
                ViewBag.ROEName = ROEData;

                var CompanyData = await unitOfWork.Company.GetAllAsync();
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
                return RedirectToAction("index", "AdminLogin");
            }
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
            var data = await unitOfWork.PDAEstimitor.GetAlllistAsync();
            if (pDAEstimator.CustomerID != null && pDAEstimator.CustomerID != 0)
            {
                data = data.Where(x => x.CustomerID == pDAEstimator.CustomerID && x.PortID == pDAEstimator.PortID).ToList();
            }
            if (pDAEstimator.PortID != null && pDAEstimator.PortID != 0)
            {
                data = data.Where(x => x.PortID == pDAEstimator.PortID).ToList();
            }
            if (pDAEstimator.TerminalID != null && pDAEstimator.TerminalID != 0)
            {
                data = data.Where(x => x.TerminalID == pDAEstimator.TerminalID).ToList();
            }
            if (pDAEstimator.CallTypeID != null && pDAEstimator.CallTypeID != 0)
            {
                data = data.Where(x => x.CallTypeID == pDAEstimator.CallTypeID).ToList();
            }
            return PartialView("partial/_ViewAll", data);
        }

        public IActionResult PortNameOnchange(PDAEstimator PDAEstimitor)
        {
            var TerminalDetailData = unitOfWork.TerminalDetails.GetAllAsync().Result.Where(x => x.PortID == PDAEstimitor.PortID);

            ViewBag.Terminal = TerminalDetailData;
            return PartialView("partial/TerminalList");
        }

        public IActionResult TerminalNameOnchange(PDAEstimator PDAEstimitor)
        {
            var BearthDetailData = unitOfWork.BerthDetails.GetAllAsync().Result.Where(x => x.TerminalID == PDAEstimitor.TerminalID);
            ViewBag.Berth = BearthDetailData;
            return PartialView("partial/BerthList");
        }

        public async Task<ActionResult> PDAEstimitorSave(PDAEstimator PDAEstimitor)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string ETA_String = PDAEstimitor.ETA_String + " " + "12:00:00 AM";
            DateTime Validity_To = DateTime.ParseExact(ETA_String, new string[] { "dd.M.yyyy hh:mm:ss tt", "dd-M-yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt" }, provider, DateTimeStyles.None);

            PDAEstimitor.ETA = Validity_To;

            decimal berthStayHrs = PDAEstimitor.LoadDischargeRate != 0? Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(PDAEstimitor.CargoQty) / Convert.ToDecimal(PDAEstimitor.LoadDischargeRate)) * 24) + 4 : 0;
            decimal berthStayDay = PDAEstimitor.LoadDischargeRate != 0 ? Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(PDAEstimitor.CargoQty) / Convert.ToDecimal(PDAEstimitor.LoadDischargeRate))): 0;
            decimal berthStayShift = PDAEstimitor.LoadDischargeRate != 0 ?  Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(PDAEstimitor.CargoQty) / Convert.ToDecimal(PDAEstimitor.LoadDischargeRate) * 3)) :0;

           var calltype = unitOfWork.CallTypes.GetByIdAsync(PDAEstimitor.CallTypeID).Result;

            if (calltype.CallTypeName.ToUpper() == "FOREIGN")
            {
                PDAEstimitor.BerthStay = Convert.ToInt64(berthStayHrs);
                PDAEstimitor.BerthStayDay = Convert.ToInt64(berthStayDay);
                PDAEstimitor.BerthStayShift = Convert.ToInt64(berthStayShift);
            }
            if(calltype.CallTypeName.ToUpper() == "COASTAL IN FOREIGN OUT" || calltype.CallTypeName.ToUpper() == "FOREIGN IN COASTAL OUT")
            {
                PDAEstimitor.BerthStayHoursCoastal= Convert.ToInt64(berthStayHrs);
                PDAEstimitor.BerthStayDayCoastal= Convert.ToInt64(berthStayDay);
                PDAEstimitor.BerthStayShiftCoastal= Convert.ToInt64(berthStayShift);

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

            if (PDAEstimitor.PDAEstimatorID > 0)
            {
                await unitOfWork.PDAEstimitor.UpdateAsync(PDAEstimitor);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                var id = await unitOfWork.PDAEstimitor.AddAsync(PDAEstimitor);
                //if (id != "" && Convert.ToInt64(id) > 0)
                //{
                //    PDAEstimatorOutPut pDAEstimatorOutPut = new PDAEstimatorOutPut();
                //    //var PDAData = unitOfWork.PDAEstimitor.GetAlllistAsync().Result.Where(x => x.PDAEstimatorID == Convert.ToInt64(id)).FirstOrDefault();

                //    var CompanyData = unitOfWork.Company.GetAlllistAsync().Result.Where(x => x.CompanyId == PDAEstimitor.InternalCompanyID).FirstOrDefault();
                //    var TaxData = unitOfWork.Taxs.GetAllAsync().Result;

                //    string Addressline2 = "";
                //    pDAEstimatorOutPut.PDAEstimatorID = Convert.ToInt64(id);
                //    if (CompanyData != null)
                //    {
                //        if (CompanyData.Address2 != null)
                //        {
                //            Addressline2 = CompanyData.Address1.ToUpper() + ", " + CompanyData.Address2.ToUpper() + ", ";
                //        }
                //        else
                //        {
                //            Addressline2 = CompanyData.Address1.ToUpper() + ", ";
                //        }

                //        pDAEstimatorOutPut.CompanyName = CompanyData.CompanyName;
                //        pDAEstimatorOutPut.CompanyAddress1 = Addressline2;
                //        pDAEstimatorOutPut.CompanyAddress2 = CompanyData.CityName.ToUpper() + ", " + CompanyData.StateName.ToUpper() + ", " + CompanyData.CountryName.ToUpper();
                //        pDAEstimatorOutPut.CompanyTelephone = CompanyData.Telephone;
                //        pDAEstimatorOutPut.CompanyAlterTel = CompanyData.AlterTel;
                //        pDAEstimatorOutPut.CompanyEmail = CompanyData.Email.ToUpper();
                //        pDAEstimatorOutPut.CompanyLogo = CompanyData.CompanyLog;
                //    }
                //    else
                //    {
                //        pDAEstimatorOutPut.CompanyAddress1 = "";
                //        pDAEstimatorOutPut.CompanyAddress2 = "";
                //        pDAEstimatorOutPut.CompanyTelephone = "";
                //        pDAEstimatorOutPut.CompanyAlterTel = "";
                //        pDAEstimatorOutPut.CompanyEmail = "";
                //        pDAEstimatorOutPut.CompanyLogo = "";
                //    }

                //    var custdata = unitOfWork.Customer.GetByIdAsync(PDAEstimitor.CustomerID).Result;
                //    if (custdata != null && custdata.BankID != null)
                //    {
                //        var BankMaster = unitOfWork.BankMaster.GetByIdAsync(custdata.BankID).Result;
                //        if (BankMaster != null)
                //        {
                //            pDAEstimatorOutPut.NameofBeneficiary = BankMaster.NameofBeneficiary;
                //            pDAEstimatorOutPut.BeneficiaryAddress = BankMaster.BeneficiaryAddress;
                //            pDAEstimatorOutPut.AccountNo = BankMaster.AccountNo;
                //            pDAEstimatorOutPut.Beneficiary_Bank_Name = BankMaster.Beneficiary_Bank_Name;
                //            pDAEstimatorOutPut.Beneficiary_Bank_Address = BankMaster.Beneficiary_Bank_Address;
                //            pDAEstimatorOutPut.Beneficiary_RTGS_NEFT_IFSC_Code = BankMaster.Beneficiary_Bank_Name;
                //            pDAEstimatorOutPut.Beneficiary_Bank_Swift_Code = BankMaster.Beneficiary_Bank_Name;
                //            pDAEstimatorOutPut.Intermediary_Bank = BankMaster.Intermediary_Bank;
                //            pDAEstimatorOutPut.Intermediary_Bank_Swift_Code = BankMaster.Intermediary_Bank;
                //        }
                //    }

                //    var currencyData = unitOfWork.Currencys.GetAllAsync().Result;
                //    pDAEstimatorOutPut.BaseCurrencyCode = currencyData.Where(x => x.BaseCurrency == true) != null ? currencyData.Where(x => x.BaseCurrency == true).FirstOrDefault().CurrencyCode : "";
                //    pDAEstimatorOutPut.DefaultCurrencyCode = currencyData.Where(x => x.DefaultCurrecny == true) != null ? currencyData.Where(x => x.DefaultCurrecny == true).FirstOrDefault().CurrencyCode : "";
                //    var taxrate = TaxData.Where(x => x.TaxName.Contains("GST")).Select(x => x.TaxRate).FirstOrDefault();
                //    pDAEstimatorOutPut.Taxrate = taxrate;
                //    pDAEstimatorOutPut.PDAEstimatorOutPutDate = DateTime.Now;
                //    var PDAEstimitorOUTPUTid = await unitOfWork.PDAEstimitorOUTPUT.AddAsync(pDAEstimatorOutPut);

                //    if (PDAEstimitorOUTPUTid != "" && Convert.ToInt64(PDAEstimitorOUTPUTid) > 0)
                //    {
                //        var NotesData = unitOfWork.PDAEstimitor.GetNotes().Result.ToList();
                //        foreach (var note in NotesData)
                //        {
                //            PDAEstimatorOutPutNote pDAEstimatorOutPutNote = new PDAEstimatorOutPutNote();
                //            pDAEstimatorOutPutNote.PDAEstimatorOutPutID = Convert.ToInt64(PDAEstimitorOUTPUTid);
                //            pDAEstimatorOutPutNote.Note = note.Note;
                //            pDAEstimatorOutPutNote.sequnce = note.sequnce;
                //            // await unitOfWork.PDAEstimitorOUTNote.AddAsync(pDAEstimatorOutPutNote);
                //        }
                //    }

                //    var pDAEstimatorOutPutdata = unitOfWork.PDAEstimitorOUTPUT.GetByIdAsync(Convert.ToInt64(id)).Result;
                //    PDAEstimatorOutPutView pDAEstimatorOutPutView = new PDAEstimatorOutPutView()
                //    {
                //        PDAEstimatorID = pDAEstimatorOutPutdata.PDAEstimatorID,
                //        CompanyName = pDAEstimatorOutPutdata.CompanyName,
                //        CompanyAddress1 = pDAEstimatorOutPutdata.CompanyAddress1.ToUpper(),
                //        CompanyAddress2 = pDAEstimatorOutPutdata.CompanyAddress2.ToUpper(),
                //        CompanyTelephone = pDAEstimatorOutPutdata.CompanyTelephone.ToUpper(),
                //        CompanyAlterTel = pDAEstimatorOutPutdata.CompanyAlterTel.ToUpper(),
                //        CompanyEmail = pDAEstimatorOutPutdata.CompanyEmail.ToUpper(),
                //        CompanyLogo = pDAEstimatorOutPutdata.CompanyLogo,
                //    };


                //    BankMaster bankMaster = new BankMaster()
                //    {
                //        NameofBeneficiary = pDAEstimatorOutPutdata.NameofBeneficiary,
                //        BeneficiaryAddress = pDAEstimatorOutPutdata.BeneficiaryAddress,
                //        AccountNo = pDAEstimatorOutPutdata.AccountNo,
                //        Beneficiary_Bank_Name = pDAEstimatorOutPutdata.Beneficiary_Bank_Name,
                //        Beneficiary_Bank_Address = pDAEstimatorOutPutdata.Beneficiary_Bank_Address,
                //        Beneficiary_RTGS_NEFT_IFSC_Code = pDAEstimatorOutPutdata.Beneficiary_Bank_Name,
                //        Beneficiary_Bank_Swift_Code = pDAEstimatorOutPutdata.Beneficiary_Bank_Name,
                //        Intermediary_Bank = pDAEstimatorOutPutdata.Intermediary_Bank,
                //        Intermediary_Bank_Swift_Code = pDAEstimatorOutPutdata.Intermediary_Bank,
                //    };

                //    pDAEstimatorOutPutView.BankMaster = bankMaster;

                //    //var Notesdata = await unitOfWork.PDAEstimitorOUTNote.GetAllNotesByPDAEstimatorOutPutIDAsync(Convert.ToInt64(id));
                //    List<Notes> notes = new List<Notes>();

                //}

                _toastNotification.AddSuccessToastMessage("Submitted successfully");
                RedirectToAction("PDAEstimator", "PDAEstimator", id);
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
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
        //[HttpPost]
        //public FileResult Export(string GridHtml)
        //{

        //}
    }


}
