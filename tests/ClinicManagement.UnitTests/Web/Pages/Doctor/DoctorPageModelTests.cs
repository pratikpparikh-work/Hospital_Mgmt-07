using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.Pages.Doctor;
using ClinicManagement.Web.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages.Doctor
{
    public class DoctorHomeModelTests
    {
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<ILogger<DoctorHomeModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public DoctorHomeModelTests()
        {
            _doctorServiceMock = new Mock<IDoctorService>();
            _loggerMock = new Mock<ILogger<DoctorHomeModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private DoctorHomeModel CreateModel()
        {
            var model = new DoctorHomeModel(_doctorServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        private void SetupSession(int? userId, int? userType)
        {
            if (userId.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserId", userId.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserId");
            if (userType.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserType", userType.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserType");
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var model = CreateModel();
            Assert.NotNull(model);
        }

        [Fact]
        public void Constructor_InitializesNullDoctorInfo()
        {
            var model = CreateModel();
            Assert.Null(model.DoctorInfo);
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
        public async Task OnGetAsync_DoctorUser_ReturnsPage()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetDoctorInfoAsync(2, default))
                .ReturnsAsync(new DoctorDetailDto { DoctorId = 2, Name = "Dr. Smith", Department = "Cardiology", Gender = "Male", ChargesPerVisit = 300, Qualification = "MBBS", Age = 40, BirthDate = "1984-01-01" });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.NotNull(model.DoctorInfo);
            Assert.Equal("Dr. Smith", model.DoctorInfo!.Name);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPageWithNullInfo()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetDoctorInfoAsync(2, default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Null(model.DoctorInfo);
        }

        [Fact]
        public async Task OnGetAsync_DoctorUser_CallsServiceWithCorrectId()
        {
            SetupSession(5, 2);
            _doctorServiceMock.Setup(s => s.GetDoctorInfoAsync(5, default))
                .ReturnsAsync(new DoctorDetailDto { DoctorId = 5, Name = "Dr. Five" });
            var model = CreateModel();
            await model.OnGetAsync();
            _doctorServiceMock.Verify(s => s.GetDoctorInfoAsync(5, default), Times.Once);
        }
    }

    public class PendingAppointmentModelTests
    {
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<ILogger<PendingAppointmentModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public PendingAppointmentModelTests()
        {
            _doctorServiceMock = new Mock<IDoctorService>();
            _loggerMock = new Mock<ILogger<PendingAppointmentModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private PendingAppointmentModel CreateModel()
        {
            var model = new PendingAppointmentModel(_doctorServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        private void SetupSession(int? userId, int? userType)
        {
            if (userId.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserId", userId.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserId");
            if (userType.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserType", userType.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserType");
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var model = CreateModel();
            Assert.NotNull(model);
            Assert.NotNull(model.Appointments);
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
        public async Task OnGetAsync_DoctorUser_LoadsAppointments()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetPendingAppointmentsAsync(2, default))
                .ReturnsAsync(new List<AppointmentDto> { new AppointmentDto { AppointId = 1, PatientName = "Alice", Status = "Pending" } });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            var appts = new List<AppointmentDto>(model.Appointments);
            Assert.Single(appts);
            Assert.Equal("Alice", appts[0].PatientName);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPageWithEmptyList()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetPendingAppointmentsAsync(2, default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostApproveAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var model = CreateModel();
            var result = await model.OnPostApproveAsync(1);
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostApproveAsync_DoctorUser_ApprovesAppointment()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.ApproveAppointmentAsync(1, default)).ReturnsAsync(true);
            var model = CreateModel();
            var result = await model.OnPostApproveAsync(1);
            _doctorServiceMock.Verify(s => s.ApproveAppointmentAsync(1, default), Times.Once);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostApproveAsync_ServiceThrowsException_StillRedirects()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.ApproveAppointmentAsync(It.IsAny<int>(), default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnPostApproveAsync(1);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostRejectAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var model = CreateModel();
            var result = await model.OnPostRejectAsync(1);
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostRejectAsync_DoctorUser_RejectsAppointment()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.RejectAppointmentAsync(3, default)).ReturnsAsync(true);
            var model = CreateModel();
            var result = await model.OnPostRejectAsync(3);
            _doctorServiceMock.Verify(s => s.RejectAppointmentAsync(3, default), Times.Once);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostRejectAsync_ServiceThrowsException_StillRedirects()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.RejectAppointmentAsync(It.IsAny<int>(), default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnPostRejectAsync(3);
            Assert.IsType<RedirectToPageResult>(result);
        }
    }

    public class HistoryUpdateModelTests
    {
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<ILogger<HistoryUpdateModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public HistoryUpdateModelTests()
        {
            _doctorServiceMock = new Mock<IDoctorService>();
            _loggerMock = new Mock<ILogger<HistoryUpdateModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private HistoryUpdateModel CreateModel()
        {
            var model = new HistoryUpdateModel(_doctorServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        private void SetupSession(int? userId, int? userType)
        {
            if (userId.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserId", userId.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserId");
            if (userType.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserType", userType.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserType");
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
            Assert.Equal(0, model.AppointmentId);
            Assert.Equal(string.Empty, model.PatientName);
            Assert.Equal(string.Empty, model.Disease);
            Assert.Equal(string.Empty, model.Progress);
            Assert.Equal(string.Empty, model.Prescription);
            Assert.Equal(string.Empty, model.Message);
            Assert.False(model.Success);
        }

        [Fact]
        public void OnGet_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var model = CreateModel();
            var result = model.OnGet(1, "Alice");
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public void OnGet_DoctorUser_ReturnsPageWithData()
        {
            SetupSession(2, 2);
            var model = CreateModel();
            var result = model.OnGet(10, "Bob");
            Assert.IsType<PageResult>(result);
            Assert.Equal(10, model.AppointmentId);
            Assert.Equal("Bob", model.PatientName);
        }

        [Fact]
        public void OnGet_DoctorUser_DefaultPatientName()
        {
            SetupSession(2, 2);
            var model = CreateModel();
            var result = model.OnGet(5);
            Assert.IsType<PageResult>(result);
            Assert.Equal(5, model.AppointmentId);
            Assert.Equal("", model.PatientName);
        }

        [Fact]
        public async Task OnPostAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var model = CreateModel();
            var result = await model.OnPostAsync(1, "Flu", "Recovering", "Paracetamol");
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostAsync_SuccessfulUpdate_ReturnsPageWithSuccessMessage()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.UpdatePrescriptionAsync(2, 10, "Flu", "Recovering", "Paracetamol", default)).ReturnsAsync(true);
            var model = CreateModel();
            var result = await model.OnPostAsync(10, "Flu", "Recovering", "Paracetamol");
            Assert.IsType<PageResult>(result);
            Assert.Equal("Patient history updated successfully!", model.Message);
            Assert.True(model.Success);
        }

        [Fact]
        public async Task OnPostAsync_FailedUpdate_ReturnsPageWithFailureMessage()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.UpdatePrescriptionAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), default)).ReturnsAsync(false);
            var model = CreateModel();
            var result = await model.OnPostAsync(10, "Flu", "Recovering", "Paracetamol");
            Assert.IsType<PageResult>(result);
            Assert.Equal("Failed to update patient history.", model.Message);
            Assert.False(model.Success);
        }

        [Fact]
        public async Task OnPostAsync_ServiceThrowsException_ReturnsPageWithErrorMessage()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.UpdatePrescriptionAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnPostAsync(10, "Flu", "Recovering", "Paracetamol");
            Assert.IsType<PageResult>(result);
            Assert.Equal("An error occurred. Please try again.", model.Message);
        }
    }

    public class BillModelTests
    {
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<ILogger<BillModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public BillModelTests()
        {
            _doctorServiceMock = new Mock<IDoctorService>();
            _loggerMock = new Mock<ILogger<BillModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private BillModel CreateModel()
        {
            var model = new BillModel(_doctorServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        private void SetupSession(int? userId, int? userType)
        {
            if (userId.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserId", userId.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserId");
            if (userType.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserType", userType.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserType");
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var model = CreateModel();
            Assert.NotNull(model);
            Assert.NotNull(model.Bills);
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
        public async Task OnGetAsync_DoctorUser_LoadsBills()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetBillableAppointmentsAsync(2, default))
                .ReturnsAsync(new List<BillDto> { new BillDto { AppointId = 1, PatientName = "Alice", BillAmount = 500, BillStatus = "Unpaid" } });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            var bills = new List<BillDto>(model.Bills);
            Assert.Single(bills);
            Assert.Equal("Alice", bills[0].PatientName);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetBillableAppointmentsAsync(2, default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostMarkPaidAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var model = CreateModel();
            var result = await model.OnPostMarkPaidAsync(1);
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostMarkPaidAsync_DoctorUser_MarksBillPaid()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.MarkBillPaidAsync(2, 5, default)).ReturnsAsync(true);
            var model = CreateModel();
            var result = await model.OnPostMarkPaidAsync(5);
            _doctorServiceMock.Verify(s => s.MarkBillPaidAsync(2, 5, default), Times.Once);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostMarkPaidAsync_ServiceThrowsException_StillRedirects()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.MarkBillPaidAsync(It.IsAny<int>(), It.IsAny<int>(), default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnPostMarkPaidAsync(5);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostMarkUnpaidAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var model = CreateModel();
            var result = await model.OnPostMarkUnpaidAsync(1);
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/SignUp", redirect.PageName);
        }

        [Fact]
        public async Task OnPostMarkUnpaidAsync_DoctorUser_MarksBillUnpaid()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.MarkBillUnpaidAsync(2, 5, default)).ReturnsAsync(true);
            var model = CreateModel();
            var result = await model.OnPostMarkUnpaidAsync(5);
            _doctorServiceMock.Verify(s => s.MarkBillUnpaidAsync(2, 5, default), Times.Once);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostMarkUnpaidAsync_ServiceThrowsException_StillRedirects()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.MarkBillUnpaidAsync(It.IsAny<int>(), It.IsAny<int>(), default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnPostMarkUnpaidAsync(5);
            Assert.IsType<RedirectToPageResult>(result);
        }
    }

    public class PatientHistoryModelTests
    {
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<ILogger<PatientHistoryModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public PatientHistoryModelTests()
        {
            _doctorServiceMock = new Mock<IDoctorService>();
            _loggerMock = new Mock<ILogger<PatientHistoryModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private PatientHistoryModel CreateModel()
        {
            var model = new PatientHistoryModel(_doctorServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        private void SetupSession(int? userId, int? userType)
        {
            if (userId.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserId", userId.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserId");
            if (userType.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserType", userType.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserType");
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var model = CreateModel();
            Assert.NotNull(model);
            Assert.NotNull(model.Appointments);
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
        public async Task OnGetAsync_DoctorUser_LoadsTodaysAppointments()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetTodaysAppointmentsAsync(2, default))
                .ReturnsAsync(new List<AppointmentDto> { new AppointmentDto { AppointId = 1, PatientName = "Carol", Status = "Approved" } });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            var appts = new List<AppointmentDto>(model.Appointments);
            Assert.Single(appts);
            Assert.Equal("Carol", appts[0].PatientName);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetTodaysAppointmentsAsync(2, default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
        }
    }

    public class PreviousHistoryModelTests
    {
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<ILogger<PreviousHistoryModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public PreviousHistoryModelTests()
        {
            _doctorServiceMock = new Mock<IDoctorService>();
            _loggerMock = new Mock<ILogger<PreviousHistoryModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private PreviousHistoryModel CreateModel()
        {
            var model = new PreviousHistoryModel(_doctorServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            return model;
        }

        private void SetupSession(int? userId, int? userType)
        {
            if (userId.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserId", userId.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserId");
            if (userType.HasValue) SessionMockHelper.SetupInt32(_sessionMock, "UserType", userType.Value);
            else SessionMockHelper.SetupMissingKey(_sessionMock, "UserType");
        }

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            var model = CreateModel();
            Assert.NotNull(model);
            Assert.NotNull(model.History);
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
        public async Task OnGetAsync_DoctorUser_LoadsHistory()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetPatientHistoryAsync(2, default))
                .ReturnsAsync(new List<PatientHistoryDto> { new PatientHistoryDto { AppointId = 1, PatientName = "Dave", Disease = "Flu" } });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            var history = new List<PatientHistoryDto>(model.History);
            Assert.Single(history);
            Assert.Equal("Dave", history[0].PatientName);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(2, 2);
            _doctorServiceMock.Setup(s => s.GetPatientHistoryAsync(2, default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
        }
    }
}
