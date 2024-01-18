using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class FormulaAttributes
    {
        public int FormulaId { get; set; }
        public string FormulaName    { get; set; }
       
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }

    }
}
