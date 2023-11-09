using System.ComponentModel.DataAnnotations;

namespace IdentityApp.ViewModels;

public class EditViewModel
{
    public string? Id { get; set; }
    public string? FullName { get; set; }

    [EmailAddress]
    public string? Email { get; set; } 

    [DataType(DataType.Password)]
    [StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
    public string? Password { get; set; } 

    [DataType(DataType.Password)]
    [Compare(nameof(Password),ErrorMessage = "Password and Confirm Password do not match")]
    public string? ConfrimPassword { get; set; }
}