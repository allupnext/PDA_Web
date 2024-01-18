using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class FormulaTransaction
    {
        public int formulaTrasID { get; set; }
        public int formulaID { get; set; }
        public int formulaAttributeID { get; set; }
        public int formulaSlabID { get; set; }
        public int formulaOperatorID { get; set; }
        public decimal formulaValue { get; set; }
        public bool IsDeleted { get; set; }


    }
}
