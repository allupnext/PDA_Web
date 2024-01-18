using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class Roles : AuditableEntity
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        //public string Description { get; set; }
        //public bool Active { get; set; }
        //public DateTime CreationDate { get; set; }
        //public string IPAddress { get; set; }
        //public Int64 ModifyUserID { get; set; }
        //public DateTime ModifyDate { get; set; }
        public bool Status { get; set; } = true;
        public bool IsDeleted { get; set; }

    }

}
