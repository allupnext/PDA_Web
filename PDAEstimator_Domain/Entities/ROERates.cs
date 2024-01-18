using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PDAEstimator_Domain.Entities
{
    public class ROERates:AuditableEntity
    {
        public Int64 ID { get; set; }

        public int RoenameID { get; set; }
        public string Roename { get; set; }

        public int CurrencyId  { get; set; }
        public int UserId { get; set; }
        public decimal ROERate { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Status { get; set; } = true;

        [DisplayName("Currency Name")]
        public string CurrencyName { get; set; } 
        public List<decimal> ROEinsertedvalue { get; set; }
        public List<int> Currencyinsertedvalue { get; set; }
        public bool IsDeleted { get; set; }

        public List<int> ROENameinsertedvalue { get; set; }

    }
}

