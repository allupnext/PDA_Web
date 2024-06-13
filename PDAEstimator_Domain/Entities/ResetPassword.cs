using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDAEstimator_Domain.Entities
{
    public class ResetPassword
    {
        public int userId { get; set; }
        public string Email { get; set; }
        public string EmailID { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password and ConfirmPassword must be Match")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }

        public int Error { get; set; } = 0;

        public string? MacAddress { get; set; } 

    }
}
