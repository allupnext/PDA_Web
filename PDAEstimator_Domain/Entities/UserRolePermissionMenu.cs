using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class UserRolePermissionMenu
    {
        public int UserRolePermissionMenuId { get; set; }
        public string MenuName { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
