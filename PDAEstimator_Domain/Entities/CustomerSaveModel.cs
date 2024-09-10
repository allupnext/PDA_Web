using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CustomerSaveModel
    {

        public int CustomerId { get; set; }
        public string Company { get; set; }
        public string PrimaryCompany { get; set; }
        public string SecondaryCompany { get; set; }
        public int? PrimaryCompanyId { get; set; }
        public string Beneficiary_Bank_Name { get; set; }
        public string? Bank_Code { get; set; }
        public int BankID { get; set; }
        public int[]? SecondaryCompanyId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
