using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class AdminDashboard
    {
        public List<PDAEstimatorList> pDAEstimatorLists { get; set; }

        public List<CustomerList> customerList { get; set; }
    }
}
