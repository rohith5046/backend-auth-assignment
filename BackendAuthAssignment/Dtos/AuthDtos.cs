using System.Text.Json;

namespace BackendAuthAssignment.Dtos;

public record RequestOtpDto(string PhoneNumber);

public record VerifyOtpDto(string PhoneNumber, string Otp);

public record RefreshDto(string RefreshToken);

public record LogoutDto(string RefreshToken);

public record AuthSuccessResponse(
    string AccessToken,
    string RefreshToken,
    bool IsBasicRegistrationComplete
);

public record RegisterBasicDto(
    string FullName,
    string DateOfBirth,   // "YYYY-MM-DD"
    string Email,
    JsonElement Location  // free-form JSON
);
