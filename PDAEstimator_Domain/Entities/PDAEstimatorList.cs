using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class PDAEstimatorList
    {
        public int PDAEstimatorID { get; set; }

        public int CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string VesselName { get; set; }
        public string PortName { get; set; } = string.Empty;
        public int PortID { get; set; }
        public int ActivityTypeId { get; set; }
        public long? BerthStayDay { get; set; }
        public string TerminalName { get; set; } = string.Empty;
        public string BerthName { get; set; } = string.Empty;
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ETA { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public int TerminalID { get; set; }
        public int BerthId { get; set; }

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

        public int? AnchorageStay { get; set; }

        public decimal? LOA { get; set; }

        public decimal? Beam { get; set; }

        public bool IsDeleted { get; set; }
        public int? InternalCompanyID { get; set; }
        public string CustomerCompanyName { get; set; }

        public string InternalCompanyName { get; set; }
        public long? BerthStayShift { get; set; }

        public int VesselBallast { get; set; } = 0;
        public int IsReducedGRT { get; set; } = 0;
        public long? BerthStayDayCoastal { get; set; }
        public long? BerthStayShiftCoastal { get; set; }
        public long? BerthStayHoursCoastal { get; set; }

        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }
        public string ModifyUser { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ModifyDate { get; set; }
        
        public string? PortFile { get; set; }
        public string? PortFileTanker { get; set; }
    }
}
