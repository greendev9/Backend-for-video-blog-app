using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.Auth
{
  public class CreateAccount
  {

    //[Remote("IsNickNameExists", "Registration", ErrorMessage = null, ErrorMessageResourceName = "NicknameExists", ErrorMessageResourceType = typeof(Resources.Resources))]

    [Required(ErrorMessage = "Requiredfield")]
    [StringLength(12, MinimumLength = 6, ErrorMessage = "NicknameLengh")]
    [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "EnglishCharsOnly")]
    //[RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "EnglishCharsOnly")]
    public string NickName { set; get; }

    public Gender Gender { set; get; }

    [EmailAddress(ErrorMessage = "Invalidemail")]
    [Required(ErrorMessage = "Requiredfield")]
    public string Email { set; get; }

    [Range(1, int.MaxValue, ErrorMessage = "Requiredfield")]
    [Required(ErrorMessage = "Requiredfield")]
    public int YearOfBirth { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*\]\[_+-]{8,20}$", ErrorMessage = "PasswordEnglish8Chars")]
    //[RegularExpression(@"^[a-zA-Z0-9-~!@#$%^&*_+=]{8,20}$", ErrorMessage = "PasswordEnglish8Chars")]
    [Required(ErrorMessage = "Requiredfield")]
    public string Password { get; set; }

    //[Required(ErrorMessage = "Requiredfield")]
    //[Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
    //public string ConfirmPassword { get; set; }

    [Range(typeof(bool), "true", "true", ErrorMessage = "TermsAccept")]
    public bool TermsAndConditions { get; set; }

    [Required(ErrorMessage = "Requiredfield")]
    public int? CountryId { get; set; }

    [Required(ErrorMessage = "Requiredfield")]
    public int? LanguageId { get; set; }

    public int? ItemColorID { get; set; }

    public int? Application { get; set; }


  }
}