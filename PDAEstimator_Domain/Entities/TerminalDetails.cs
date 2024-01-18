using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class TerminalDetails : AuditableEntity
    {
        [Required]
        [StringLength(50)]
        [DisplayName("Terminal Code")]
        public string TerminalCode { get; set; }
        
        [StringLength(200)]
        [DisplayName("Terminal Name")]
        public string TerminalName { get; set; }
     
        [DisplayName("Status")]
        public bool? Status { get; set; }

        public int? PortID { get; set; }
        public bool IsDeleted { get; set; }

        public string? PortName { get; set; }

    }
}
