using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels;

public class CreateViewModel
{

    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),ErrorMessage = "Password and Confirm Password do not match")]
    public string ConfrimPassword { get; set; } = string.Empty;
}