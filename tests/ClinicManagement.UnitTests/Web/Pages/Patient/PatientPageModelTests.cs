using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.Pages.Patient;
using ClinicManagement.Web.Tests.Helpers;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ClinicManagement.Web.Tests.Pages.Patient
{
    public class PatientHomeModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<PatientHomeModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public PatientHomeModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<PatientHomeModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private PatientHomeModel CreateModel()
        {
            var model = new PatientHomeModel(_patientServiceMock.Object, _loggerMock.Object);
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public void Constructor_InitializesNullProperties()
        {
            var model = CreateModel();
            Assert.Null(model.PatientInfo);
            Assert.Null(model.CurrentAppointment);
        }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_WrongUserType_RedirectsToSignUp()
        {
            SetupSession(2, 2);
            var result = await CreateModel().OnGetAsync();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetPatientInfoAsync(1, default)).ReturnsAsync(new PatientDetailDto { PatientId = 1, Name = "Alice", Age = 30, Gender = "Female" });
            _patientServiceMock.Setup(s => s.GetCurrentAppointmentAsync(1, default)).ReturnsAsync((CurrentAppointmentDto?)null);
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.NotNull(model.PatientInfo);
            Assert.Equal("Alice", model.PatientInfo!.Name);
        }

        [Fact]
        public async Task OnGetAsync_PatientWithCurrentAppointment_SetsCurrentAppointment()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetPatientInfoAsync(1, default)).ReturnsAsync(new PatientDetailDto { PatientId = 1, Name = "Bob" });
            _patientServiceMock.Setup(s => s.GetCurrentAppointmentAsync(1, default)).ReturnsAsync(new CurrentAppointmentDto { DoctorName = "Dr. Smith", Timings = "10:00 AM" });
            var model = CreateModel();
            await model.OnGetAsync();
            Assert.NotNull(model.CurrentAppointment);
            Assert.Equal("Dr. Smith", model.CurrentAppointment!.DoctorName);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetPatientInfoAsync(1, default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync();
            Assert.IsType<PageResult>(result);
        }
    }

    public class AppointmentTakerModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<AppointmentTakerModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<ITempDataDictionary> _tempDataMock;

        public AppointmentTakerModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<AppointmentTakerModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _tempDataMock = new Mock<ITempDataDictionary>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private AppointmentTakerModel CreateModel()
        {
            var model = new AppointmentTakerModel(_patientServiceMock.Object, _loggerMock.Object);
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            model.TempData = _tempDataMock.Object;
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public void Constructor_InitializesDefaultProperties()
        {
            var model = CreateModel();
            Assert.Equal(0, model.DoctorId);
            Assert.Equal(string.Empty, model.DoctorName);
            Assert.NotNull(model.FreeSlots);
        }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync(1);
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_LoadsDoctorAndSlots()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDoctorProfileAsync(5, default)).ReturnsAsync(new DoctorProfileDto { DoctorId = 5, Name = "Dr. Jones" });
            _patientServiceMock.Setup(s => s.GetFreeSlotsAsync(5, 1, default)).ReturnsAsync(new List<FreeSlotDto> { new FreeSlotDto { SlotId = 1, Timings = "9:00 AM", Date = DateTime.Today } });
            var model = CreateModel();
            var result = await model.OnGetAsync(5);
            Assert.IsType<PageResult>(result);
            Assert.Equal(5, model.DoctorId);
            Assert.Equal("Dr. Jones", model.DoctorName);
            Assert.Single(new List<FreeSlotViewModel>(model.FreeSlots));
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDoctorProfileAsync(It.IsAny<int>(), default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync(5);
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnPostAsync(1, 1);
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnPostAsync_PatientUser_BooksAppointmentAndRedirects()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.BookAppointmentAsync(5, 1, 2, default)).ReturnsAsync(new AppointmentResultDto { Success = true, Message = "Booked!" });
            var model = CreateModel();
            var result = await model.OnPostAsync(5, 2);
            _patientServiceMock.Verify(s => s.BookAppointmentAsync(5, 1, 2, default), Times.Once);
            Assert.Equal("/Patient/AppointmentRequestSent", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnPostAsync_ServiceThrowsException_RedirectsWithErrorMessage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.BookAppointmentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnPostAsync(5, 2);
            Assert.IsType<RedirectToPageResult>(result);
        }
    }

    public class AppointmentRequestSentModelTests
    {
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<ITempDataDictionary> _tempDataMock;

        public AppointmentRequestSentModelTests()
        {
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _tempDataMock = new Mock<ITempDataDictionary>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private AppointmentRequestSentModel CreateModel()
        {
            var model = new AppointmentRequestSentModel();
            model.PageContext = new PageContext { HttpContext = _httpContextMock.Object };
            model.TempData = _tempDataMock.Object;
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
        public void Constructor_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public void Constructor_InitializesDefaultProperties()
        {
            var model = CreateModel();
            Assert.False(model.Success);
            Assert.Equal(string.Empty, model.Message);
        }

        [Fact]
        public void OnGet_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = CreateModel().OnGet();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public void OnGet_PatientUser_ReturnsPage()
        {
            SetupSession(1, 1);
            _tempDataMock.Setup(t => t["AppointmentSuccess"]).Returns(true);
            _tempDataMock.Setup(t => t["AppointmentMessage"]).Returns("Booked!");
            var result = CreateModel().OnGet();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void OnGet_PatientUser_NoTempData_DefaultsToFalse()
        {
            SetupSession(1, 1);
            _tempDataMock.Setup(t => t["AppointmentSuccess"]).Returns(null);
            _tempDataMock.Setup(t => t["AppointmentMessage"]).Returns(null);
            var model = CreateModel();
            var result = model.OnGet();
            Assert.IsType<PageResult>(result);
            Assert.False(model.Success);
            Assert.Equal(string.Empty, model.Message);
        }
    }

    public class TreatmentHistoryModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<TreatmentHistoryModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public TreatmentHistoryModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<TreatmentHistoryModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private TreatmentHistoryModel CreateModel()
        {
            var model = new TreatmentHistoryModel(_patientServiceMock.Object, _loggerMock.Object);
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_LoadsTreatments()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetTreatmentHistoryAsync(1, default)).ReturnsAsync(new List<TreatmentHistoryDto> { new TreatmentHistoryDto { AppointId = 1, DoctorName = "Dr. X", Disease = "Flu" } });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Single(new List<TreatmentHistoryDto>(model.Treatments));
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetTreatmentHistoryAsync(1, default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync();
            Assert.IsType<PageResult>(result);
        }
    }

    public class PatientFeedbackModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<PatientFeedbackModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public PatientFeedbackModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<PatientFeedbackModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private PatientFeedbackModel CreateModel()
        {
            var model = new PatientFeedbackModel(_patientServiceMock.Object, _loggerMock.Object);
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public void Constructor_InitializesDefaultProperties()
        {
            var model = CreateModel();
            Assert.False(model.HasPendingFeedback);
            Assert.Equal(0, model.AppointmentId);
            Assert.Equal(string.Empty, model.DoctorName);
            Assert.Equal(string.Empty, model.Timings);
            Assert.False(model.FeedbackSubmitted);
        }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_WithPendingFeedback_LoadsFeedback()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetPendingFeedbackAsync(1, default)).ReturnsAsync(new PendingFeedbackDto { HasPendingFeedback = true, AppointmentId = 10, DoctorName = "Dr. Green", Timings = "3:00 PM" });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.True(model.HasPendingFeedback);
            Assert.Equal(10, model.AppointmentId);
            Assert.Equal("Dr. Green", model.DoctorName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_NoPendingFeedback_DefaultValues()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetPendingFeedbackAsync(1, default)).ReturnsAsync((PendingFeedbackDto?)null);
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.False(model.HasPendingFeedback);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetPendingFeedbackAsync(1, default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnPostAsync(1);
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnPostAsync_PatientUser_SubmitsFeedback()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.SubmitFeedbackAsync(10, default)).ReturnsAsync(true);
            var model = CreateModel();
            var result = await model.OnPostAsync(10);
            _patientServiceMock.Verify(s => s.SubmitFeedbackAsync(10, default), Times.Once);
            Assert.IsType<PageResult>(result);
            Assert.True(model.FeedbackSubmitted);
        }

        [Fact]
        public async Task OnPostAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.SubmitFeedbackAsync(It.IsAny<int>(), default)).ThrowsAsync(new Exception("DB error"));
            var model = CreateModel();
            var result = await model.OnPostAsync(10);
            Assert.IsType<PageResult>(result);
            Assert.False(model.FeedbackSubmitted);
        }
    }

    public class ViewDoctorsModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<ViewDoctorsModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public ViewDoctorsModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<ViewDoctorsModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private ViewDoctorsModel CreateModel()
        {
            var model = new ViewDoctorsModel(_patientServiceMock.Object, _loggerMock.Object);
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public void Constructor_InitializesDefaultProperties()
        {
            var model = CreateModel();
            Assert.Equal(string.Empty, model.SelectedDepartment);
            Assert.NotNull(model.Departments);
            Assert.NotNull(model.Doctors);
        }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_LoadsDepartments()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDepartmentsAsync(default)).ReturnsAsync(new List<DepartmentDto> { new DepartmentDto { DeptNo = 1, DeptName = "Cardiology" } });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Single(new List<DepartmentOptionViewModel>(model.Departments));
        }

        [Fact]
        public async Task OnGetAsync_WithDepartment_LoadsDoctors()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDepartmentsAsync(default)).ReturnsAsync(new List<DepartmentDto> { new DepartmentDto { DeptNo = 1, DeptName = "Cardiology" } });
            _patientServiceMock.Setup(s => s.GetDoctorsByDepartmentAsync("Cardiology", default)).ReturnsAsync(new List<DoctorListDto> { new DoctorListDto { DoctorId = 1, Name = "Dr. Heart", Department = "Cardiology" } });
            var model = CreateModel();
            var result = await model.OnGetAsync("Cardiology");
            Assert.IsType<PageResult>(result);
            Assert.Equal("Cardiology", model.SelectedDepartment);
            Assert.Single(new List<DoctorListViewModel>(model.Doctors));
        }

        [Fact]
        public async Task OnGetAsync_NullDepartment_SetsEmptyString()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDepartmentsAsync(default)).ReturnsAsync(new List<DepartmentDto>());
            var model = CreateModel();
            var result = await model.OnGetAsync(null);
            Assert.IsType<PageResult>(result);
            Assert.Equal(string.Empty, model.SelectedDepartment);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDepartmentsAsync(default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync();
            Assert.IsType<PageResult>(result);
        }
    }

    public class PatientNotificationsModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<PatientNotificationsModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public PatientNotificationsModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<PatientNotificationsModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private PatientNotificationsModel CreateModel()
        {
            var model = new PatientNotificationsModel(_patientServiceMock.Object, _loggerMock.Object);
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public void Constructor_InitializesDefaultProperties()
        {
            var model = CreateModel();
            Assert.False(model.HasNotification);
            Assert.Equal(string.Empty, model.DoctorName);
            Assert.Equal(string.Empty, model.Timings);
        }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_WithNotification_LoadsNotification()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetNotificationsAsync(1, default)).ReturnsAsync(new NotificationDto { HasNotification = true, DoctorName = "Dr. Patel", Timings = "11:00 AM" });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.True(model.HasNotification);
            Assert.Equal("Dr. Patel", model.DoctorName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_NoNotification_DefaultValues()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetNotificationsAsync(1, default)).ReturnsAsync((NotificationDto?)null);
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.False(model.HasNotification);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetNotificationsAsync(1, default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync();
            Assert.IsType<PageResult>(result);
        }
    }

    public class BillsHistoryModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<BillsHistoryModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public BillsHistoryModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<BillsHistoryModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private BillsHistoryModel CreateModel()
        {
            var model = new BillsHistoryModel(_patientServiceMock.Object, _loggerMock.Object);
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_LoadsBills()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetBillHistoryAsync(1, default)).ReturnsAsync(new List<BillHistoryDto> { new BillHistoryDto { AppointId = 1, DoctorName = "Dr. X", BillAmount = 500, BillStatus = "Paid" } });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Single(new List<BillHistoryDto>(model.Bills));
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetBillHistoryAsync(1, default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync();
            Assert.IsType<PageResult>(result);
        }
    }

    public class CurrentAppointmentModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<CurrentAppointmentModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public CurrentAppointmentModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<CurrentAppointmentModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private CurrentAppointmentModel CreateModel()
        {
            var model = new CurrentAppointmentModel(_patientServiceMock.Object, _loggerMock.Object);
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync();
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_LoadsCurrentAppointment()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetCurrentAppointmentAsync(1, default)).ReturnsAsync(new CurrentAppointmentDto { DoctorName = "Dr. Lee", Timings = "2:00 PM" });
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.NotNull(model.CurrentAppointment);
            Assert.Equal("Dr. Lee", model.CurrentAppointment!.DoctorName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_NoCurrentAppointment_ReturnsNull()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetCurrentAppointmentAsync(1, default)).ReturnsAsync((CurrentAppointmentDto?)null);
            var model = CreateModel();
            var result = await model.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Null(model.CurrentAppointment);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetCurrentAppointmentAsync(1, default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync();
            Assert.IsType<PageResult>(result);
        }
    }

    public class DoctorProfileModelTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<ILogger<DoctorProfileModel>> _loggerMock;
        private readonly Mock<ISession> _sessionMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public DoctorProfileModelTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _loggerMock = new Mock<ILogger<DoctorProfileModel>>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.Setup(c => c.Session).Returns(_sessionMock.Object);
        }

        private DoctorProfileModel CreateModel()
        {
            var model = new DoctorProfileModel(_patientServiceMock.Object, _loggerMock.Object);
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
        public void Constructor_WithValidDependencies_CreatesInstance() { Assert.NotNull(CreateModel()); }

        [Fact]
        public async Task OnGetAsync_NoSession_RedirectsToSignUp()
        {
            SetupSession(null, null);
            var result = await CreateModel().OnGetAsync(1);
            Assert.Equal("/SignUp", Assert.IsType<RedirectToPageResult>(result).PageName);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_LoadsDoctorProfile()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDoctorProfileAsync(3, default)).ReturnsAsync(new DoctorProfileDto { DoctorId = 3, Name = "Dr. Brown", Department = "Orthopedics", ChargesPerVisit = 400, ReputeIndex = 4.5, PatientsTreated = 100, Qualification = "MBBS, MS", WorkExperience = 15, Age = 45, Gender = "Male" });
            var model = CreateModel();
            var result = await model.OnGetAsync(3);
            Assert.IsType<PageResult>(result);
            Assert.NotNull(model.Profile);
            Assert.Equal("Dr. Brown", model.Profile!.Name);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_NullProfile_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDoctorProfileAsync(99, default)).ReturnsAsync((DoctorProfileDto?)null);
            var model = CreateModel();
            var result = await model.OnGetAsync(99);
            Assert.IsType<PageResult>(result);
            Assert.Null(model.Profile);
        }

        [Fact]
        public async Task OnGetAsync_ServiceThrowsException_ReturnsPage()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDoctorProfileAsync(It.IsAny<int>(), default)).ThrowsAsync(new Exception("DB error"));
            var result = await CreateModel().OnGetAsync(3);
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_PatientUser_CallsServiceWithCorrectDoctorId()
        {
            SetupSession(1, 1);
            _patientServiceMock.Setup(s => s.GetDoctorProfileAsync(7, default)).ReturnsAsync(new DoctorProfileDto { DoctorId = 7, Name = "Dr. Seven" });
            await CreateModel().OnGetAsync(7);
            _patientServiceMock.Verify(s => s.GetDoctorProfileAsync(7, default), Times.Once);
        }
    }
}
