using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class UserAuth
    {
        public string EmployCode { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
    }
}
