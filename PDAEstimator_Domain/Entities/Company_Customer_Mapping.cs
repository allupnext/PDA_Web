using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Company_Customer_Mapping
    {
        public int CompanyID { get; set; }
        public int CustomerID { get; set; }
        public bool IsPrimary { get; set; }
    }
}
