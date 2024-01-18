using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class CityList : AuditableEntity
    {
        public int StateId { get; set; }

        public string CityName { get; set; }
        public bool IsDeleted { get; set; }

    }
}
