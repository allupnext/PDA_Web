using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class FormulaMaster
    {
        public int formulaMasterID { get; set; }
        public string formulaName { get; set; } = string.Empty;

        public bool formulaStatus { get; set; } = true;
        public bool IsDeleted { get; set; }

        public int? PortID { get; set; }

    }
}
