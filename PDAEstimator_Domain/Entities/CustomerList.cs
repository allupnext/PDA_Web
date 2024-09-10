using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CustomerList
    {
        public int CustomerId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string Address1 { get; set; }
        public string Company { get; set; }

        public string Address2 { get; set; }
        public int? City { get; set; }
        public string CityName { get; set; }
        public string? StateName { get; set; }
        public string CountryName { get; set; }

        public int State { get; set; }
        public int Country { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }

        public string CountryCode { get; set; }
        public string AlternativeEmail { get; set; }
        public bool IsEmailNotification { get; set; }
        public string PrimaryCompany { get; set; }
        public string SecondaryCompany { get; set; }

        public int BankId { get; set; }
        public string Beneficiary_Bank_Name { get; set; }
        public string? Bank_Code { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifyBy { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
