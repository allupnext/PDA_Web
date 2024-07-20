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
        public decimal? MaxLoa { get; set; }
        public decimal? MaxBeam { get; set; }
        public decimal? MaxArrivalDraft { get; set; }
        public string? TerminalName { get; set; }

    }
}
