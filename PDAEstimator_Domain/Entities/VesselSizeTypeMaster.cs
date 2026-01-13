using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class VesselSizeTypeMaster
    {
        public long VesselSizeTypeId { get; set; }
        public string VesselSizeTypeName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int? DWT { get; set; }

        public int? GRT { get; set; }
        public int? NRT { get; set; }
        public decimal? LOA { get; set; }
        public decimal? Beam { get; set; }
        public bool IsDeleted { get; set; }
    }
}
