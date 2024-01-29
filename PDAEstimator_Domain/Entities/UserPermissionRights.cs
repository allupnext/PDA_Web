using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class UserPermissionRights
    {
        public int RoleID { get; set; }
        public int UserRolePermissionId { get; set; }
        public int IsPermission { get; set; }
        public string RoleName { get; set; }
        public string MenuName { get; set; }
        public string UserRolePermission { get; set; }
    }
}
