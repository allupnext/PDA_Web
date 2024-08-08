using System.ComponentModel.DataAnnotations;

namespace PDAEstimator_Domain.Entities
{
    public class PDAEstimator
    {
        public long PDAEstimatorID { get; set; }

        public int CustomerID { get; set; }

        public string VesselName { get; set; }

        public int PortID { get; set; }
        public int ActivityTypeId { get; set; }

        public int TerminalID { get; set; }
        public int BerthId { get; set; }

        public int CallTypeID { get; set; }

        public int CargoID { get; set; }

        public int CargoQty { get; set; }

        public int? CargoQtyCBM { get; set; }

        public string? CargoUnitofMasurement { get; set; }

        public int LoadDischargeRate { get; set; }

        public int CurrencyID { get; set; }

        public decimal ROE { get; set; }
        public long? BerthStayDay { get; set; }

        public int? DWT { get; set; }

        //public DateTime? ETA { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ETA { get; set; }
        public string ETA_String { get; set; }

        public decimal? ArrivalDraft { get; set; }

        public int? GRT { get; set; }
        public int? RGRT { get; set; }

        public int? NRT { get; set; }

        public long? BerthStay { get; set; }

        public int? AnchorageStay { get; set; }

        public decimal? LOA { get; set; }

        public decimal? Beam { get; set; }

        public bool IsDeleted { get; set; }

        public int? InternalCompanyID { get; set; }
        public long? BerthStayShift { get; set; }
        public int VesselBallast { get; set; } = 0;
        public int IsReducedGRT { get; set; } = 0;

        public long? BerthStayDayCoastal { get; set; }
        public long? BerthStayShiftCoastal { get; set; }
        public long? BerthStayHoursCoastal { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? CreationFromDate { get; set; }
        public DateTime? CreationToDate { get; set; }
        public string? ModifyUserID { get; set; }
        public DateTime? ModifyDate { get; set; }

        public string? Disclaimer { get; set; }

        public bool IsCustomerCreated { get; set; }

        public string? PortFile { get; set; }
        public string? PortFileTanker { get; set; }
    }

}
