using System.ComponentModel.DataAnnotations;

namespace Travel_Agency___Data.ViewModels
{


public class PasswordOperationsViewModel
{
  [Required]
  [EmailAddress]
   public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string? CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]

    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }

    [Required]
    public string? ResetToken { get; set; }
}

}
