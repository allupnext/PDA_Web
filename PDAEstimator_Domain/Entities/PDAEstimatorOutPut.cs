using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class PDAEstimatorOutPut
    {
        public long PDAEstimatorOutPutID { get; set; }
        public long PDAEstimatorID { get; set; }
        public DateTime PDAEstimatorOutPutDate { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyAddress1 { get; set; }
        public string CompanyAddress2 { get; set; } 
       
        public string CompanyTelephone { get; set; }
        public string CompanyAlterTel { get ; set; }    
        public string CompanyEmail { get; set; }
        public string NameofBeneficiary { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string AccountNo { get; set; }
        public string Beneficiary_Bank_Name { get; set; }
        public string Beneficiary_Bank_Address { get; set; }
        public string Beneficiary_RTGS_NEFT_IFSC_Code { get; set; }
        public string Beneficiary_Bank_Swift_Code { get; set; }
        public string Intermediary_Bank { get; set; }
        public string Intermediary_Bank_Swift_Code { get; set; }
        public string BaseCurrencyCode { get; set; }
        public string DefaultCurrencyCode { get; set; }
        public decimal Taxrate { get; set; }

        public int BaseCurrencyCodeID { get; set; }
        public int DefaultCurrencyCodeID { get; set; }
        public string? Disclaimer { get; set; }
    }
}
