using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CargoDetailsList : AuditableEntity
    {
        public int? CargoTypeID { get; set; }
        public string CargoTypeName { get; set; } = string.Empty;
        public string CargoName { get; set; } = string.Empty;
        public bool CargoStatus { get; set; } = true;
        public string CargoFile { get; set; } = string.Empty;
        public string CargoFamilyName { get; set; } = string.Empty;
        public int? CargoFamilyID { get; set; }
        public bool IsDeleted { get; set; }

    }
}
