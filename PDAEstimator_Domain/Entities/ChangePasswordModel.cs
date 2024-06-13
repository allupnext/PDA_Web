using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class ChangePasswordModel
    {
        public int userId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "NewPassword and ConfirmPassword must be Match")]
        public string ConfirmPassword { get; set; }

        public int Changed { get; set; }
        public string MacAddress { get; set; }
    }
}
