using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Requiredfield")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }

        [Required(ErrorMessage = "Requiredfield")]
        public string Password { set; get; }
    }

    public class RecoveryModel
    {
        [Required(ErrorMessage = "Requiredfield")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }

        [Required(ErrorMessage = "Requiredfield")]
        public string Password { set; get; }

        [Required(ErrorMessage = "Requiredfield")]
        [Display(Name = "Enter the number from the image")]
        public string Captcha { get; set; }
        public bool IsWeb { get; set; }
    }
}
