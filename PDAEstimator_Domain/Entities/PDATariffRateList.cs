using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class PDATariffRateList 
    {
        public int TariffRateID { get; set; } 
        public int PortID { get; set; }
       
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Validity_From { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Validity_To { get; set; }

        public int? TerminalID { get; set; }

     
        public int? BerthID { get; set; }
        public int? CargoID { get; set; }

        public int? CallTypeID { get; set; }

      
        public int ExpenseCategoryID { get; set; }

        public string? ExpenseName { get; set; }

        public int ChargeCodeID { get; set; }
        public string? ChargeCodeName { get; set; }

        public int? SlabID { get; set; }
        public string? SlabName { get; set; }
        public decimal? SlabFrom { get; set; }
        public decimal? SlabTo { get; set; }

        public decimal Rate { get; set; }
        public int status { get; set; }

        public int? CurrencyID { get; set; }

        public int? FormulaID { get; set; }

        public string? Formula { get; set; }

        public string? Remark { get; set; }
        public int? TaxID { get; set; }

        public decimal? UNITS { get; set; }

        public decimal Amount { get; set; }

        public decimal GSTBase { get; set; }

        public decimal GSTDefault { get; set; }

        public int? SlabIncreemental { get; set; }
        public bool NonIncreemental { get; set; } = true;

        public int VesselBallast { get; set; } = 0;

        public int ChargeCodeSequence { get; set; } = 0;

        public int Reduced_GRT { get; set; } = 0;
        public int Range_TariffID { get; set; } = 0;

        public int? OperationTypeID { get; set; }

    }
}
