using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class FormularTransList
    {
        public int formulaTrasID { get; set; }
        public int formulaID { get; set; }

        public int? PortID { get; set; }

        public int PortName { get; set; }

        public int formulaAttributeID { get; set; }
        public string formulaAttributeName { get; set; }

        public int formulaSlabID { get; set; }
        public string formulaSlabName { get; set; }

        public int formulaOperatorID { get; set; }
        public string formulaOperatorName { get; set; }
        public decimal formulaValue { get; set; }
        public bool IsDeleted { get; set; }

    }
}
