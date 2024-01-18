using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Notes : AuditableEntity
    {
        public string Note { get; set; } =string.Empty;
        public int sequnce { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; }

    }
}
