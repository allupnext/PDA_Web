using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CompanyMaster
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLog { get; set; } 
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public string AlterTel { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
        public string CountryCode { get; set; }

    }
}
