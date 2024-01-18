using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class FormulaList
    {
        public int formulaMasterID { get; set; }
        public string formulaName { get; set; }
        public bool formulaStatus { get; set; } = true;
        public string PortName { get; set; }
        public int formulaAttributeID { get; set; }
        public int formulaSlabID { get; set; }
        public int formulaOperatorID { get; set; }
        public string formulaValue { get; set; }
        public string formula { get; set; }
        public bool IsDeleted { get; set; }

    }
}
