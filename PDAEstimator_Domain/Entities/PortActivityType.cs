using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class PortActivityType : AuditableEntity
    {
        public string ActivityType { get; set; } = string.Empty;
        public bool Status { get; set; } = false;


    }
}
