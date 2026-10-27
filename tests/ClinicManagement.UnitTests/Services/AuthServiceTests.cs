using ClinicManagement.Domain.DTOs;
using Xunit;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicManagement.UnitTests.Services;

/// <summary>
/// Unit tests for AuthService.
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<ILoginRepository> _loginRepositoryMock;
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _loginRepositoryMock = new Mock<ILoginRepository>();
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _authService = new AuthService(_loginRepositoryMock.Object, _patientRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var login = new LoginTable { LoginId = 1, Email = "test@test.com", Password = "password", Type = 1 };
        _loginRepositoryMock.Setup(r => r.ValidateLoginAsync("test@test.com", "password", default))
            .ReturnsAsync(login);

        // Act
        var result = await _authService.ValidateLoginAsync("test@test.com", "password");

        // Assert
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(1);
        result.UserType.Should().Be(1);
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidEmail_ReturnsEmailNotFound()
    {
        // Arrange
        _loginRepositoryMock.Setup(r => r.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync((LoginTable?)null);
        _loginRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.ValidateLoginAsync("notfound@test.com", "password");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Email not found");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithWrongPassword_ReturnsIncorrectPassword()
    {
        // Arrange
        _loginRepositoryMock.Setup(r => r.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync((LoginTable?)null);
        _loginRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), default))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.ValidateLoginAsync("test@test.com", "wrongpassword");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Incorrect Password");
    }

    [Fact]
    public async Task RegisterPatientAsync_WithNewEmail_ReturnsSuccess()
    {
        // Arrange
        _loginRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);
        _loginRepositoryMock.Setup(r => r.AddAsync(It.IsAny<LoginTable>(), default))
            .ReturnsAsync(1);
        _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>(), default))
            .Returns(Task.CompletedTask);

        var dto = new PatientRegisterDto
        {
            Name = "John Doe",
            BirthDate = "1990-01-01",
            Email = "john@test.com",
            Password = "password123",
            PhoneNo = "1234567890",
            Gender = "M",
            Address = "123 Main St"
        };

        // Act
        var result = await _authService.RegisterPatientAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.PatientId.Should().Be(1);
    }

    [Fact]
    public async Task RegisterPatientAsync_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        _loginRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), default))
            .ReturnsAsync(true);

        var dto = new PatientRegisterDto { Email = "existing@test.com" };

        // Act
        var result = await _authService.RegisterPatientAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Email already exists");
    }
}
