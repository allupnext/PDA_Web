using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class TariffMasterList
    {
        public int TariffID { get; set; }
        public int PortID { get; set; }
        public string PortName { get; set; }

        public int TariffCount { get; set; }


        public bool IsDeleted { get; set; }

    }
}
