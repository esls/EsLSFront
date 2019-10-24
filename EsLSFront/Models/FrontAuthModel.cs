using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EsLSFront.Models
{
    public class FrontAuthModel
    {
        [StringLength(16, ErrorMessage = "The user name should not be longer than 16 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "The user name can contain alphanumeric characters and underscores only")]
        [Required]
        public string Username { get; set; }

        [RegularExpression(@"^(?=\D*\d)[a-zA-Z0-9]{8,}$", ErrorMessage = "The password must be at least 8 characters long and contain at least one of each: lowercase character, uppercase character, digit")]
        [Required]
        public string Password { get; set; }
    }
}
