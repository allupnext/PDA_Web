using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Designation
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public bool IsDeleted { get; set; }

    }
}
