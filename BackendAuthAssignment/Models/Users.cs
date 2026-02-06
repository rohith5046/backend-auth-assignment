using System.ComponentModel.DataAnnotations;

namespace BackendAuthAssignment.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(20)]
    public string PhoneNumber { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    // Registration gating
    public bool IsBasicRegistrationComplete { get; set; } = false;

    public UserProfile? Profile { get; set; }
}
