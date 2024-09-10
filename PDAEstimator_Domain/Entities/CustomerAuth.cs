using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CustomerAuth
    {
        public string Email { get; set; } = string.Empty;
        public string CustomerPassword { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }
}
