using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class ROENames : AuditableEntity
    {
        public string ROEName { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        
        public bool IsDefault { get; set; } = false;

    }
}
