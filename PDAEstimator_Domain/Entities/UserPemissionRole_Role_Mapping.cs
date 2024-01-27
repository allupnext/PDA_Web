using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class UserPemissionRole_Role_Mapping
    {
        public int RoleID { get;set; }
        public int UserRolePermissionId { get;set; }
        public bool IsPermission { get; set; } = false;
    }
}
