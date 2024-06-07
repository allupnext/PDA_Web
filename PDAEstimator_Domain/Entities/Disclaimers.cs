using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Disclaimers : AuditableEntity
    {
        public string Disclaimer { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; }
    }
}
