using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class PortDetails : AuditableEntity
    {
        [Required]
        [StringLength(50)]
        [DisplayName("Port Code")]
        public string PortCode { get; set; }
        [StringLength(200)]
        [DisplayName("Port Name")]
        public string PortName { get; set; }
        [DisplayName("City")]
        public int? City { get; set; }
        [DisplayName("State")]
        public int? State { get; set; }
        [DisplayName("Country")]
        public int? Country { get; set; }
        [DisplayName("Status")]
        public bool? Status { get; set; } = true;

        [StringLength(500)]
        [DisplayName("Port File")]
        public string? PortFile { get; set; }

        [StringLength(500)]
        [DisplayName("Port File")]
        public string? PortFileTanker { get; set; }
        public bool IsDeleted { get; set; }

    }
}
