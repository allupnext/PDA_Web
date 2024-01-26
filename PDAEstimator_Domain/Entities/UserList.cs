using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class UserList : AuditableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployCode { get; set; }
        public string UserPassword { get; set; }
        public string ConfirmPasword { get; set; }

        public long MobileNo { get; set; }
        public string Salutation { get; set; }

        public string EmailID { get; set; }

        public DateTime DOB { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public bool IsAdmin { get; set; } = true;
        public DateTime CreationDate { get; set; }
        public string IPAddress { get; set; }
        public bool Status { get; set; } = true;
        public Int64 ModifyUserID { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool IsDeleted { get; set; }

        public string PrimaryCompany { get; set; }
        public string SecondaryCompany { get; set; }
        public string Ports { get; set; }

    }
}
