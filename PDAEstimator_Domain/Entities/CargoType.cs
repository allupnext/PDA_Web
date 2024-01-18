using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CargoType : AuditableEntity
    {
        public string CargoTypeName { get; set; }
        public bool CargoTypeStatus { get; set; } = false;
        public bool IsDeleted { get; set; }

    }
}
