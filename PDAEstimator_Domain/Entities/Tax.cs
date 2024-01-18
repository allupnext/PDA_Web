using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Tax : AuditableEntity
    {

        public string TaxName { get; set; }

        public string CombineTaxName { get; set; } = string.Empty;
        public decimal TaxRate { get; set; }
        public bool CombineTax { get; set; } = true;

        public int CombineTaxId { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        public string ToDate_String { get; set; }
        public string FromDate_String { get; set; }
        public bool IsDeleted { get; set; }

    }
}
