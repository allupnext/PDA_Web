using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class FormulaOprator
    {

        public int formulaOperatorID { get; set; }
        public string formulaOperator { get; set; }

        public bool formulaOpreratorstatus { get; set; } = true;
        public bool IsDeleted { get; set; }

    }
}
