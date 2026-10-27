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
    public class AddStaffModelTests
    {
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<ILogger<AddStaffModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public AddStaffModelTests()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _loggerMock = new Mock<ILogger<AddStaffModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private AddStaffModel CreateModel()
        {
            var model = new AddStaffModel(_adminServiceMock.Object, _loggerMock.Object);
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
            Assert.NotNull(model.StaffInput);
            Assert.Equal(string.Empty, model.Message);
            Assert.False(model.Success);
        }

        [Fact]
        public void OnGet_NonAdminUser_RedirectsToSignUp()
        {
            SetupSession(1);
            var model = CreateModel();

            var result = model.OnGet();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public void OnGet_AdminUser_ReturnsPage()
        {
            SetupSession(3);
            var model = CreateModel();

            var result = model.OnGet();

            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void OnGet_NoSession_RedirectsToSignUp()
        {
            SetupSession(null);
            var model = CreateModel();

            var result = model.OnGet();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_NonAdminUser_RedirectsToSignUp()
        {
            SetupSession(1);
            var model = CreateModel();

            var result = await model.OnPostAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_InvalidModelState_ReturnsPageWithMessage()
        {
            SetupSession(3);
            var model = CreateModel();
            model.ModelState.AddModelError("Name", "Required");

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("Please fill in all required fields.", model.Message);
        }

        [Fact]
        public async Task OnPostAsync_SuccessfulAddition_ReturnsPageWithSuccessMessage()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), default))
                .ReturnsAsync(true);

            var model = CreateModel();
            model.StaffInput = new AddStaffViewModel
            {
                Name = "New Staff",
                BirthDate = "1990-01-01",
                Phone = "1234567890",
                Gender = 'M',
                Address = "123 Main St",
                Salary = 30000,
                Qualification = "BSc",
                Designation = "Nurse"
            };

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("Staff member added successfully!", model.Message);
            Assert.True(model.Success);
        }

        [Fact]
        public async Task OnPostAsync_FailedAddition_ReturnsPageWithFailureMessage()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), default))
                .ReturnsAsync(false);

            var model = CreateModel();
            model.StaffInput = new AddStaffViewModel
            {
                Name = "Fail Staff",
                BirthDate = "1990-01-01",
                Phone = "1234567890",
                Gender = 'M',
                Address = "123 Main St",
                Salary = 30000,
                Qualification = "BSc",
                Designation = "Nurse"
            };

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("Failed to add staff member. Please try again.", model.Message);
            Assert.False(model.Success);
        }

        [Fact]
        public async Task OnPostAsync_ServiceThrowsException_ReturnsPageWithErrorMessage()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), default))
                .ThrowsAsync(new Exception("DB error"));

            var model = CreateModel();
            model.StaffInput = new AddStaffViewModel
            {
                Name = "Error Staff",
                BirthDate = "1990-01-01",
                Phone = "1234567890",
                Gender = 'M',
                Address = "123 Main St",
                Salary = 30000,
                Qualification = "BSc",
                Designation = "Nurse"
            };

            var result = await model.OnPostAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal("An error occurred. Please try again.", model.Message);
        }

        [Fact]
        public async Task OnPostAsync_SuccessfulAddition_MapsDtoCorrectly()
        {
            SetupSession(3);
            AddStaffDto? capturedDto = null;
            _adminServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), default))
                .Callback<AddStaffDto, System.Threading.CancellationToken>((dto, _) => capturedDto = dto)
                .ReturnsAsync(true);

            var model = CreateModel();
            model.StaffInput = new AddStaffViewModel
            {
                Name = "Mapped Staff",
                BirthDate = "1985-03-20",
                Phone = "9876543210",
                Gender = 'F',
                Address = "456 Oak Ave",
                Salary = 40000,
                Qualification = "BSc Nursing",
                Designation = "Head Nurse"
            };

            await model.OnPostAsync();

            Assert.NotNull(capturedDto);
            Assert.Equal("Mapped Staff", capturedDto!.Name);
            Assert.Equal("1985-03-20", capturedDto.BirthDate);
            Assert.Equal("9876543210", capturedDto.Phone);
            Assert.Equal('F', capturedDto.Gender);
            Assert.Equal("456 Oak Ave", capturedDto.Address);
            Assert.Equal(40000, capturedDto.Salary);
            Assert.Equal("BSc Nursing", capturedDto.Qualification);
            Assert.Equal("Head Nurse", capturedDto.Designation);
        }

        [Fact]
        public async Task OnPostAsync_SuccessfulAddition_VerifiesServiceCalled()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), default))
                .ReturnsAsync(true);

            var model = CreateModel();
            model.StaffInput = new AddStaffViewModel
            {
                Name = "Verify Staff",
                BirthDate = "1990-01-01",
                Phone = "1234567890",
                Gender = 'M',
                Address = "123 Main St",
                Salary = 30000,
                Qualification = "BSc",
                Designation = "Nurse"
            };

            await model.OnPostAsync();

            _adminServiceMock.Verify(s => s.AddStaffAsync(It.IsAny<AddStaffDto>(), default), Times.Once);
        }
    }
}
