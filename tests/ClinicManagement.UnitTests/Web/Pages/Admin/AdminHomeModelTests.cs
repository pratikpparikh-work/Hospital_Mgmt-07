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
    public class AdminHomeModelTests
    {
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<ILogger<AdminHomeModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public AdminHomeModelTests()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _loggerMock = new Mock<ILogger<AdminHomeModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private AdminHomeModel CreateModel()
        {
            var model = new AdminHomeModel(_adminServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        private void SetupSession(int? userId, int? userType)
        {
            if (userId.HasValue)
                SessionMockHelper.SetupInt32(_sessionMock, "UserId", userId.Value);
            else
                SessionMockHelper.SetupMissingKey(_sessionMock, "UserId");

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
        public void Constructor_InitializesAdminData()
        {
            var model = CreateModel();
            Assert.NotNull(model.AdminData);
        }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var model = CreateModel();

            var result = await model.OnGetAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_WrongUserType_RedirectsToSignUp()
        {
            SetupSession(1, 1);
            var model = CreateModel();

            var result = await model.OnGetAsync();

            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnGetAsync_AdminUser_ReturnsPage()
        {
            SetupSession(3, 3);
            _adminServiceMock.Setup(s => s.GetAdminHomeDataAsync(default))
                .ReturnsAsync(new AdminHomeDto
                {
                    TotalDoctors = 5,
                    TotalPatients = 20,
                    TotalIncome = 50000.0,
                    Departments = new List<DepartmentSummaryDto>
                    {
                        new DepartmentSummaryDto { DeptName = "Cardiology", DoctorCount = 2, Description = "Heart" }
                    },
                    Appointments = new List<AppointmentSummaryDto>
                    {
                        new AppointmentSummaryDto { AppointId = 1, PatientName = "Alice", DoctorName = "Dr. Bob", Date = DateTime.Now, Status = "Pending" }
                    }
                });

            var model = CreateModel();
            var result = await model.OnGetAsync();

            Assert.IsType<PageResult>(result);
            Assert.Equal(5, model.AdminData.TotalDoctors);
            Assert.Equal(20, model.AdminData.TotalPatients);
            Assert.Equal(50000.0, model.AdminData.TotalIncome);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPageWithEmptyData()
        {
            SetupSession(3, 3);
            _adminServiceMock.Setup(s => s.GetAdminHomeDataAsync(default))
                .ThrowsAsync(new Exception("DB error"));

            var model = CreateModel();
            var result = await model.OnGetAsync();

            Assert.IsType<PageResult>(result);
            Assert.NotNull(model.AdminData);
        }

        [Fact]
        public async Task OnGetAsync_AdminUser_MapsDepartmentsCorrectly()
        {
            SetupSession(3, 3);
            _adminServiceMock.Setup(s => s.GetAdminHomeDataAsync(default))
                .ReturnsAsync(new AdminHomeDto
                {
                    TotalDoctors = 3,
                    TotalPatients = 10,
                    TotalIncome = 10000.0,
                    Departments = new List<DepartmentSummaryDto>
                    {
                        new DepartmentSummaryDto { DeptName = "Neurology", DoctorCount = 3, Description = "Brain" }
                    },
                    Appointments = new List<AppointmentSummaryDto>()
                });

            var model = CreateModel();
            await model.OnGetAsync();

            var depts = new List<DepartmentSummaryViewModel>(model.AdminData.Departments);
            Assert.Single(depts);
            Assert.Equal("Neurology", depts[0].DeptName);
            Assert.Equal(3, depts[0].DoctorCount);
            Assert.Equal("Brain", depts[0].Description);
        }

        [Fact]
        public async Task OnGetAsync_AdminUser_MapsAppointmentsCorrectly()
        {
            var date = new DateTime(2024, 5, 1);
            SetupSession(3, 3);
            _adminServiceMock.Setup(s => s.GetAdminHomeDataAsync(default))
                .ReturnsAsync(new AdminHomeDto
                {
                    TotalDoctors = 1,
                    TotalPatients = 1,
                    TotalIncome = 500.0,
                    Departments = new List<DepartmentSummaryDto>(),
                    Appointments = new List<AppointmentSummaryDto>
                    {
                        new AppointmentSummaryDto { AppointId = 99, PatientName = "Bob", DoctorName = "Dr. Alice", Date = date, Status = "Approved" }
                    }
                });

            var model = CreateModel();
            await model.OnGetAsync();

            var appts = new List<AppointmentSummaryViewModel>(model.AdminData.Appointments);
            Assert.Single(appts);
            Assert.Equal(99, appts[0].AppointId);
            Assert.Equal("Bob", appts[0].PatientName);
            Assert.Equal("Dr. Alice", appts[0].DoctorName);
            Assert.Equal(date, appts[0].Date);
            Assert.Equal("Approved", appts[0].Status);
        }
    }
}
