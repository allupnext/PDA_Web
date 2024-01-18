using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CargoFamily : AuditableEntity
    {
        public int CargoTypeID { get; set; }

        public string CargoFamilyName { get; set; }
        public bool CargoFamilyStatus { get; set; } = true;
        public bool IsDeleted { get; set; }

    }
}
