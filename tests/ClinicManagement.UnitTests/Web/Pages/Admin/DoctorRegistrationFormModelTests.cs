using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.Pages.Admin;
using ClinicManagement.Web.Tests.Helpers;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages.Admin
{
    public class DoctorRegistrationFormModelTests
    {
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<ILogger<DoctorRegistrationFormModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public DoctorRegistrationFormModelTests()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _loggerMock = new Mock<ILogger<DoctorRegistrationFormModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private DoctorRegistrationFormModel CreateModel()
        {
            var model = new DoctorRegistrationFormModel(_adminServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        private void SetupSession(int? userType)
        {
            if (userType.HasValue)
                SessionMockHelper.SetupInt32(_sessionMock, "UserType", userType.Value);
            else
                SessionMockHelper.SetupMissingKey(_sessionMock, "UserType");
        }

        private void SetupDepartments()
        {
            _adminServiceMock.Setup(s => s.GetDepartmentsAsync(default))
                .ReturnsAsync(new List<DepartmentDto>
                {
                    new DepartmentDto { DeptNo = 1, DeptName = "Cardiology" },
                    new DepartmentDto { DeptNo = 2, DeptName = "Neurology" }
                });
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
            Assert.NotNull(model.DoctorInput);
            Assert.Equal(string.Empty, model.Message);
            Assert.False(model.Success);
        }

        [Fact]
        public async Task OnGetAsync_NonAdminUser_RedirectsToSignUp()
        {
            SetupSession(1);
            var model = CreateModel();

            var result = await model.OnGetAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_AdminUser_ReturnsPageWithDepartments()
        {
            SetupSession(3);
            SetupDepartments();

            var model = CreateModel();
            var result = await model.OnGetAsync();

            Assert.IsType<PageResult>(result);
            var depts = new List<DepartmentOptionViewModel>(model.DoctorInput.Departments);
            Assert.Equal(2, depts.Count);
        }

        [Fact]
        public async Task OnPostAsync_NonAdminUser_RedirectsToSignUp()
        {
            SetupSession(1);
            SetupDepartments();
            var model = CreateModel();

            var result = await model.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_InvalidModelState_ReturnsPageWithMessage()
        {
            SetupSession(3);
            SetupDepartments();
            var model = CreateModel();
            model.ModelState.AddModelError("Name", "Required");

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("Please fill in all required fields.", model.Message);
        }

        [Fact]
        public async Task OnPostAsync_EmailAlreadyExists_ReturnsPageWithMessage()
        {
            SetupSession(3);
            SetupDepartments();
            _adminServiceMock.Setup(s => s.DoctorEmailExistsAsync(It.IsAny<string>(), default))
                .ReturnsAsync(true);

            var model = CreateModel();
            model.DoctorInput = new AddDoctorViewModel
            {
                Name = "Dr. Test",
                Email = "existing@test.com",
                Password = "pass123",
                BirthDate = "1980-01-01",
                DeptNo = 1,
                Phone = "1234567890",
                Gender = 'M',
                Address = "123 Main St",
                Qualification = "MBBS"
            };

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("Email already exists. Please use a different email.", model.Message);
        }

        [Fact]
        public async Task OnPostAsync_SuccessfulRegistration_ReturnsPageWithSuccessMessage()
        {
            SetupSession(3);
            SetupDepartments();
            _adminServiceMock.Setup(s => s.DoctorEmailExistsAsync(It.IsAny<string>(), default))
                .ReturnsAsync(false);
            _adminServiceMock.Setup(s => s.AddDoctorAsync(It.IsAny<AddDoctorDto>(), default))
                .ReturnsAsync(true);

            var model = CreateModel();
            model.DoctorInput = new AddDoctorViewModel
            {
                Name = "Dr. New",
                Email = "new@test.com",
                Password = "pass123",
                BirthDate = "1980-01-01",
                DeptNo = 1,
                Phone = "1234567890",
                Gender = 'M',
                Address = "123 Main St",
                Qualification = "MBBS"
            };

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("Doctor registered successfully!", model.Message);
            Assert.True(model.Success);
        }

        [Fact]
        public async Task OnPostAsync_FailedRegistration_ReturnsPageWithFailureMessage()
        {
            SetupSession(3);
            SetupDepartments();
            _adminServiceMock.Setup(s => s.DoctorEmailExistsAsync(It.IsAny<string>(), default))
                .ReturnsAsync(false);
            _adminServiceMock.Setup(s => s.AddDoctorAsync(It.IsAny<AddDoctorDto>(), default))
                .ReturnsAsync(false);

            var model = CreateModel();
            model.DoctorInput = new AddDoctorViewModel
            {
                Name = "Dr. Fail",
                Email = "fail@test.com",
                Password = "pass123",
                BirthDate = "1980-01-01",
                DeptNo = 1,
                Phone = "1234567890",
                Gender = 'M',
                Address = "123 Main St",
                Qualification = "MBBS"
            };

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("Failed to register doctor. Please try again.", model.Message);
            Assert.False(model.Success);
        }

        [Fact]
        public async Task OnPostAsync_ServiceThrowsException_ReturnsPageWithErrorMessage()
        {
            SetupSession(3);
            SetupDepartments();
            _adminServiceMock.Setup(s => s.DoctorEmailExistsAsync(It.IsAny<string>(), default))
                .ThrowsAsync(new Exception("DB error"));

            var model = CreateModel();
            model.DoctorInput = new AddDoctorViewModel
            {
                Name = "Dr. Error",
                Email = "error@test.com",
                Password = "pass123",
                BirthDate = "1980-01-01",
                DeptNo = 1,
                Phone = "1234567890",
                Gender = 'M',
                Address = "123 Main St",
                Qualification = "MBBS"
            };

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("An error occurred. Please try again.", model.Message);
        }

        [Fact]
        public async Task OnPostAsync_SuccessfulRegistration_MapsDtoCorrectly()
        {
            SetupSession(3);
            SetupDepartments();
            _adminServiceMock.Setup(s => s.DoctorEmailExistsAsync(It.IsAny<string>(), default))
                .ReturnsAsync(false);

            AddDoctorDto? capturedDto = null;
            _adminServiceMock.Setup(s => s.AddDoctorAsync(It.IsAny<AddDoctorDto>(), default))
                .Callback<AddDoctorDto, System.Threading.CancellationToken>((dto, _) => capturedDto = dto)
                .ReturnsAsync(true);

            var model = CreateModel();
            model.DoctorInput = new AddDoctorViewModel
            {
                Name = "Dr. Mapped",
                Email = "mapped@test.com",
                Password = "pass123",
                BirthDate = "1985-06-15",
                DeptNo = 2,
                Phone = "9876543210",
                Gender = 'F',
                Address = "456 Oak Ave",
                WorkExperience = 10,
                Salary = 80000,
                ChargesPerVisit = 500,
                Specialization = "Cardiology",
                Qualification = "MBBS, MD"
            };

            await model.OnPostAsync();

            Assert.NotNull(capturedDto);
            Assert.Equal("Dr. Mapped", capturedDto!.Name);
            Assert.Equal("mapped@test.com", capturedDto.Email);
            Assert.Equal(2, capturedDto.DeptNo);
            Assert.Equal('F', capturedDto.Gender);
            Assert.Equal(10, capturedDto.WorkExperience);
            Assert.Equal(80000, capturedDto.Salary);
            Assert.Equal(500, capturedDto.ChargesPerVisit);
        }
    }
}
