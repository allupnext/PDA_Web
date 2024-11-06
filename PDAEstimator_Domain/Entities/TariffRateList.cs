using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class TariffRateList : AuditableEntity
    {
        public int TariffRateID { get; set; } 
        public int TariffID { get; set; }
        public int PortID { get; set; }
        public string PortName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Validity_From { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Validity_To { get; set; }

        public int? TerminalID { get; set; }

        public string? TerminalName { get; set; }

        public int BerthID { get; set; }
        public string? BerthName { get; set; }
        public int? CargoID { get; set; }

        public string? CargoName { get; set; }
        public int? CallTypeID { get; set; }

        public string? CallTypeName { get; set; }

        public int ExpenseCategoryID { get; set; }

        public string? ExpenseName { get; set; }

        public int ChargeCodeID { get; set; }
        public int? SlabID { get; set; }
        public string? SlabName { get; set; }
        public decimal? SlabFrom { get; set; }
        public decimal? SlabTo { get; set; }

        public string? ChargeCodeName { get; set; }

        public decimal Rate { get; set; }
        public int? OperationTypeID { get; set; }
        public int status { get; set; }

        public int? CurrencyID { get; set; }

        public int? FormulaID { get; set; }

        public string? Formula { get; set; }
        public string? CurrencyCode { get; set; }
        public bool IsDeleted { get; set; }

        public string? Remark { get; set; }
        public int? TaxID { get; set; }
        public string? TaxName { get; set; }

        public string? ModifyUser { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ModifyDate { get; set; }
        public string? CreatedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreationDate { get; set; }


        public int? SlabIncreemental { get; set; }
        public int VesselBallast { get; set; } = 0;
        public int Reduced_GRT { get; set; } = 0;
        public int Range_TariffID { get; set; } = 0;
    }
}
