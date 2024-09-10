using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class EmailNotificationConfiguration
    {
        public long EmailConfigID { get; set; } 
        public string ProcessName { get; set; }
        public int CompanyId { get; set; }
        public string CompneyName { get; set; }
        public string FromEmail { get ; set; }
        public string ToEmail { get; set; } 
    }
}
