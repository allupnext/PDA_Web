using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PDAEstimator_Domain.Entities
{
    public class User : AuditableEntity
    {

        public string FirstName { get; set; }
        public string? SearchedName { get; set; }
        public string LastName { get; set; }
        public string EmployCode { get; set; }
        public string UserPassword { get; set; }
        public string ConfirmPasword { get; set; }

        //public int MobileNo { get; set; }
        public long MobileNo { get; set; }


        public string Salutation { get; set; }

        public string EmailID { get; set; }

        public DateTime DOB { get; set; }
        public int RoleId { get; set; }
        public bool IsAdmin { get; set; } = true;
        public DateTime CreationDate { get; set; }
        public string IPAddress { get; set; }
        public bool Status { get; set; } = true;
        public Int64 ModifyUserID { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsDeleted { get; set; }

        public string? PrimaryCompany { get; set; }
        public string? SecondaryCompany { get; set; }
        //public string Ports { get; set; }
        public int? PrimaryCompanyId { get; set; }
       // public int? CompanyId { get; set; }
        public int[]? SecondaryCompanyId { get; set; }
        public string? Ports { get; set; }
        public int[]? PortIds { get; set; }

        public string SelectedPortIds { get; set; }

        public string? RoleName { get; set; }
        public string? Token { get; set; }
        public string MacAddress { get; set; }

        public string? OTP { get; set; }
        public DateTime? OTPSentDate { get; set; }

        public string? LoginMachineName { get; set; }

        public DateTime? LoginDateTime { get; set; }
    }
}
