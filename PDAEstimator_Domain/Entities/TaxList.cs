using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class TaxList : AuditableEntity
    {
        public string TaxName { get; set; }
        public string TaxNames { get; set; } = string.Empty;

        public decimal TaxRate { get; set; }
        public bool CombineTax { get; set; } = true;

        public int CombineTaxId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
