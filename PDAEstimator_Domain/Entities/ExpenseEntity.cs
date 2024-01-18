using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    
        public class Expense : AuditableEntity
        {
        public bool Status { get; set; } = true;
        public int sequnce { get; set; }

        public string ExpenseName { get; set; }
        public bool IsDeleted { get; set; }

    }


}
