using System;
using System.Threading.Tasks;
using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.Pages;
using ClinicManagement.Web.Tests.Helpers;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages
{
    public class SignUpModelTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<SignUpModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public SignUpModelTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<SignUpModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private SignUpModel CreateModel()
        {
            var model = new SignUpModel(_authServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var model = CreateModel();
            Assert.NotNull(model);
        }

        [Fact]
        public void Constructor_InitializesDefaultProperties()
        {
            var model = CreateModel();
            Assert.NotNull(model.LoginInput);
            Assert.NotNull(model.RegisterInput);
            Assert.Equal(string.Empty, model.LoginMessage);
            Assert.Equal(string.Empty, model.RegisterMessage);
            Assert.False(model.RegisterSuccess);
        }

        [Fact]
        public void OnGet_ClearsSession()
        {
            var model = CreateModel();
            model.OnGet();
            _sessionMock.Verify(s => s.Clear(), Times.Once);
        }

        [Fact]
        public async Task OnPostLoginAsync_InvalidModelState_ReturnsPageWithMessage()
        {
            var model = CreateModel();
            model.ModelState.AddModelError("Email", "Required");
            var result = await model.OnPostLoginAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("Please fill in all required fields.", model.LoginMessage);
        }

        [Fact]
        public async Task OnPostLoginAsync_ValidCredentials_PatientUser_RedirectsToPatientHome()
        {
            _authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(new LoginResultDto { Success = true, UserId = 1, UserType = 1 });
            var model = CreateModel();
            model.LoginInput = new LoginViewModel { Email = "patient@test.com", Password = "pass123" };
            var result = await model.OnPostLoginAsync();
            Assert.Equal("/Patient/PatientHome", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnPostLoginAsync_ValidCredentials_DoctorUser_RedirectsToDoctorHome()
        {
            _authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(new LoginResultDto { Success = true, UserId = 2, UserType = 2 });
            var model = CreateModel();
            model.LoginInput = new LoginViewModel { Email = "doctor@test.com", Password = "pass123" };
            var result = await model.OnPostLoginAsync();
            Assert.Equal("/Doctor/DoctorHome", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnPostLoginAsync_ValidCredentials_AdminUser_RedirectsToAdminHome()
        {
            _authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(new LoginResultDto { Success = true, UserId = 3, UserType = 3 });
            var model = CreateModel();
            model.LoginInput = new LoginViewModel { Email = "admin@test.com", Password = "pass123" };
            var result = await model.OnPostLoginAsync();
            Assert.Equal("/Admin/AdminHome", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnPostLoginAsync_InvalidCredentials_ReturnsPageWithMessage()
        {
            _authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(new LoginResultDto { Success = false, Message = "Invalid credentials" });
            var model = CreateModel();
            model.LoginInput = new LoginViewModel { Email = "wrong@test.com", Password = "wrong" };
            var result = await model.OnPostLoginAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("Invalid credentials", model.LoginMessage);
        }

        [Fact]
        public async Task OnPostLoginAsync_ServiceThrowsException_ReturnsPageWithErrorMessage()
        {
            _authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            model.LoginInput = new LoginViewModel { Email = "test@test.com", Password = "pass" };
            var result = await model.OnPostLoginAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("There was some error. Try Again!", model.LoginMessage);
        }

        [Fact]
        public async Task OnPostLoginAsync_UnknownUserType_ReturnsPage()
        {
            _authServiceMock.Setup(s => s.ValidateLoginAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .ReturnsAsync(new LoginResultDto { Success = true, UserId = 99, UserType = 99 });
            var model = CreateModel();
            model.LoginInput = new LoginViewModel { Email = "unknown@test.com", Password = "pass" };
            var result = await model.OnPostLoginAsync();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostRegisterAsync_InvalidModelState_ReturnsPageWithMessage()
        {
            var model = CreateModel();
            model.ModelState.AddModelError("Name", "Required");
            var result = await model.OnPostRegisterAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("Please fill in all required fields correctly.", model.RegisterMessage);
        }

        [Fact]
        public async Task OnPostRegisterAsync_SuccessfulRegistration_RedirectsToPatientHome()
        {
            _authServiceMock.Setup(s => s.RegisterPatientAsync(It.IsAny<PatientRegisterDto>(), default))
                .ReturnsAsync(new RegisterResultDto { Success = true, PatientId = 10 });
            var model = CreateModel();
            model.RegisterInput = new RegisterViewModel { Name = "New Patient", BirthDate = "1990-01-01", Email = "new@test.com", Password = "pass123", PhoneNo = "1234567890", Gender = "Male", Address = "123 Main St" };
            var result = await model.OnPostRegisterAsync();
            Assert.Equal("/Patient/PatientHome", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnPostRegisterAsync_FailedRegistration_ReturnsPageWithMessage()
        {
            _authServiceMock.Setup(s => s.RegisterPatientAsync(It.IsAny<PatientRegisterDto>(), default))
                .ReturnsAsync(new RegisterResultDto { Success = false, Message = "Email already exists" });
            var model = CreateModel();
            model.RegisterInput = new RegisterViewModel { Name = "Existing", BirthDate = "1990-01-01", Email = "existing@test.com", Password = "pass123", PhoneNo = "1234567890", Gender = "Male", Address = "123 Main St" };
            var result = await model.OnPostRegisterAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("Email already exists", model.RegisterMessage);
            Assert.False(model.RegisterSuccess);
        }

        [Fact]
        public async Task OnPostRegisterAsync_ServiceThrowsException_ReturnsPageWithErrorMessage()
        {
            _authServiceMock.Setup(s => s.RegisterPatientAsync(It.IsAny<PatientRegisterDto>(), default))
                .ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            model.RegisterInput = new RegisterViewModel { Name = "Test", BirthDate = "1990-01-01", Email = "test@test.com", Password = "pass123", PhoneNo = "1234567890", Gender = "Male", Address = "123 Main St" };
            var result = await model.OnPostRegisterAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("There was some error. Try again!", model.RegisterMessage);
            Assert.False(model.RegisterSuccess);
        }

        [Fact]
        public async Task OnPostRegisterAsync_MapsRegisterInputToDto()
        {
            PatientRegisterDto? capturedDto = null;
            _authServiceMock.Setup(s => s.RegisterPatientAsync(It.IsAny<PatientRegisterDto>(), default))
                .Callback<PatientRegisterDto, System.Threading.CancellationToken>((dto, _) => capturedDto = dto)
                .ReturnsAsync(new RegisterResultDto { Success = true, PatientId = 5 });
            var model = CreateModel();
            model.RegisterInput = new RegisterViewModel { Name = "Alice", BirthDate = "1995-06-15", Email = "alice@test.com", Password = "secret123", PhoneNo = "9876543210", Gender = "Female", Address = "456 Oak Ave" };
            await model.OnPostRegisterAsync();
            Assert.NotNull(capturedDto);
            Assert.Equal("Alice", capturedDto!.Name);
            Assert.Equal("alice@test.com", capturedDto.Email);
            Assert.Equal("Female", capturedDto.Gender);
        }
    }
}
