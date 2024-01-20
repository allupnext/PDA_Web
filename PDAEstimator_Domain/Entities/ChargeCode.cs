using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class ChargeCode : AuditableEntity
    {
        public string ChargeCodeName { get; set; } = string.Empty;
        public int ExpenseCategoryID { get; set; } 
      
        public bool Status { get; set; } = true;
        public string Createdby { get; set; }
        public DateTime CreationDate { get; set; }
        public string lastmodifiedby { get; set; }
        public DateTime? lastmodifiedon { get; set; }
        public string IPAddress { get; set; }
        public bool IsDeleted { get; set; }
        public int Sequence { get; set; }


    }
}
