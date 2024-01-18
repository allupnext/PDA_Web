using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Country : AuditableEntity
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public bool IsDeleted { get; set; }

    }
}
