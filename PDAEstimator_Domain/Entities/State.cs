using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class State : AuditableEntity
    {
        public string StateName { get; set; }
        public int CountryId { get ; set; }
        public bool IsDeleted { get; set; }

    }
}
