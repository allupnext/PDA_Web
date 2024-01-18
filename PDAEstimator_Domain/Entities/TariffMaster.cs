using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class TariffMaster
    {
        public int TariffID { get; set; }
        public int PortID { get; set; }
      

        public int? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? IPAddress { get; set; }
        public int? ModifyUserID { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
