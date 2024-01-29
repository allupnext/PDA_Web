using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class UserRolePermissions
    {
        public int UserRolePermissionId { get;set; }
        public int UserRolePermissionTypeID { get; set; }
        public string UserRolePermission { get; set; }
        public int UserRolePermissionMenuId { get; set; }

        public bool IsPermission { get; set; } = false;

        public int RoleID { get; set; }

    }
}
