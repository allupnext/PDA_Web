using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Company_User_Mapping
    {
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public bool IsPrimary { get; set; }
    }
}
