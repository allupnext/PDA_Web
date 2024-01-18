using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class ExpenseList : AuditableEntity
    {
        public int CargoID { get; set; }
        public string CargoName { get; set; }
        public int sequnce { get; set; }

        public string ExpenseName { get; set; }
        public bool IsDeleted { get; set; }


    }
}
