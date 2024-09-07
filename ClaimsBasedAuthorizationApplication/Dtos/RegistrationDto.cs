using System.ComponentModel.DataAnnotations;

namespace ClaimsBasesAuthorizationApplication.Dtos;

public class RegistrationDto
{
    [Required] public string Email { get; set; } = null!;

    [Required]
    [Compare(nameof(ConfirmPassword))]
    public string Password { get; set; } = null!;

    [Required] [Compare(nameof(Password))] public string ConfirmPassword { get; set; } = null!;
}