using System.ComponentModel.DataAnnotations;

namespace ClaimsBasesAuthorizationApplication.Models;

public class User
{
    [Key] public long Id { get; set; }
    [MaxLength(255)] public string Email { get; set; } = null!;
    [MaxLength(255)] public string HashedPassword { get; set; } = null!;
}