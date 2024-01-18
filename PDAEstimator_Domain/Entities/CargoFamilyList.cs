using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CargoFamilyList:AuditableEntity
    {
        public int CargoTypeID { get; set; }
        public string CargoTypeName { get; set; } = string.Empty;

        public string CargoFamilyName { get; set; } = string.Empty;
        public bool CargoFamilyStatus { get; set; } = true;
        public bool IsDeleted { get; set; }

    }
}
