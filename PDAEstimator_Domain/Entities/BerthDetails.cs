using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class BerthDetails : AuditableEntity
    {
        [Required]
        [StringLength(50)]
        [DisplayName("Berth Code")]
        public string BerthCode { get; set; }

        [StringLength(200)]
        [DisplayName("Berth Name")]
        public string BerthName { get; set; }

        [DisplayName("Status")]
        public bool? BerthStatus { get; set; }

        [DisplayName("Terminal")]
        public int? TerminalID { get; set; }
        public bool IsDeleted { get; set; }
        [DisplayName("Max Loa")]
        public decimal? MaxLoa { get; set; }
        [DisplayName("Max Beam")]
        public decimal? MaxBeam { get; set; }
        [DisplayName("Max Arrival Draft")]
        public decimal? MaxArrivalDraft { get; set; }
        [DisplayName("Max DWT")]
        public decimal? DWT { get; set; }
        [DisplayName("Max Displacement ")]
        public decimal? MaxDisplacement { get; set; }
        [DisplayName("Terminal Name")]
        public string? TerminalName { get; set; }

    }
}
