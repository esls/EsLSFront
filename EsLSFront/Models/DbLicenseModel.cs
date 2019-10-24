using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EsLSFront.Models
{
    public class DbLicenseModel
    {
        [Required]
        public int Id { get; set; }

        [DataType(DataType.Date, ErrorMessage = "A valid date must be entered")]
        [Required]
        public DateTime CreationDate { get; set; }

        [StringLength(64, ErrorMessage = "Full name must not be longer than 64 characters")]
        [Required]
        public string FullName { get; set; }

        [StringLength(64, ErrorMessage = "E-mail must not be longer than 64 characters")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "License keys must be 10 characters long")]
        [Required]
        public string LicenseKey { get; set; }

        [DataType(DataType.Currency)]
        [Required]
        public decimal Price { get; set; }
    }
}
