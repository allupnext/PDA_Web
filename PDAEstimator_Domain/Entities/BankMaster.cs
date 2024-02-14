using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class BankMaster
    {
        public Int64 BankId { get; set; }
        public int CompanyId { get; set; }
        public string NameofBeneficiary { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string AccountNo { get; set; }
        public string Beneficiary_Bank_Name { get; set; }
        public string Beneficiary_Bank_Address { get; set; }
        public string Beneficiary_RTGS_NEFT_IFSC_Code { get; set; }
        public string Beneficiary_Bank_Swift_Code { get; set; }
        public string Intermediary_Bank { get; set; }
        public string Intermediary_Bank_Swift_Code { get; set; }
        public string Bank_Code { get; set; }
        public bool IsDefault { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }




    }
}
