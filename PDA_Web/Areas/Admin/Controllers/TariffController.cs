using Azure.Core;
using ClosedXML.Excel;
using ExcelDataReader;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using PDA_Web.Models;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;
using System.Data;
using System.Globalization;
using System.Transactions;
using static System.Reflection.Metadata.BlobBuilder;

namespace PDA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TariffController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IToastNotification _toastNotification;
        public TariffController(IUnitOfWork unitOfWork, IToastNotification toastNotification)
        {
            this.unitOfWork = unitOfWork;
            _toastNotification = toastNotification;

        }

        public async Task<ActionResult> InsertTarrifFromSelectedPorts(CopyTarrifModelInput Ids)//Master Save
        {
            var InsertCopiedTarrif = await unitOfWork.TariffRates.InsertTarrifFromSelectedPorts(Ids);
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }
        public async Task<ActionResult> InsertTarrifFromSamePorts(CopyTarrifModelInput Ids)//Master Save
        {
            var InsertCopiedTarrif = await unitOfWork.TariffRates.InsertTarrifFromSamePorts(Ids);
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<IActionResult> Formula()
        {
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END
            var FormulaAttribute = await unitOfWork.FormatAttribute.GetAllAsync();
            ViewBag.FormulaAttribute = FormulaAttribute;
            var FormulaSlab = await unitOfWork.tariffSegment.GetAllAsync();
            ViewBag.FormulaSlab = FormulaSlab;
            var FormulaOprator = await unitOfWork.FormulaOprator.GetAllAsync();
            ViewBag.FormulaOprator = FormulaOprator;
            var PortsData = await unitOfWork.PortDetails.GetAllAsync();
            if (PortsData.Count > 0)
                PortsData = PortsData.Where(x => x.Status == true).ToList();
            ViewBag.Ports = PortsData;
            return View();

        }
        public async Task<ActionResult> formulaTransationSave(FormulaMasterTransSave formulaMasterTransSave)
        {
            int returnformulaId = 0;
            if (formulaMasterTransSave.formulaID > 0)
            {
                FormulaTransaction formulaTransaction = new FormulaTransaction();
                formulaTransaction.formulaID = formulaMasterTransSave.formulaID;
                formulaTransaction.formulaAttributeID = formulaMasterTransSave.formulaAttributeID;
                formulaTransaction.formulaValue = formulaMasterTransSave.formulaValue;
                formulaTransaction.formulaTrasID = formulaMasterTransSave.formulaTrasID;
                formulaTransaction.formulaSlabID = formulaMasterTransSave.formulaSlabID;
                formulaTransaction.formulaOperatorID = formulaMasterTransSave.formulaOperatorID;

                await unitOfWork.FormulaTransaction.AddAsync(formulaTransaction);
                returnformulaId = formulaMasterTransSave.formulaID;
            }
            else
            {
                FormulaMaster formulaMaster = new FormulaMaster();
                formulaMaster.formulaName = formulaMasterTransSave.formulaName;
                formulaMaster.PortID = formulaMasterTransSave.PortID;
                formulaMaster.formulaStatus = formulaMasterTransSave.formulaStatus;
                var formulaID = await unitOfWork.Formula.AddAsync(formulaMaster);
                if (!string.IsNullOrEmpty(formulaID))
                {
                    FormulaTransaction formulaTransaction = new FormulaTransaction();
                    formulaTransaction.formulaID = Convert.ToInt32(formulaID);
                    formulaTransaction.formulaAttributeID = formulaMasterTransSave.formulaAttributeID;
                    formulaTransaction.formulaValue = formulaMasterTransSave.formulaValue;
                    formulaTransaction.formulaTrasID = formulaMasterTransSave.formulaTrasID;
                    formulaTransaction.formulaSlabID = formulaMasterTransSave.formulaSlabID;
                    formulaTransaction.formulaOperatorID = formulaMasterTransSave.formulaOperatorID;

                    await unitOfWork.FormulaTransaction.AddAsync(formulaTransaction);
                    returnformulaId = Convert.ToInt32(formulaID);
                    //_toastNotification.AddSuccessToastMessage("Inserted successfully");
                }
            }
            return Json(new
            {
                proceed = true,
                msg = "",
                formulaid = returnformulaId
            });
        }
        public async Task<ActionResult> formulaSave(FormulaMaster formula)//Master Save
        {
            _toastNotification.AddSuccessToastMessage("submitted successfully");
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<IActionResult> CargoLoad(CargoHandleds cargoHandleds)
        {
            var TerminalId = cargoHandleds.TerminalID;
            var PortId = cargoHandleds.PortID;
            var cargoList = await unitOfWork.PDAEstimitor.GetCargoByTerminalAndPortAsync(TerminalId, PortId);
            ViewBag.Cargo = cargoList;
            return PartialView("partial/_CargoList");
        }

        public async Task<ActionResult> formulaByFormulaId(FormulaMaster formula)//Master Save
        {
            if (formula.formulaMasterID != null && formula.formulaMasterID > 0)
            {
                string formulastring = string.Empty;
                var formulatransdata = await unitOfWork.FormulaTransaction.GetAllTransAsync((int)formula.formulaMasterID);
                foreach (var formularTransList in formulatransdata)
                {
                    if (formularTransList.formulaAttributeID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaAttributeName : formularTransList.formulaAttributeName;
                    if (formularTransList.formulaSlabID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaSlabName : formularTransList.formulaSlabName;
                    if (formularTransList.formulaOperatorID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaOperatorName : formularTransList.formulaOperatorName;
                    if (formularTransList.formulaValue > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaValue : formularTransList.formulaValue.ToString();
                }
                return Json(new
                {
                    proceed = true,
                    msg = "",
                    formulastring = formulastring
                });
            }
            else
            {

                return Json(new
                {
                    proceed = true,
                    msg = ""
                });
            }
        }
        public async Task<ActionResult> formulaClear(FormulaMaster formula)//Master Save
        {
            await unitOfWork.FormulaTransaction.DeleteFormulaIdAsync(formula.formulaMasterID);
            //await unitOfWork.Formula.DeleteAsync(formula.formulaMasterID);

            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }


        public async Task<ActionResult> formulaDelete(FormulaMaster formula)
        {
            await unitOfWork.FormulaTransaction.DeleteByFormulaIdAsync(formula.formulaMasterID);
            await unitOfWork.Formula.DeleteAsync(formula.formulaMasterID);
            _toastNotification.AddSuccessToastMessage("Deleted successfully");

            return Json(new
            {
                proceed = true,
                msg = ""
            });
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
                var dataPortDetails = await unitOfWork.PortDetails.GetAllAsync();
                if (dataPortDetails.Count > 0)
                    dataPortDetails = dataPortDetails.Where(x => x.Status == true).ToList();
                ViewBag.Port = dataPortDetails;

                var dataTerminalDetails = await unitOfWork.TerminalDetails.GetAllAsync();
                if (dataTerminalDetails.Count > 0)
                    dataTerminalDetails = dataTerminalDetails.Where(x => x.Status == true).ToList();
                ViewBag.Terminal = dataTerminalDetails;

                var dataBerthDetails = await unitOfWork.BerthDetails.GetAllAsync();
                if (dataBerthDetails.Count > 0)
                    dataBerthDetails = dataBerthDetails.Where(x => x.BerthStatus == true).ToList();
                ViewBag.Berth = dataBerthDetails;

                var dataCargoDetails = await unitOfWork.CargoDetails.GetAllAsync();
                if (dataCargoDetails.Count > 0)
                    dataCargoDetails = dataCargoDetails.Where(x => x.CargoStatus == true).ToList();
                ViewBag.Cargo = dataCargoDetails;

                var dataCallTypes = await unitOfWork.CallTypes.GetAllAsync();
                if (dataCallTypes.Count > 0)
                    dataCallTypes = dataCallTypes.Where(x => x.Status == true).ToList();
                ViewBag.CallTypes = dataCallTypes;

                var dataExpenses = await unitOfWork.Expenses.GetAllAsync();
                if (dataExpenses.Count > 0)
                    dataExpenses = dataExpenses.Where(x => x.Status == true).ToList();
                ViewBag.Expenses = dataExpenses;

                var dataChargeCodes = await unitOfWork.ChargeCodes.GetAllAsync();
                if (dataChargeCodes.Count > 0)
                    dataChargeCodes = dataChargeCodes.Where(x => x.Status == true).ToList();
                ViewBag.ChargeCodes = dataChargeCodes;

                var dataCurrency = await unitOfWork.Currencys.GetAllAsync();
                if (dataCurrency.Count > 0)
                    dataCurrency = dataCurrency.Where(x => x.Status == true).ToList();
                ViewBag.Currency = dataCurrency;

                var dataSlabs = await unitOfWork.tariffSegment.GetAllAsync();
                ViewBag.Slabs = dataSlabs;

                var dataFormulas = await unitOfWork.Formula.GetAllAsync();
                ViewBag.Formulas = dataFormulas;

                var PortActivityData = await unitOfWork.PortActivities.GetAllAsync();
                ViewBag.PortActivity = PortActivityData;

                var dataTaxs = await unitOfWork.Taxs.GetAllAsync();
                ViewBag.Texs = dataTaxs;

                return View();
            }
            else
            {
                return RedirectToAction("index", "AdminLogin");
            }
        }

        public IActionResult PortNameOnchange(CargoHandleds cargoHandleds)
        {
            var TerminalDetailData = unitOfWork.TerminalDetails.GetAllAsync().Result.Where(x => x.PortID == cargoHandleds.PortID);
            if (TerminalDetailData.Count() > 0)
                TerminalDetailData = TerminalDetailData.Where(x => x.Status == true).ToList();
            ViewBag.Terminal = TerminalDetailData;
            return PartialView("partial/TerminalList");
        }

        public IActionResult PortNameOnchangeTterminalFilter(CargoHandleds cargoHandleds)
        {
            var TerminalDetailData = unitOfWork.TerminalDetails.GetAllAsync().Result.Where(x => x.PortID == cargoHandleds.PortID);
            if (TerminalDetailData.Count() > 0)
                TerminalDetailData = TerminalDetailData.Where(x => x.Status == true).ToList();
            ViewBag.Terminal = TerminalDetailData;
            return PartialView("partial/_TerminalFilterList");
        }

        public async Task<IActionResult> PortNameOnchangeCargoFilter(CargoHandleds cargoHandleds)
        {
            var TerminalId = cargoHandleds.TerminalID;
            var PortId = cargoHandleds.PortID;
            var cargoList = await unitOfWork.PDAEstimitor.GetCargoByTerminalAndPortAsync(TerminalId, PortId);

            ViewBag.Cargo = cargoList;
            return PartialView("partial/_CargoFilterList");
        }

        public IActionResult PortNameOnchangeForumla(CargoHandleds cargoHandleds)
        {
            var dataFormulas = unitOfWork.Formula.GetAllAsync().Result.Where(x => x.PortID == cargoHandleds.PortID || x.PortID == null || x.PortID == 0);
            ViewBag.Formulas = dataFormulas;
            return PartialView("partial/FormulaList");
        }


        public IActionResult TerminalNameOnchange(CargoHandleds cargoHandleds)
        {
            var BerthDetailData = unitOfWork.BerthDetails.GetAllAsync().Result.Where(x => x.TerminalID == cargoHandleds.TerminalID);
            if (BerthDetailData.Count() > 0)
                BerthDetailData = BerthDetailData.Where(x => x.BerthStatus == true).ToList();
            ViewBag.Berth = BerthDetailData;
            return PartialView("partial/BerthList");
        }

        public IActionResult ExpensesOnchange(int Id)
        {
            var ChargeCodesData = unitOfWork.ChargeCodes.GetAllAsync().Result.Where(x => x.ExpenseCategoryID == Id);
            ViewBag.ChargeCodes = ChargeCodesData;
            return PartialView("partial/ChargeCodesList");
        }

        public async Task<IActionResult> LoadAll(TariffRateList tariff)
        {
            List<TariffRateList> TariffRatedata = new List<TariffRateList>();
            TariffRatedata = await unitOfWork.TariffMasters.GetAllTariffRateAsync(tariff.PortID);

            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END
            var formulatransdata = await unitOfWork.FormulaTransaction.GetAllTransAsync();
            foreach (var TariffRate in TariffRatedata)
            {
                if (TariffRate.FormulaID != null && TariffRate.FormulaID > 0)
                {
                    string formulastring = string.Empty;
                    var formulatran = formulatransdata.Where(x => x.formulaID == ((int)TariffRate.FormulaID));

                    foreach (var formularTransList in formulatran)
                    {
                        if (formularTransList.formulaAttributeID > 0)
                            formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaAttributeName : formularTransList.formulaAttributeName;
                        if (formularTransList.formulaSlabID > 0)
                            formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaSlabName : formularTransList.formulaSlabName;
                        if (formularTransList.formulaOperatorID > 0)
                            formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaOperatorName : formularTransList.formulaOperatorName;
                        if (formularTransList.formulaValue > 0)
                            formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaValue.ToString("#,##0.#######") : formularTransList.formulaValue.ToString("#,##0.#######");
                    }
                    TariffRate.Formula = formulastring;
                    TariffRate.FormulaID = (int)TariffRate.FormulaID;
                }
            }


            if (tariff.TerminalID != null && tariff.TerminalID != 0)
            {
                TariffRatedata = TariffRatedata.Where(x => x.TerminalID == tariff.TerminalID).ToList();
            }
            if (tariff.BerthID != null && tariff.BerthID != 0)
            {
                TariffRatedata = TariffRatedata.Where(x => x.BerthID == tariff.BerthID).ToList();
            }
            if (tariff.CargoID != null && tariff.CargoID != 0)
            {
                TariffRatedata = TariffRatedata.Where(x => x.CargoID == tariff.CargoID).ToList();
            }
            if (tariff.status != null && tariff.status != 0)
            {
                TariffRatedata = TariffRatedata.Where(x => x.status == tariff.status).ToList();
            }
            if (tariff.Validity_From != null /*&& tariff. Validity_From != 0*/)
            {
                TariffRatedata = TariffRatedata.Where(x => x.Validity_From >= tariff.Validity_From).ToList();
            }
            if (tariff.Validity_To != null/* && tariff.Validity_To != 0*/)
            {
                TariffRatedata = TariffRatedata.Where(x => x.Validity_To <= tariff.Validity_To).ToList();
            }
            if (tariff.CallTypeID != null && tariff.CallTypeID != 0)
            {
                TariffRatedata = TariffRatedata.Where(x => x.CallTypeID == tariff.CallTypeID).ToList();
            }

            if (tariff.ExpenseCategoryID != null && tariff.ExpenseCategoryID != 0)
            {
                TariffRatedata = TariffRatedata.Where(x => x.ExpenseCategoryID == tariff.ExpenseCategoryID).ToList();
            }
            if (tariff.ChargeCodeID != null && tariff.ChargeCodeID != 0)
            {
                TariffRatedata = TariffRatedata.Where(x => x.ChargeCodeID == tariff.ChargeCodeID).ToList();
            }
            return PartialView("partial/_ViewAllTariffRate", TariffRatedata);
        }


        public async Task<IActionResult> LoadAllFormula(TariffRateList tariff)
        {
            List<FormulaList> formulaLists = new List<FormulaList>();

            var formulasdata = await unitOfWork.Formula.GetAllAsync();
            // Temp Solution START
            var UserPermissionModel = await unitOfWork.Roles.GetUserPermissionRights();
            ViewBag.UserPermissionModel = UserPermissionModel;
            var Currentuser = HttpContext.Session.GetString("UserID");

            var UserRole = await unitOfWork.Roles.GetUserRoleName(Convert.ToInt64(Currentuser));
            ViewBag.UserRoleName = UserRole;
            // Temp Solution END
            string formulastring = string.Empty;
            foreach (var formula in formulasdata)
            {
                formulastring = string.Empty;
                var formulatransdata = await unitOfWork.FormulaTransaction.GetAllTransAsync(formula.formulaMasterID);
                foreach (var formularTransList in formulatransdata)
                {
                    if (formularTransList.formulaAttributeID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaAttributeName : formularTransList.formulaAttributeName;
                    if (formularTransList.formulaSlabID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaSlabName : formularTransList.formulaSlabName;
                    if (formularTransList.formulaOperatorID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaOperatorName : formularTransList.formulaOperatorName;
                    if (formularTransList.formulaValue > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaValue : formularTransList.formulaValue.ToString();
                }

                string portname = "";
                if (formula.PortID != null && formula.PortID > 0)
                {
                    var PortData = await unitOfWork.PortDetails.GetByIdAsync((int)formula.PortID);
                    portname = PortData.PortName;
                }
                FormulaList formulaList = new FormulaList();
                formulaList.formula = formulastring;
                formulaList.PortName = portname;
                formulaList.formulaMasterID = formula.formulaMasterID;

                formulaList.formulaName = formula.formulaName;
                formulaList.formulaStatus = formula.formulaStatus;
                formulaLists.Add(formulaList);
            }

            return PartialView("partial/_ViewAllFormula", formulaLists);
        }


        public async Task<ActionResult> TariffSave(TariffRateList tariff)
        {

            TariffMaster tariffMaster = new TariffMaster();
            tariffMaster.PortID = tariff.PortID;
            TariffRate tariffRate = new TariffRate();
            tariffRate.PortID = tariff.PortID;
            tariffRate.TerminalID = tariff.TerminalID;
            tariffRate.BerthID = tariff.BerthID;
            tariffRate.CallTypeID = tariff.CallTypeID;
            tariffRate.CargoID = tariff.CargoID;
            tariffRate.CurrencyID = tariff.CurrencyID;
            tariffRate.SlabID = tariff.SlabID;
            tariffRate.SlabFrom = tariff.SlabFrom;
            tariffRate.SlabTo = tariff.SlabTo;
            tariffRate.Rate = tariff.Rate;
            tariffRate.ChargeCodeID = tariff.ChargeCodeID;
            tariffRate.ExpenseCategoryID = tariff.ExpenseCategoryID;
            tariffRate.Validity_From = tariff.Validity_From;
            tariffRate.Validity_To = tariff.Validity_To;
            tariffRate.TaxID = tariff.TaxID;
            tariffRate.Remark = tariff.Remark;
            tariffRate.SlabIncreemental = tariff.SlabIncreemental;
            tariffRate.VesselBallast = tariff.VesselBallast;

            await unitOfWork.TariffRates.AddAsync(tariffRate);
            _toastNotification.AddSuccessToastMessage("Inserted successfully");

            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> TariffRateSave(TariffRate tariffrate)
        {
            //var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
            //var test1 = HttpContext.Features.Get<IHttpConnectionFeature>();
            //var test2 = HttpContext.Connection.RemoteIpAddress;
            CultureInfo provider = CultureInfo.InvariantCulture;

            string Validity_Tostring = tariffrate.StringValidity_To + " " + "12:00:00 AM";
            string Validity_Fromstring = tariffrate.StringValidity_From + " " + "12:00:00 AM";

            DateTime Validity_To = DateTime.ParseExact(Validity_Tostring, new string[] { "dd.M.yyyy hh:mm:ss tt", "dd-M-yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt" }, provider, DateTimeStyles.None);
            DateTime Validity_Froms = DateTime.ParseExact(Validity_Fromstring, new string[] { "dd.M.yyyy hh:mm:ss tt", "dd-M-yyyy hh:mm:ss tt", "dd/M/yyyy hh:mm:ss tt" }, provider, DateTimeStyles.None);

            tariffrate.Validity_From = Validity_Froms;
            tariffrate.Validity_To = Validity_To;



            if (tariffrate.TariffRateID > 0)
            {
                var Currentuser = HttpContext.Session.GetString("UserID");
                tariffrate.ModifyUserID = Currentuser;
                await unitOfWork.TariffRates.UpdateAsync(tariffrate);
                _toastNotification.AddSuccessToastMessage("Updated Successfully");
            }
            else
            {
                var Currentuser = HttpContext.Session.GetString("UserID");
                tariffrate.CreatedBy = Currentuser;
                await unitOfWork.TariffRates.AddAsync(tariffrate);
                _toastNotification.AddSuccessToastMessage("Inserted successfully");
            }
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }


        public async Task<ActionResult> EditTTariffRate(TariffRate tariffrate)
        {
            var data = await unitOfWork.TariffRates.GetByIdAsync(tariffrate.TariffRateID);
            return Json(new
            {
                TariffRates = data,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> EditFormula(int Id)
        {
            string formulastring = string.Empty, formulaname = string.Empty;
            int? portId = 0;
            bool status = false;
            if (Id > 0)
            {
                var formulamasterdata = await unitOfWork.Formula.GetByIdAsync(Id);
                var formulatransdata = await unitOfWork.FormulaTransaction.GetAllTransAsync((int)Id);
                foreach (var formularTransList in formulatransdata)
                {
                    if (formularTransList.formulaAttributeID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaAttributeName : formularTransList.formulaAttributeName;
                    if (formularTransList.formulaSlabID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaSlabName : formularTransList.formulaSlabName;
                    if (formularTransList.formulaOperatorID > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaOperatorName : formularTransList.formulaOperatorName;
                    if (formularTransList.formulaValue > 0)
                        formulastring = formulastring != "" ? formulastring + " " + formularTransList.formulaValue.ToString("#,##0.#######") : formularTransList.formulaValue.ToString("#,##0.#######");
                }

                formulaname = formulamasterdata.formulaName;
                portId = formulamasterdata.PortID;
                status = formulamasterdata.formulaStatus;
            }
            return Json(new
            {
                formulaname = formulaname,
                formulastring = formulastring,
                portId = portId,
                status = status,
                proceed = true,
                msg = ""
            });
        }

        public async Task<ActionResult> deleteTariffRate(TariffRate tariffrate)
        {
            var data = await unitOfWork.TariffRates.DeleteAsync(tariffrate.TariffRateID);
            return Json(new
            {
                proceed = true,
                msg = ""
            });
        }


        [HttpGet]
        public async Task<IActionResult> DownloadTariffExcelTemplate()
        {
            // =========================
            // LOAD MASTER DATA (DB)
            // =========================

            var ports = (await unitOfWork.PortDetails.GetAllAsync())
                .Select(x => x.PortName).ToList();

            var terminals = (await unitOfWork.TerminalDetails.GetAllAsync())
                .Select(x => x.TerminalName).ToList();

            var berths = (await unitOfWork.BerthDetails.GetAllAsync())
                .Select(x => x.BerthName).ToList();

            var cargos = (await unitOfWork.CargoDetails.GetAllAsync())
                .Select(x => x.CargoName).ToList();

            var callTypes = (await unitOfWork.CallTypes.GetAllAsync())
                .Select(x => x.CallTypeName).ToList();

            var expenses = (await unitOfWork.Expenses.GetAllAsync())
                .Select(x => x.ExpenseName).ToList();

            var chargeCodes = (await unitOfWork.ChargeCodes.GetAllAsync())
                .Select(x => x.ChargeCodeName).ToList();

            var currencies = (await unitOfWork.Currencys.GetAllAsync())
                .Select(x => x.CurrencyCode).ToList();

            var slabs = (await unitOfWork.tariffSegment.GetAllAsync())
                .Select(x => x.TariffSegmentName).ToList();

            var formulas = (await unitOfWork.Formula.GetAllAsync())
                .Select(x => x.formulaName).ToList();

            var taxes = (await unitOfWork.Taxs.GetAllAsync())
                .Select(x => x.TaxName).ToList();

            var portActivities = (await unitOfWork.PortActivities.GetAllAsync())
                .Select(x => x.ActivityType).ToList();

            // =========================
            // CREATE EXCEL
            // =========================

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("TariffTemplate");

            string[] headers =
            {
                "TariffRateID","Port","Terminal","Berth","Cargo",
                "Validity_From","Validity_To","OperationType","Status",
                "CallType","Expenses","ChargeCodes",
                "VesselBallast","Reduced_GRT","Tax","Slab",
                "SlabFrom","SlabTo","Range_TariffID",
                "Rate","Currency","Formula","Remark"
            };

            for (int i = 0; i < headers.Length; i++)
                ws.Cell(1, i + 1).Value = headers[i];

            ws.Row(1).Style.Font.Bold = true;
            ws.SheetView.FreezeRows(1);

            // =========================
            // MASTER DATA (HIDDEN)
            // =========================

            var master = wb.Worksheets.Add("MasterData");
            master.Visibility = XLWorksheetVisibility.VeryHidden;

            int col = 1;
            var masterRowCount = new Dictionary<int, int>();

            void Fill(string title, List<string> values)
            {
                master.Cell(1, col).Value = title;

                for (int i = 0; i < values.Count; i++)
                    master.Cell(i + 2, col).Value = values[i];

                masterRowCount[col] = values.Count + 1; // header + data
                col++;
            }

            Fill("Port", ports);
            Fill("Terminal", terminals);
            Fill("Berth", berths);
            Fill("Cargo", cargos);
            Fill("OperationType", portActivities);
            Fill("Status", new() { "Active", "Expired", "Pending" });
            Fill("CallType", callTypes);
            Fill("Expenses", expenses);
            Fill("ChargeCodes", chargeCodes);
            Fill("YesNo", new() { "Not-Applicable", "No", "Yes" });
            Fill("Tax", taxes);
            Fill("Slab", slabs);
            Fill("Currency", currencies);
            Fill("Formula", formulas);
            Fill("Incremental", new() { "Increemental", "Not-Increemental" });

            // =========================
            // 4️⃣ APPLY DROPDOWNS (FIXED)
            // =========================

            void Drop(string column, int masterCol)
            {
                var lastRow = masterRowCount[masterCol];
                var dv = ws.Range($"{column}:{column}").SetDataValidation();
                dv.List(master.Range(2, masterCol, lastRow, masterCol));
            }

            Drop("B", 1);   // Port
            Drop("C", 2);   // Terminal
            Drop("D", 3);   // Berth
            Drop("E", 4);   // Cargo
            Drop("H", 5);   // OperationType
            Drop("I", 6);   // Status
            Drop("J", 7);   // CallType
            Drop("K", 8);   // Expenses
            Drop("L", 9);   // ChargeCodes
            Drop("M", 10);  // VesselBallast
            Drop("N", 10);  // Reduced_GRT
            Drop("O", 11);  // Tax
            Drop("P", 12);  // Slab
            Drop("U", 13);  // Currency
            Drop("V", 14);  // Formula
            Drop("R", 15);  // SlabIncreemental

            // =========================
            // 5️⃣ DATA TYPE VALIDATION
            // =========================

            // Validity_From
            var fromDateValidation = ws.Range("F:F").SetDataValidation();
            fromDateValidation.AllowedValues = XLAllowedValues.Date;
            fromDateValidation.Operator = XLOperator.Between;
            fromDateValidation.MinValue = "DATE(2000,1,1)";
            fromDateValidation.MaxValue = "DATE(2100,12,31)";
            fromDateValidation.ShowErrorMessage = true;
            fromDateValidation.ErrorTitle = "Invalid Date";
            fromDateValidation.ErrorMessage = "Enter a valid date";

            // Validity_To
            var toDateValidation = ws.Range("G:G").SetDataValidation();
            toDateValidation.AllowedValues = XLAllowedValues.Date;
            toDateValidation.Operator = XLOperator.Between;
            toDateValidation.MinValue = "DATE(2000,1,1)";
            toDateValidation.MaxValue = "DATE(2100,12,31)";
            toDateValidation.ShowErrorMessage = true;
            toDateValidation.ErrorTitle = "Invalid Date";
            toDateValidation.ErrorMessage = "Enter a valid date";

            // Date display format (calendar picker)
            ws.Column("F").Style.NumberFormat.Format = "dd/MM/yyyy";
            ws.Column("G").Style.NumberFormat.Format = "dd/MM/yyyy";

            // Default current date (optional UX)
            ws.Cell("F2").Value = DateTime.Today;
            ws.Cell("G2").Value = DateTime.Today;


            ws.Range("Q:R").SetDataValidation().AllowedValues = XLAllowedValues.Decimal;
            ws.Range("T:T").SetDataValidation().AllowedValues = XLAllowedValues.Decimal;
            ws.Range("S:S").SetDataValidation().AllowedValues = XLAllowedValues.WholeNumber;

            // =========================
            // 6️⃣ AUTO COLUMN WIDTH BASED ON DROPDOWN TEXT
            // =========================

            void SetWidth(int colIndex, IEnumerable<string> values)
            {
                var max = values.Any() ? values.Max(x => x.Length) : 12;
                ws.Column(colIndex).Width = Math.Max(14, max + 4);
            }

            SetWidth(2, ports);
            SetWidth(3, terminals);
            SetWidth(4, berths);
            SetWidth(5, cargos);
            SetWidth(8, portActivities);
            SetWidth(9, new[] { "Active", "Expired", "Pending" });
            SetWidth(10, callTypes);
            SetWidth(11, expenses);
            SetWidth(12, chargeCodes);
            SetWidth(13, new[] { "Not-Applicable", "No", "Yes" });
            SetWidth(14, new[] { "Not-Applicable", "No", "Yes" });
            SetWidth(15, taxes);
            SetWidth(16, slabs);
            SetWidth(21, currencies);
            SetWidth(22, formulas);

            ws.Columns().AdjustToContents();

            // =========================
            // 7️⃣ RETURN FILE
            // =========================

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Tariff_Excel_Template.xlsx"
            );
        }



        [HttpPost]
        public async Task<ActionResult> UploadTariffExcel(IFormFile file)
        {
            var errors = new List<string>();
            var entities = new List<TariffRate>();

            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var ds = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            var table = ds.Tables[0];

            int excelRow = 1; // header

            foreach (DataRow row in table.Rows)
            {
                excelRow++;

                // ===== TariffRateID (UPSERT KEY) =====
                int tariffRateId = 0;
                if (HasValue(row[0]) && int.TryParse(row[0].ToString(), out var trId))
                    tariffRateId = trId;

                var req = new TariffLookupRequest
                {
                    Port = row[1]?.ToString(),
                    Terminal = row[2]?.ToString(),
                    Berth = row[3]?.ToString(),
                    Cargo = row[4]?.ToString(),
                    OperationType = row[7]?.ToString(),
                    CallType = row[9]?.ToString(),
                    Expense = row[10]?.ToString(),
                    ChargeCode = row[11]?.ToString(),
                    Tax = row[14]?.ToString(),
                    Slab = row[15]?.ToString(),
                    Currency = row[20]?.ToString(),
                    Formula = row[21]?.ToString()
                };

                var lookup = await unitOfWork.TariffRates.ResolveAllIdsAsync(req);

                // ===== REQUIRED (MATCH UI) =====
                if (!HasValue(row[1]) || lookup.PortID == 0)
                    errors.Add($"Row {excelRow}: Invalid port");

                if (HasValue(row[2]) && lookup.TerminalID == 0)
                    errors.Add($"Row {excelRow}: Invalid terminal");

                if (HasValue(row[3]) && lookup.BerthID == 0)
                    errors.Add($"Row {excelRow}: Invalid berth");

                if (HasValue(row[4]) && lookup.CargoID == 0)
                    errors.Add($"Row {excelRow}: Invalid cargo");

                if (!DateTime.TryParse(row[5]?.ToString(), out DateTime vf))
                    errors.Add($"Row {excelRow}: Invalid validity from date");

                if (!DateTime.TryParse(row[6]?.ToString(), out DateTime vt))
                    errors.Add($"Row {excelRow}: Invalid validity to date");

                if (HasValue(row[7]) && lookup.OperationTypeID == 0)
                    errors.Add($"Row {excelRow}: Invalid Operation");

                if (HasValue(row[8]) && MapStatus(row[8]) == 0)
                    errors.Add($"Row {excelRow}: Invalid Status");

                if (HasValue(row[9]) && lookup.CallTypeID == 0)
                    errors.Add($"Row {excelRow}: Invalid call type");

                if (!HasValue(row[10]) || lookup.ExpenseCategoryID == 0)
                    errors.Add($"Row {excelRow}: Invalid expenses");

                if (!HasValue(row[11]) || lookup.ChargeCodeID == 0)
                    errors.Add($"Row {excelRow}: Invalid charge codes");

                if (HasValue(row[12]) && MapYesNo(row[12]) == 0)
                    errors.Add($"Row {excelRow}: Invalid VesselBallast");
                
                if (HasValue(row[13]) && MapYesNo(row[13]) == 0)
                    errors.Add($"Row {excelRow}: Invalid Reduced GRT");

                if (HasValue(row[14]) && lookup.TaxID == 0)
                    errors.Add($"Row {excelRow}: Invalid tax");

                if (!decimal.TryParse(row[19]?.ToString(), out decimal rate))
                    errors.Add($"Row {excelRow}: Invalid rate");

                if (HasValue(row[20]) && lookup.CurrencyID == 0)
                    errors.Add($"Row {excelRow}: Invalid Currency");

                if (!HasValue(row[20]) || lookup.FormulaID == 0)
                    errors.Add($"Row {excelRow}: Invalid formulas");
           
                // ===== SLAB RULE =====
                bool slabSelected = HasValue(row[15]);

                decimal? slabFrom = null;
                decimal? slabTo = null;

                if (slabSelected)
                {
                    if (lookup.SlabID == 0)
                        errors.Add($"Row {excelRow}: Invalid slab");

                    if (!decimal.TryParse(row[16]?.ToString(), out var sf))
                        errors.Add($"Row {excelRow}: Please enter slab from");
                    else slabFrom = sf;

                    if (!decimal.TryParse(row[17]?.ToString(), out var st))
                        errors.Add($"Row {excelRow}: Please enter slab to");
                    else slabTo = st;
                }

                if (errors.Any()) continue;

                entities.Add(new TariffRate
                {
                    TariffRateID = tariffRateId,

                    PortID = lookup.PortID,
                    CurrencyID = lookup.CurrencyID,

                    TerminalID = lookup.TerminalID,
                    BerthID = lookup.BerthID,
                    CargoID = lookup.CargoID,
                    CallTypeID = lookup.CallTypeID,
                    ExpenseCategoryID = lookup.ExpenseCategoryID,
                    ChargeCodeID = lookup.ChargeCodeID,
                    TaxID = lookup.TaxID,
                    SlabID = slabSelected ? lookup.SlabID : null,
                    FormulaID = lookup.FormulaID,
                    OperationTypeID = lookup.OperationTypeID,

                    Validity_From = vf,
                    Validity_To = vt,
                    Rate = rate,

                    SlabFrom = slabFrom,
                    SlabTo = slabTo,
                    Range_TariffID = int.TryParse(row[18]?.ToString(), out var rt) ? rt : 0,

                    VesselBallast = MapYesNo(row[12]),
                    Reduced_GRT = MapYesNo(row[13]),
                    SlabIncreemental = MapIncremental(row[15]),
                    status = MapStatus(row[8]),

                    Remark = row[22]?.ToString(),
                    CreatedBy = HttpContext.Session.GetString("UserID"),
                    ModifyUserID = HttpContext.Session.GetString("UserID"),
                    CreationDate = DateTime.Now,
                    IsDeleted = false
                });
            }

            if (errors.Any())
                return Json(new { success = false, errors });

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            foreach (var item in entities)
            {
                if (item.TariffRateID > 0)
                    await unitOfWork.TariffRates.UpdateAsync(item);   // 🔁 UPDATE
                else
                    await unitOfWork.TariffRates.AddAsync(item);      // ➕ INSERT
            }

            scope.Complete();

            return Json(new { success = true });
        }



        bool HasValue(object val)
        {
            return val != null && !string.IsNullOrWhiteSpace(val.ToString());
        }

        int MapYesNo(object val)
        {
            if (!HasValue(val)) return 0;
            var v = val.ToString().Trim().ToLower();
            if (v == "yes") return 2;
            if (v == "no") return 1;
            return 0;
        }

        int MapStatus(object val)
        {
            if (!HasValue(val)) return 0; // invalid
            var v = val.ToString().Trim().ToLower();
            if (v == "active") return 1;
            if (v == "expired") return 2;
            if (v == "pending") return 3;
            return 0; // invalid
        }

        int MapIncremental(object val)
        {
            if (!HasValue(val)) return 0;
            return val.ToString().Trim().ToLower() == "incremental" ? 1 : 0;
        }


    }
}
