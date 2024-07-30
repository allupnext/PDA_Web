using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;
using NToastNotify;
using PDAEstimator_Application.Interfaces;
using PDAEstimator_Domain.Entities;
using PDAEstimator_Infrastructure.Repositories;
using System.Globalization;
using static System.Reflection.Metadata.BlobBuilder;
using Azure.Core;
using Microsoft.AspNetCore.Http.Features;
using PDA_Web.Models;
using iTextSharp.text.pdf;

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
            if (TerminalDetailData.Count()> 0)
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
            var dataFormulas =  unitOfWork.Formula.GetAllAsync().Result.Where(x=>x.PortID == cargoHandleds.PortID || x.PortID == null || x.PortID == 0);
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
                    var formulatran = formulatransdata.Where(x=>x.formulaID == ((int)TariffRate.FormulaID));

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
            tariffRate.TaxID= tariff.TaxID;
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


    }
}
