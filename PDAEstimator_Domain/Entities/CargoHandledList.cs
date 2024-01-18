using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CargoHandledList  : AuditableEntity
    {

        public string TerminalName { get; set; } = string.Empty;
        public int TerminalID { get; set; }

        public int BerthID { get; set; }
        public string BerthName { get; set; } = string.Empty;

        public int PortID { get; set; }
        public string PortName { get; set; } = string.Empty;

        public int CargoID { get; set; }
        public string CargoName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public bool CargoStatus { get; set; } = true;
    }
}
