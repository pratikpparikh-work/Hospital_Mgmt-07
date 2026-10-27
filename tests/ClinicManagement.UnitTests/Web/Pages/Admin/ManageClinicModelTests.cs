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
    public class ManageClinicModelTests
    {
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<ILogger<ManageClinicModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public ManageClinicModelTests()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _loggerMock = new Mock<ILogger<ManageClinicModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private ManageClinicModel CreateModel()
        {
            var model = new ManageClinicModel(_adminServiceMock.Object, _loggerMock.Object);
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

        private void SetupDefaultData()
        {
            _adminServiceMock.Setup(s => s.GetDoctorsAsync(It.IsAny<string>(), default))
                .ReturnsAsync(new List<DoctorListDto>
                {
                    new DoctorListDto { DoctorId = 1, Name = "Dr. Smith", Department = "Cardiology", Phone = "1234567890", ChargesPerVisit = 300 }
                });
            _adminServiceMock.Setup(s => s.GetPatientsAsync(It.IsAny<string>(), default))
                .ReturnsAsync(new List<PatientListDto>
                {
                    new PatientListDto { PatientId = 1, Name = "Alice", Phone = "9876543210" }
                });
            _adminServiceMock.Setup(s => s.GetStaffAsync(It.IsAny<string>(), default))
                .ReturnsAsync(new List<StaffListDto>
                {
                    new StaffListDto { StaffId = 1, Name = "Bob", Designation = "Nurse" }
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
            Assert.Equal(string.Empty, model.SearchQuery);
            Assert.Equal("doctors", model.ActiveTab);
            Assert.NotNull(model.Doctors);
            Assert.NotNull(model.Patients);
            Assert.NotNull(model.Staff);
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
        public async Task OnGetAsync_AdminUser_ReturnsPage()
        {
            SetupSession(3);
            SetupDefaultData();

            var model = CreateModel();
            var result = await model.OnGetAsync();

            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_WithSearchQuery_SetsSearchQuery()
        {
            SetupSession(3);
            SetupDefaultData();

            var model = CreateModel();
            var result = await model.OnGetAsync("Smith", "doctors");

            Assert.IsType<PageResult>(result);
            Assert.Equal("Smith", model.SearchQuery);
            Assert.Equal("doctors", model.ActiveTab);
        }

        [Fact]
        public async Task OnGetAsync_NullSearchQuery_SetsEmptyString()
        {
            SetupSession(3);
            SetupDefaultData();

            var model = CreateModel();
            var result = await model.OnGetAsync(null, null);

            Assert.IsType<PageResult>(result);
            Assert.Equal(string.Empty, model.SearchQuery);
            Assert.Equal("doctors", model.ActiveTab);
        }

        [Fact]
        public async Task OnGetAsync_AdminUser_LoadsDoctors()
        {
            SetupSession(3);
            SetupDefaultData();

            var model = CreateModel();
            await model.OnGetAsync();

            var doctors = new List<DoctorListViewModel>(model.Doctors);
            Assert.Single(doctors);
            Assert.Equal("Dr. Smith", doctors[0].Name);
            Assert.Equal("Cardiology", doctors[0].Department);
        }

        [Fact]
        public async Task OnGetAsync_AdminUser_LoadsPatients()
        {
            SetupSession(3);
            SetupDefaultData();

            var model = CreateModel();
            await model.OnGetAsync();

            var patients = new List<PatientListViewModel>(model.Patients);
            Assert.Single(patients);
            Assert.Equal("Alice", patients[0].Name);
        }

        [Fact]
        public async Task OnGetAsync_AdminUser_LoadsStaff()
        {
            SetupSession(3);
            SetupDefaultData();

            var model = CreateModel();
            await model.OnGetAsync();

            var staff = new List<StaffListViewModel>(model.Staff);
            Assert.Single(staff);
            Assert.Equal("Bob", staff[0].Name);
            Assert.Equal("Nurse", staff[0].Designation);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPageWithEmptyData()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.GetDoctorsAsync(It.IsAny<string>(), default))
                .ThrowsAsync(new Exception("DB error"));

            var model = CreateModel();
            var result = await model.OnGetAsync();

            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteDoctorAsync_NonAdminUser_RedirectsToSignUp()
        {
            SetupSession(1);
            var model = CreateModel();

            var result = await model.OnPostDeleteDoctorAsync(1);

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostDeleteDoctorAsync_AdminUser_DeletesDoctorAndRedirects()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.DeleteDoctorAsync(It.IsAny<int>(), default))
                .ReturnsAsync(true);

            var model = CreateModel();
            var result = await model.OnPostDeleteDoctorAsync(5);

            _adminServiceMock.Verify(s => s.DeleteDoctorAsync(5, default), Times.Once);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteDoctorAsync_ServiceThrowsException_StillRedirects()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.DeleteDoctorAsync(It.IsAny<int>(), default))
                .ThrowsAsync(new Exception("DB error"));

            var model = CreateModel();
            var result = await model.OnPostDeleteDoctorAsync(5);

            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteStaffAsync_NonAdminUser_RedirectsToSignUp()
        {
            SetupSession(1);
            var model = CreateModel();

            var result = await model.OnPostDeleteStaffAsync(1);

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostDeleteStaffAsync_AdminUser_DeletesStaffAndRedirects()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.DeleteStaffAsync(It.IsAny<int>(), default))
                .ReturnsAsync(true);

            var model = CreateModel();
            var result = await model.OnPostDeleteStaffAsync(7);

            _adminServiceMock.Verify(s => s.DeleteStaffAsync(7, default), Times.Once);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostDeleteStaffAsync_ServiceThrowsException_StillRedirects()
        {
            SetupSession(3);
            _adminServiceMock.Setup(s => s.DeleteStaffAsync(It.IsAny<int>(), default))
                .ThrowsAsync(new Exception("DB error"));

            var model = CreateModel();
            var result = await model.OnPostDeleteStaffAsync(7);

            Assert.IsType<RedirectToPageResult>(result);
        }
    }
}
