using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for authentication operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ILoginRepository _loginRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ILoginRepository loginRepository,
        IPatientRepository patientRepository,
        ILogger<AuthService> logger)
    {
        _loginRepository = loginRepository;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Validates user login credentials.
    /// </summary>
    public async Task<LoginResultDto> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", email);

            var login = await _loginRepository.ValidateLoginAsync(email, password, cancellationToken);

            if (login == null)
            {
                // Check if email exists
                var emailExists = await _loginRepository.EmailExistsAsync(email, cancellationToken);
                if (!emailExists)
                {
                    return new LoginResultDto { Success = false, Message = "Email not found. Try Again!" };
                }
                return new LoginResultDto { Success = false, Message = "Incorrect Password. Try Again!" };
            }

            _logger.LogInformation("Login successful for user ID: {UserId}, Type: {UserType}", login.LoginId, login.Type);

            return new LoginResultDto
            {
                Success = true,
                UserId = login.LoginId,
                UserType = login.Type,
                Message = "Login successful"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", email);
            return new LoginResultDto { Success = false, Message = "There was some error. Try Again!" };
        }
    }

    /// <summary>
    /// Registers a new patient.
    /// </summary>
    public async Task<RegisterResultDto> RegisterPatientAsync(PatientRegisterDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Patient registration attempt for email: {Email}", dto.Email);

            // Check if email already exists
            var emailExists = await _loginRepository.EmailExistsAsync(dto.Email, cancellationToken);
            if (emailExists)
            {
                return new RegisterResultDto { Success = false, Message = "Email already exists. Please choose a different one." };
            }

            // Create login entry
            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 1 // Patient
            };

            var loginId = await _loginRepository.AddAsync(login, cancellationToken);

            // Create patient entry
            var patient = new Patient
            {
                PatientId = loginId,
                Name = dto.Name,
                Phone = dto.PhoneNo,
                Address = dto.Address,
                BirthDate = DateTime.Parse(dto.BirthDate),
                Gender = string.IsNullOrEmpty(dto.Gender) ? 'M' : dto.Gender[0]
            };

            await _patientRepository.AddAsync(patient, cancellationToken);

            _logger.LogInformation("Patient registered successfully with ID: {PatientId}", loginId);

            return new RegisterResultDto
            {
                Success = true,
                PatientId = loginId,
                Message = "Registration Successful!"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration for email: {Email}", dto.Email);
            return new RegisterResultDto { Success = false, Message = "There was some error. Try again!" };
        }
    }
}
