using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Currency:AuditableEntity
    {
        
        public string CurrencyName { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public bool Status { get; set; } = true;

        public bool BaseCurrency { get; set; } = false;

        public bool DefaultCurrecny { get; set; } = false;
    }
}
