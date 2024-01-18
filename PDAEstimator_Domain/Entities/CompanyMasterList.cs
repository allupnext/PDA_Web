using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CompanyMasterList
    {

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLog { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int Country { get; set; }
        public int State { get; set; }
        public int City { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string Telephone { get; set; }
        public string AlterTel { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
