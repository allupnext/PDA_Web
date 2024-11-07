using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class PDAEstimatorOutPutView
    {
        public long PDAEstimatorID { get; set; }

        public int CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }

        public string CompanyLogoBase64 { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; }
        public string CompanyTelephone { get; set; }
        public string CompanyAlterTel { get; set; }
        public string CompanyEmail { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string VesselName { get; set; }
        public string PortName { get; set; } = string.Empty;
        public int PortID { get; set; }
        public int ActivityTypeId { get; set; }
        
        public string TerminalName { get; set; } = string.Empty;
        public string BerthName { get; set; } = string.Empty;

        public DateTime? ETA { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public int TerminalID { get; set; }
        public string CallTypeName { get; set; } = string.Empty;

        public int CallTypeID { get; set; }
        public string CurrencyName { get; set; } = string.Empty;

        public int CargoID { get; set; }

        public int CargoQty { get; set; }
        public int? CargoQtyCBM { get; set; }


        public string? CargoUnitofMasurement { get; set; }

        public int LoadDischargeRate { get; set; }
        public string CargoName { get; set; } = string.Empty;

        public int CurrencyID { get; set; }

        public decimal ROE { get; set; }

        public int? DWT { get; set; }

        public decimal? ArrivalDraft { get; set; }

        public int? GRT { get; set; }

        public int? RGRT { get; set; }

        public int? NRT { get; set; }

        public long? BerthStay { get; set; }
        public long? BerthStayShift { get; set; }

        public long? BerthStayDay { get; set; }

        public long? BerthStayDayCoastal { get; set; }
        public long? BerthStayShiftCoastal { get; set; }
        public long? BerthStayHoursCoastal { get; set; }

        public int? AnchorageStay { get; set; }

        public decimal? LOA { get; set; }

        public decimal? Beam { get; set; }

        public bool IsDeleted { get; set; }
        public int? InternalCompanyID { get; set; }

        
        public BankMaster BankMaster { get; set; }
        public CompanyMasterList CompanyMasterList { get; set; }

        public List<Notes> NotesData { get; set; }

        public List<ChargeCodeList> ChargeCodes { get; set; }

        public List<PDATariffRateList> TariffRateList { get; set; }

        public List<Expense> Expenses{ get; set; }

        public string BaseCurrencyCode{ get; set; }

        public int BaseCurrencyCodeID { get; set; }

        public int DefaultCurrencyCodeID { get; set; }

        public string DefaultCurrencyCode { get; set; }

        public decimal Taxrate { get; set; }

        public int VesselBallast { get; set; } = 0;

        public int BerthID { get; set; }
        public string? Disclaimer { get; set; }
    }
}
