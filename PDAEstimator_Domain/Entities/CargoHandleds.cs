using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CargoHandleds : AuditableEntity
    {
        public int TerminalID { get; set; }
        public int BerthID { get; set; }
        public int PortID { get; set; }
        public int CargoID { get; set; }

        public bool CargoStatus { get; set; } = true;
        public bool IsDeleted { get; set; }

        public int[]? CargoIDs { get; set; }

    }
}
