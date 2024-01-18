using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CallType : AuditableEntity
    {
        public string CallTypeName { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public bool IsDeleted { get; set; }


    }
}
