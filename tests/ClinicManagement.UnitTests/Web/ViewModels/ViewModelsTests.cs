using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using ClinicManagement.Web.ViewModels;

namespace ClinicManagement.Web.Tests.ViewModels
{
    public class ViewModelsTests
    {
        // ─── LoginViewModel ───────────────────────────────────────────────────────

        [Fact]
        public void LoginViewModel_DefaultValues_AreEmpty()
        {
            var vm = new LoginViewModel();
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
        }

        [Fact]
        public void LoginViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new LoginViewModel { Email = "test@test.com", Password = "pass123" };
            Assert.Equal("test@test.com", vm.Email);
            Assert.Equal("pass123", vm.Password);
        }

        [Fact]
        public void LoginViewModel_EmptyEmail_FailsValidation()
        {
            var vm = new LoginViewModel { Email = "", Password = "pass123" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void LoginViewModel_InvalidEmail_FailsValidation()
        {
            var vm = new LoginViewModel { Email = "not-an-email", Password = "pass123" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        }

        [Fact]
        public void LoginViewModel_EmptyPassword_FailsValidation()
        {
            var vm = new LoginViewModel { Email = "test@test.com", Password = "" };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void LoginViewModel_ValidData_PassesValidation()
        {
            var vm = new LoginViewModel { Email = "test@test.com", Password = "pass123" };
            var results = ValidateModel(vm);
            Assert.Empty(results);
        }

        // ─── RegisterViewModel ────────────────────────────────────────────────────

        [Fact]
        public void RegisterViewModel_DefaultValues_AreEmpty()
        {
            var vm = new RegisterViewModel();
            Assert.Equal(string.Empty, vm.Name);
            Assert.Equal(string.Empty, vm.BirthDate);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
            Assert.Equal(string.Empty, vm.PhoneNo);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(string.Empty, vm.Address);
        }

        [Fact]
        public void RegisterViewModel_ValidData_PassesValidation()
        {
            var vm = new RegisterViewModel
            {
                Name = "John Doe",
                BirthDate = "1990-01-01",
                Email = "john@example.com",
                Password = "pass123",
                PhoneNo = "1234567890",
                Gender = "Male",
                Address = "123 Main St"
            };
            var results = ValidateModel(vm);
            Assert.Empty(results);
        }

        [Fact]
        public void RegisterViewModel_NameTooLong_FailsValidation()
        {
            var vm = new RegisterViewModel
            {
                Name = new string('A', 31),
                BirthDate = "1990-01-01",
                Email = "john@example.com",
                Password = "pass123",
                PhoneNo = "1234567890",
                Gender = "Male",
                Address = "123 Main St"
            };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void RegisterViewModel_PasswordTooShort_FailsValidation()
        {
            var vm = new RegisterViewModel
            {
                Name = "John",
                BirthDate = "1990-01-01",
                Email = "john@example.com",
                Password = "abc",
                PhoneNo = "1234567890",
                Gender = "Male",
                Address = "123 Main St"
            };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Password"));
        }

        [Fact]
        public void RegisterViewModel_PhoneNoTooLong_FailsValidation()
        {
            var vm = new RegisterViewModel
            {
                Name = "John",
                BirthDate = "1990-01-01",
                Email = "john@example.com",
                Password = "pass123",
                PhoneNo = new string('1', 16),
                Gender = "Male",
                Address = "123 Main St"
            };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("PhoneNo"));
        }

        [Fact]
        public void RegisterViewModel_AddressTooLong_FailsValidation()
        {
            var vm = new RegisterViewModel
            {
                Name = "John",
                BirthDate = "1990-01-01",
                Email = "john@example.com",
                Password = "pass123",
                PhoneNo = "1234567890",
                Gender = "Male",
                Address = new string('A', 41)
            };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Address"));
        }

        // ─── AdminHomeViewModel ───────────────────────────────────────────────────

        [Fact]
        public void AdminHomeViewModel_DefaultValues_AreCorrect()
        {
            var vm = new AdminHomeViewModel();
            Assert.Equal(0, vm.TotalDoctors);
            Assert.Equal(0, vm.TotalPatients);
            Assert.Equal(0.0, vm.TotalIncome);
            Assert.NotNull(vm.Departments);
            Assert.NotNull(vm.Appointments);
        }

        [Fact]
        public void AdminHomeViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new AdminHomeViewModel
            {
                TotalDoctors = 10,
                TotalPatients = 50,
                TotalIncome = 12345.67,
                Departments = new List<DepartmentSummaryViewModel>
                {
                    new DepartmentSummaryViewModel { DeptName = "Cardiology", DoctorCount = 3 }
                },
                Appointments = new List<AppointmentSummaryViewModel>
                {
                    new AppointmentSummaryViewModel { AppointId = 1, PatientName = "Alice", DoctorName = "Dr. Bob", Status = "Pending" }
                }
            };
            Assert.Equal(10, vm.TotalDoctors);
            Assert.Equal(50, vm.TotalPatients);
            Assert.Equal(12345.67, vm.TotalIncome);
            Assert.Single(vm.Departments);
            Assert.Single(vm.Appointments);
        }

        // ─── DepartmentSummaryViewModel ───────────────────────────────────────────

        [Fact]
        public void DepartmentSummaryViewModel_DefaultValues_AreCorrect()
        {
            var vm = new DepartmentSummaryViewModel();
            Assert.Equal(string.Empty, vm.DeptName);
            Assert.Equal(0, vm.DoctorCount);
            Assert.Null(vm.Description);
        }

        [Fact]
        public void DepartmentSummaryViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new DepartmentSummaryViewModel
            {
                DeptName = "Neurology",
                DoctorCount = 5,
                Description = "Brain specialists"
            };
            Assert.Equal("Neurology", vm.DeptName);
            Assert.Equal(5, vm.DoctorCount);
            Assert.Equal("Brain specialists", vm.Description);
        }

        // ─── AppointmentSummaryViewModel ──────────────────────────────────────────

        [Fact]
        public void AppointmentSummaryViewModel_DefaultValues_AreCorrect()
        {
            var vm = new AppointmentSummaryViewModel();
            Assert.Equal(0, vm.AppointId);
            Assert.Equal(string.Empty, vm.PatientName);
            Assert.Equal(string.Empty, vm.DoctorName);
            Assert.Null(vm.Date);
            Assert.Equal(string.Empty, vm.Status);
        }

        [Fact]
        public void AppointmentSummaryViewModel_SetProperties_ReturnCorrectValues()
        {
            var date = new DateTime(2024, 1, 15);
            var vm = new AppointmentSummaryViewModel
            {
                AppointId = 42,
                PatientName = "Alice",
                DoctorName = "Dr. Smith",
                Date = date,
                Status = "Approved"
            };
            Assert.Equal(42, vm.AppointId);
            Assert.Equal("Alice", vm.PatientName);
            Assert.Equal("Dr. Smith", vm.DoctorName);
            Assert.Equal(date, vm.Date);
            Assert.Equal("Approved", vm.Status);
        }

        // ─── AddDoctorViewModel ───────────────────────────────────────────────────

        [Fact]
        public void AddDoctorViewModel_DefaultValues_AreCorrect()
        {
            var vm = new AddDoctorViewModel();
            Assert.Equal(string.Empty, vm.Name);
            Assert.Equal(string.Empty, vm.Email);
            Assert.Equal(string.Empty, vm.Password);
            Assert.Equal(0, vm.DeptNo);
            Assert.Equal(0, vm.WorkExperience);
            Assert.Equal(0, vm.Salary);
            Assert.Equal(0, vm.ChargesPerVisit);
            Assert.NotNull(vm.Departments);
        }

        [Fact]
        public void AddDoctorViewModel_ValidData_PassesValidation()
        {
            var vm = new AddDoctorViewModel
            {
                Name = "Dr. Jane",
                Email = "jane@clinic.com",
                Password = "pass123",
                BirthDate = "1980-05-10",
                DeptNo = 1,
                Phone = "9876543210",
                Gender = 'F',
                Address = "456 Oak Ave",
                WorkExperience = 10,
                Salary = 80000,
                ChargesPerVisit = 500,
                Specialization = "Cardiology",
                Qualification = "MBBS, MD"
            };
            var results = ValidateModel(vm);
            Assert.Empty(results);
        }

        [Fact]
        public void AddDoctorViewModel_WorkExperienceOutOfRange_FailsValidation()
        {
            var vm = new AddDoctorViewModel
            {
                Name = "Dr. Jane",
                Email = "jane@clinic.com",
                Password = "pass123",
                BirthDate = "1980-05-10",
                DeptNo = 1,
                Phone = "9876543210",
                Gender = 'F',
                Address = "456 Oak Ave",
                WorkExperience = 51,
                Salary = 80000,
                ChargesPerVisit = 500,
                Qualification = "MBBS"
            };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("WorkExperience"));
        }

        [Fact]
        public void AddDoctorViewModel_NegativeSalary_FailsValidation()
        {
            var vm = new AddDoctorViewModel
            {
                Name = "Dr. Jane",
                Email = "jane@clinic.com",
                Password = "pass123",
                BirthDate = "1980-05-10",
                DeptNo = 1,
                Phone = "9876543210",
                Gender = 'F',
                Address = "456 Oak Ave",
                WorkExperience = 5,
                Salary = -1,
                ChargesPerVisit = 500,
                Qualification = "MBBS"
            };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Salary"));
        }

        // ─── DepartmentOptionViewModel ────────────────────────────────────────────

        [Fact]
        public void DepartmentOptionViewModel_DefaultValues_AreCorrect()
        {
            var vm = new DepartmentOptionViewModel();
            Assert.Equal(0, vm.DeptNo);
            Assert.Equal(string.Empty, vm.DeptName);
        }

        [Fact]
        public void DepartmentOptionViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new DepartmentOptionViewModel { DeptNo = 3, DeptName = "Orthopedics" };
            Assert.Equal(3, vm.DeptNo);
            Assert.Equal("Orthopedics", vm.DeptName);
        }

        // ─── AddStaffViewModel ────────────────────────────────────────────────────

        [Fact]
        public void AddStaffViewModel_DefaultValues_AreCorrect()
        {
            var vm = new AddStaffViewModel();
            Assert.Equal(string.Empty, vm.Name);
            Assert.Equal(string.Empty, vm.BirthDate);
            Assert.Equal(string.Empty, vm.Phone);
            Assert.Equal(string.Empty, vm.Address);
            Assert.Equal(0, vm.Salary);
            Assert.Equal(string.Empty, vm.Qualification);
            Assert.Equal(string.Empty, vm.Designation);
        }

        [Fact]
        public void AddStaffViewModel_ValidData_PassesValidation()
        {
            var vm = new AddStaffViewModel
            {
                Name = "John Staff",
                BirthDate = "1985-03-20",
                Phone = "1234567890",
                Gender = 'M',
                Address = "789 Pine Rd",
                Salary = 30000,
                Qualification = "BSc Nursing",
                Designation = "Nurse"
            };
            var results = ValidateModel(vm);
            Assert.Empty(results);
        }

        [Fact]
        public void AddStaffViewModel_DesignationTooLong_FailsValidation()
        {
            var vm = new AddStaffViewModel
            {
                Name = "John Staff",
                BirthDate = "1985-03-20",
                Phone = "1234567890",
                Gender = 'M',
                Address = "789 Pine Rd",
                Salary = 30000,
                Qualification = "BSc",
                Designation = new string('X', 16)
            };
            var results = ValidateModel(vm);
            Assert.Contains(results, r => r.MemberNames.Contains("Designation"));
        }

        // ─── ManageClinicViewModel ────────────────────────────────────────────────

        [Fact]
        public void ManageClinicViewModel_DefaultValues_AreCorrect()
        {
            var vm = new ManageClinicViewModel();
            Assert.Equal(string.Empty, vm.SearchQuery);
            Assert.Equal("doctors", vm.ActiveTab);
            Assert.NotNull(vm.Doctors);
            Assert.NotNull(vm.Patients);
            Assert.NotNull(vm.Staff);
        }

        [Fact]
        public void ManageClinicViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new ManageClinicViewModel
            {
                SearchQuery = "Smith",
                ActiveTab = "patients",
                Doctors = new List<DoctorListViewModel> { new DoctorListViewModel { DoctorId = 1, Name = "Dr. Smith" } },
                Patients = new List<PatientListViewModel> { new PatientListViewModel { PatientId = 2, Name = "Alice" } },
                Staff = new List<StaffListViewModel> { new StaffListViewModel { StaffId = 3, Name = "Bob", Designation = "Nurse" } }
            };
            Assert.Equal("Smith", vm.SearchQuery);
            Assert.Equal("patients", vm.ActiveTab);
            Assert.Single(vm.Doctors);
            Assert.Single(vm.Patients);
            Assert.Single(vm.Staff);
        }

        // ─── DoctorListViewModel ──────────────────────────────────────────────────

        [Fact]
        public void DoctorListViewModel_DefaultValues_AreCorrect()
        {
            var vm = new DoctorListViewModel();
            Assert.Equal(0, vm.DoctorId);
            Assert.Equal(string.Empty, vm.Name);
            Assert.Equal(string.Empty, vm.Department);
            Assert.Null(vm.Phone);
            Assert.Equal(0.0, vm.ChargesPerVisit);
        }

        [Fact]
        public void DoctorListViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new DoctorListViewModel
            {
                DoctorId = 7,
                Name = "Dr. Adams",
                Department = "Cardiology",
                Phone = "5551234567",
                ChargesPerVisit = 300.0
            };
            Assert.Equal(7, vm.DoctorId);
            Assert.Equal("Dr. Adams", vm.Name);
            Assert.Equal("Cardiology", vm.Department);
            Assert.Equal("5551234567", vm.Phone);
            Assert.Equal(300.0, vm.ChargesPerVisit);
        }

        // ─── PatientListViewModel ─────────────────────────────────────────────────

        [Fact]
        public void PatientListViewModel_DefaultValues_AreCorrect()
        {
            var vm = new PatientListViewModel();
            Assert.Equal(0, vm.PatientId);
            Assert.Equal(string.Empty, vm.Name);
            Assert.Null(vm.Phone);
        }

        [Fact]
        public void PatientListViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new PatientListViewModel { PatientId = 5, Name = "Carol", Phone = "9998887777" };
            Assert.Equal(5, vm.PatientId);
            Assert.Equal("Carol", vm.Name);
            Assert.Equal("9998887777", vm.Phone);
        }

        // ─── StaffListViewModel ───────────────────────────────────────────────────

        [Fact]
        public void StaffListViewModel_DefaultValues_AreCorrect()
        {
            var vm = new StaffListViewModel();
            Assert.Equal(0, vm.StaffId);
            Assert.Equal(string.Empty, vm.Name);
            Assert.Equal(string.Empty, vm.Designation);
        }

        [Fact]
        public void StaffListViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new StaffListViewModel { StaffId = 9, Name = "Dave", Designation = "Receptionist" };
            Assert.Equal(9, vm.StaffId);
            Assert.Equal("Dave", vm.Name);
            Assert.Equal("Receptionist", vm.Designation);
        }

        // ─── PatientHomeViewModel ─────────────────────────────────────────────────

        [Fact]
        public void PatientHomeViewModel_DefaultValues_AreCorrect()
        {
            var vm = new PatientHomeViewModel();
            Assert.Equal(string.Empty, vm.Name);
            Assert.Null(vm.Phone);
            Assert.Null(vm.Address);
            Assert.Equal(string.Empty, vm.BirthDate);
            Assert.Equal(0, vm.Age);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Null(vm.CurrentAppointment);
        }

        [Fact]
        public void PatientHomeViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new PatientHomeViewModel
            {
                Name = "Eve",
                Phone = "1112223333",
                Address = "100 Elm St",
                BirthDate = "1995-07-22",
                Age = 29,
                Gender = "Female",
                CurrentAppointment = new CurrentAppointmentViewModel { DoctorName = "Dr. Lee", Timings = "10:00 AM" }
            };
            Assert.Equal("Eve", vm.Name);
            Assert.Equal("1112223333", vm.Phone);
            Assert.Equal("100 Elm St", vm.Address);
            Assert.Equal("1995-07-22", vm.BirthDate);
            Assert.Equal(29, vm.Age);
            Assert.Equal("Female", vm.Gender);
            Assert.NotNull(vm.CurrentAppointment);
            Assert.Equal("Dr. Lee", vm.CurrentAppointment.DoctorName);
        }

        // ─── CurrentAppointmentViewModel ──────────────────────────────────────────

        [Fact]
        public void CurrentAppointmentViewModel_DefaultValues_AreCorrect()
        {
            var vm = new CurrentAppointmentViewModel();
            Assert.Equal(string.Empty, vm.DoctorName);
            Assert.Equal(string.Empty, vm.Timings);
        }

        [Fact]
        public void CurrentAppointmentViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new CurrentAppointmentViewModel { DoctorName = "Dr. Kim", Timings = "2:00 PM" };
            Assert.Equal("Dr. Kim", vm.DoctorName);
            Assert.Equal("2:00 PM", vm.Timings);
        }

        // ─── BillHistoryViewModel ─────────────────────────────────────────────────

        [Fact]
        public void BillHistoryViewModel_DefaultValues_AreCorrect()
        {
            var vm = new BillHistoryViewModel();
            Assert.NotNull(vm.Bills);
            Assert.Empty(vm.Bills);
        }

        [Fact]
        public void BillHistoryViewModel_SetBills_ReturnCorrectValues()
        {
            var vm = new BillHistoryViewModel
            {
                Bills = new List<BillHistoryItemViewModel>
                {
                    new BillHistoryItemViewModel { AppointId = 1, DoctorName = "Dr. X", BillAmount = 500.0, BillStatus = "Paid" }
                }
            };
            Assert.Single(vm.Bills);
        }

        // ─── BillHistoryItemViewModel ─────────────────────────────────────────────

        [Fact]
        public void BillHistoryItemViewModel_DefaultValues_AreCorrect()
        {
            var vm = new BillHistoryItemViewModel();
            Assert.Equal(0, vm.AppointId);
            Assert.Equal(string.Empty, vm.DoctorName);
            Assert.Null(vm.Date);
            Assert.Null(vm.BillAmount);
            Assert.Null(vm.BillStatus);
        }

        [Fact]
        public void BillHistoryItemViewModel_SetProperties_ReturnCorrectValues()
        {
            var date = new DateTime(2024, 3, 10);
            var vm = new BillHistoryItemViewModel
            {
                AppointId = 11,
                DoctorName = "Dr. Y",
                Date = date,
                BillAmount = 750.0,
                BillStatus = "Unpaid"
            };
            Assert.Equal(11, vm.AppointId);
            Assert.Equal("Dr. Y", vm.DoctorName);
            Assert.Equal(date, vm.Date);
            Assert.Equal(750.0, vm.BillAmount);
            Assert.Equal("Unpaid", vm.BillStatus);
        }

        // ─── TreatmentHistoryViewModel ────────────────────────────────────────────

        [Fact]
        public void TreatmentHistoryViewModel_DefaultValues_AreCorrect()
        {
            var vm = new TreatmentHistoryViewModel();
            Assert.NotNull(vm.Treatments);
            Assert.Empty(vm.Treatments);
        }

        [Fact]
        public void TreatmentHistoryItemViewModel_DefaultValues_AreCorrect()
        {
            var vm = new TreatmentHistoryItemViewModel();
            Assert.Equal(0, vm.AppointId);
            Assert.Equal(string.Empty, vm.DoctorName);
            Assert.Null(vm.Date);
            Assert.Null(vm.Disease);
            Assert.Null(vm.Progress);
            Assert.Null(vm.Prescription);
        }

        [Fact]
        public void TreatmentHistoryItemViewModel_SetProperties_ReturnCorrectValues()
        {
            var date = new DateTime(2024, 2, 5);
            var vm = new TreatmentHistoryItemViewModel
            {
                AppointId = 20,
                DoctorName = "Dr. Z",
                Date = date,
                Disease = "Flu",
                Progress = "Recovering",
                Prescription = "Paracetamol"
            };
            Assert.Equal(20, vm.AppointId);
            Assert.Equal("Dr. Z", vm.DoctorName);
            Assert.Equal(date, vm.Date);
            Assert.Equal("Flu", vm.Disease);
            Assert.Equal("Recovering", vm.Progress);
            Assert.Equal("Paracetamol", vm.Prescription);
        }

        // ─── ViewDoctorsViewModel ─────────────────────────────────────────────────

        [Fact]
        public void ViewDoctorsViewModel_DefaultValues_AreCorrect()
        {
            var vm = new ViewDoctorsViewModel();
            Assert.Equal(string.Empty, vm.SelectedDepartment);
            Assert.NotNull(vm.Departments);
            Assert.NotNull(vm.Doctors);
        }

        [Fact]
        public void ViewDoctorsViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new ViewDoctorsViewModel
            {
                SelectedDepartment = "Cardiology",
                Departments = new List<DepartmentOptionViewModel> { new DepartmentOptionViewModel { DeptNo = 1, DeptName = "Cardiology" } },
                Doctors = new List<DoctorListViewModel> { new DoctorListViewModel { DoctorId = 1, Name = "Dr. Heart" } }
            };
            Assert.Equal("Cardiology", vm.SelectedDepartment);
            Assert.Single(vm.Departments);
            Assert.Single(vm.Doctors);
        }

        // ─── DoctorProfileViewModel ───────────────────────────────────────────────

        [Fact]
        public void DoctorProfileViewModel_DefaultValues_AreCorrect()
        {
            var vm = new DoctorProfileViewModel();
            Assert.Equal(0, vm.DoctorId);
            Assert.Equal(string.Empty, vm.Name);
            Assert.Null(vm.Phone);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(0.0, vm.ChargesPerVisit);
            Assert.Equal(0.0, vm.ReputeIndex);
            Assert.Equal(0, vm.PatientsTreated);
            Assert.Equal(string.Empty, vm.Qualification);
            Assert.Null(vm.Specialization);
            Assert.Equal(0, vm.WorkExperience);
            Assert.Equal(0, vm.Age);
            Assert.Equal(string.Empty, vm.Department);
        }

        [Fact]
        public void DoctorProfileViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new DoctorProfileViewModel
            {
                DoctorId = 3,
                Name = "Dr. Brown",
                Phone = "4445556666",
                Gender = "Male",
                ChargesPerVisit = 400.0,
                ReputeIndex = 4.5,
                PatientsTreated = 200,
                Qualification = "MBBS, MS",
                Specialization = "Orthopedics",
                WorkExperience = 15,
                Age = 45,
                Department = "Orthopedics"
            };
            Assert.Equal(3, vm.DoctorId);
            Assert.Equal("Dr. Brown", vm.Name);
            Assert.Equal(4.5, vm.ReputeIndex);
            Assert.Equal(200, vm.PatientsTreated);
        }

        // ─── TakeAppointmentViewModel ─────────────────────────────────────────────

        [Fact]
        public void TakeAppointmentViewModel_DefaultValues_AreCorrect()
        {
            var vm = new TakeAppointmentViewModel();
            Assert.Equal(0, vm.DoctorId);
            Assert.Equal(string.Empty, vm.DoctorName);
            Assert.NotNull(vm.FreeSlots);
        }

        [Fact]
        public void FreeSlotViewModel_DefaultValues_AreCorrect()
        {
            var vm = new FreeSlotViewModel();
            Assert.Equal(0, vm.SlotId);
            Assert.Equal(string.Empty, vm.Timings);
            Assert.Equal(default(DateTime), vm.Date);
        }

        [Fact]
        public void FreeSlotViewModel_SetProperties_ReturnCorrectValues()
        {
            var date = new DateTime(2024, 6, 1);
            var vm = new FreeSlotViewModel { SlotId = 5, Timings = "9:00 AM", Date = date };
            Assert.Equal(5, vm.SlotId);
            Assert.Equal("9:00 AM", vm.Timings);
            Assert.Equal(date, vm.Date);
        }

        // ─── AppointmentRequestViewModel ──────────────────────────────────────────

        [Fact]
        public void AppointmentRequestViewModel_DefaultValues_AreCorrect()
        {
            var vm = new AppointmentRequestViewModel();
            Assert.False(vm.Success);
            Assert.Equal(string.Empty, vm.Message);
        }

        [Fact]
        public void AppointmentRequestViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new AppointmentRequestViewModel { Success = true, Message = "Booked!" };
            Assert.True(vm.Success);
            Assert.Equal("Booked!", vm.Message);
        }

        // ─── PatientNotificationsViewModel ────────────────────────────────────────

        [Fact]
        public void PatientNotificationsViewModel_DefaultValues_AreCorrect()
        {
            var vm = new PatientNotificationsViewModel();
            Assert.False(vm.HasNotification);
            Assert.Equal(string.Empty, vm.DoctorName);
            Assert.Equal(string.Empty, vm.Timings);
        }

        [Fact]
        public void PatientNotificationsViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new PatientNotificationsViewModel { HasNotification = true, DoctorName = "Dr. Patel", Timings = "11:00 AM" };
            Assert.True(vm.HasNotification);
            Assert.Equal("Dr. Patel", vm.DoctorName);
            Assert.Equal("11:00 AM", vm.Timings);
        }

        // ─── PatientFeedbackViewModel ─────────────────────────────────────────────

        [Fact]
        public void PatientFeedbackViewModel_DefaultValues_AreCorrect()
        {
            var vm = new PatientFeedbackViewModel();
            Assert.False(vm.HasPendingFeedback);
            Assert.Equal(0, vm.AppointmentId);
            Assert.Equal(string.Empty, vm.DoctorName);
            Assert.Equal(string.Empty, vm.Timings);
            Assert.False(vm.FeedbackSubmitted);
        }

        [Fact]
        public void PatientFeedbackViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new PatientFeedbackViewModel
            {
                HasPendingFeedback = true,
                AppointmentId = 99,
                DoctorName = "Dr. Green",
                Timings = "3:00 PM",
                FeedbackSubmitted = true
            };
            Assert.True(vm.HasPendingFeedback);
            Assert.Equal(99, vm.AppointmentId);
            Assert.Equal("Dr. Green", vm.DoctorName);
            Assert.Equal("3:00 PM", vm.Timings);
            Assert.True(vm.FeedbackSubmitted);
        }

        // ─── DoctorHomeViewModel ──────────────────────────────────────────────────

        [Fact]
        public void DoctorHomeViewModel_DefaultValues_AreCorrect()
        {
            var vm = new DoctorHomeViewModel();
            Assert.Equal(0, vm.DoctorId);
            Assert.Equal(string.Empty, vm.Name);
            Assert.Null(vm.Phone);
            Assert.Null(vm.Address);
            Assert.Equal(string.Empty, vm.Gender);
            Assert.Equal(string.Empty, vm.Department);
            Assert.Equal(0.0, vm.ChargesPerVisit);
            Assert.Null(vm.MonthlySalary);
            Assert.Null(vm.ReputeIndex);
            Assert.Equal(0, vm.PatientsTreated);
            Assert.Equal(string.Empty, vm.Qualification);
            Assert.Null(vm.Specialization);
            Assert.Null(vm.WorkExperience);
            Assert.Equal(0, vm.Age);
            Assert.Equal(string.Empty, vm.BirthDate);
        }

        [Fact]
        public void DoctorHomeViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new DoctorHomeViewModel
            {
                DoctorId = 2,
                Name = "Dr. White",
                Phone = "7778889999",
                Address = "200 Maple Ave",
                Gender = "Male",
                Department = "Neurology",
                ChargesPerVisit = 600.0,
                MonthlySalary = 90000.0,
                ReputeIndex = 4.8,
                PatientsTreated = 500,
                Qualification = "MBBS, DM",
                Specialization = "Neurology",
                WorkExperience = 20,
                Age = 50,
                BirthDate = "1974-01-01"
            };
            Assert.Equal(2, vm.DoctorId);
            Assert.Equal("Dr. White", vm.Name);
            Assert.Equal(4.8, vm.ReputeIndex);
            Assert.Equal(500, vm.PatientsTreated);
        }

        // ─── PendingAppointmentViewModel ──────────────────────────────────────────

        [Fact]
        public void PendingAppointmentViewModel_DefaultValues_AreCorrect()
        {
            var vm = new PendingAppointmentViewModel();
            Assert.NotNull(vm.Appointments);
            Assert.Empty(vm.Appointments);
        }

        // ─── AppointmentItemViewModel ─────────────────────────────────────────────

        [Fact]
        public void AppointmentItemViewModel_DefaultValues_AreCorrect()
        {
            var vm = new AppointmentItemViewModel();
            Assert.Equal(0, vm.AppointId);
            Assert.Equal(string.Empty, vm.PatientName);
            Assert.Null(vm.Date);
            Assert.Equal(string.Empty, vm.Status);
            Assert.Null(vm.Disease);
            Assert.Null(vm.Progress);
            Assert.Null(vm.Prescription);
        }

        [Fact]
        public void AppointmentItemViewModel_SetProperties_ReturnCorrectValues()
        {
            var date = new DateTime(2024, 4, 20);
            var vm = new AppointmentItemViewModel
            {
                AppointId = 55,
                PatientName = "Frank",
                Date = date,
                Status = "Completed",
                Disease = "Hypertension",
                Progress = "Stable",
                Prescription = "Amlodipine"
            };
            Assert.Equal(55, vm.AppointId);
            Assert.Equal("Frank", vm.PatientName);
            Assert.Equal(date, vm.Date);
            Assert.Equal("Completed", vm.Status);
            Assert.Equal("Hypertension", vm.Disease);
        }

        // ─── HistoryUpdateViewModel ───────────────────────────────────────────────

        [Fact]
        public void HistoryUpdateViewModel_DefaultValues_AreCorrect()
        {
            var vm = new HistoryUpdateViewModel();
            Assert.Equal(0, vm.AppointmentId);
            Assert.Equal(string.Empty, vm.PatientName);
            Assert.Equal(string.Empty, vm.Disease);
            Assert.Equal(string.Empty, vm.Progress);
            Assert.Equal(string.Empty, vm.Prescription);
            Assert.NotNull(vm.TodaysAppointments);
        }

        [Fact]
        public void HistoryUpdateViewModel_SetProperties_ReturnCorrectValues()
        {
            var vm = new HistoryUpdateViewModel
            {
                AppointmentId = 77,
                PatientName = "Grace",
                Disease = "Diabetes",
                Progress = "Improving",
                Prescription = "Metformin",
                TodaysAppointments = new List<AppointmentItemViewModel>
                {
                    new AppointmentItemViewModel { AppointId = 77, PatientName = "Grace" }
                }
            };
            Assert.Equal(77, vm.AppointmentId);
            Assert.Equal("Grace", vm.PatientName);
            Assert.Equal("Diabetes", vm.Disease);
            Assert.Single(vm.TodaysAppointments);
        }

        // ─── BillViewModel ────────────────────────────────────────────────────────

        [Fact]
        public void BillViewModel_DefaultValues_AreCorrect()
        {
            var vm = new BillViewModel();
            Assert.NotNull(vm.Bills);
            Assert.Empty(vm.Bills);
        }

        // ─── BillItemViewModel ────────────────────────────────────────────────────

        [Fact]
        public void BillItemViewModel_DefaultValues_AreCorrect()
        {
            var vm = new BillItemViewModel();
            Assert.Equal(0, vm.AppointId);
            Assert.Equal(string.Empty, vm.PatientName);
            Assert.Null(vm.Date);
            Assert.Null(vm.BillAmount);
            Assert.Null(vm.BillStatus);
        }

        [Fact]
        public void BillItemViewModel_SetProperties_ReturnCorrectValues()
        {
            var date = new DateTime(2024, 5, 15);
            var vm = new BillItemViewModel
            {
                AppointId = 33,
                PatientName = "Henry",
                Date = date,
                BillAmount = 1200.0,
                BillStatus = "Paid"
            };
            Assert.Equal(33, vm.AppointId);
            Assert.Equal("Henry", vm.PatientName);
            Assert.Equal(1200.0, vm.BillAmount);
            Assert.Equal("Paid", vm.BillStatus);
        }

        // ─── PreviousHistoryViewModel ─────────────────────────────────────────────

        [Fact]
        public void PreviousHistoryViewModel_DefaultValues_AreCorrect()
        {
            var vm = new PreviousHistoryViewModel();
            Assert.NotNull(vm.History);
            Assert.Empty(vm.History);
        }

        // ─── PatientHistoryItemViewModel ──────────────────────────────────────────

        [Fact]
        public void PatientHistoryItemViewModel_DefaultValues_AreCorrect()
        {
            var vm = new PatientHistoryItemViewModel();
            Assert.Equal(0, vm.AppointId);
            Assert.Equal(string.Empty, vm.PatientName);
            Assert.Null(vm.Date);
            Assert.Null(vm.Disease);
            Assert.Null(vm.Progress);
            Assert.Null(vm.Prescription);
            Assert.Null(vm.BillStatus);
        }

        [Fact]
        public void PatientHistoryItemViewModel_SetProperties_ReturnCorrectValues()
        {
            var date = new DateTime(2024, 1, 10);
            var vm = new PatientHistoryItemViewModel
            {
                AppointId = 88,
                PatientName = "Iris",
                Date = date,
                Disease = "Asthma",
                Progress = "Controlled",
                Prescription = "Inhaler",
                BillStatus = "Paid"
            };
            Assert.Equal(88, vm.AppointId);
            Assert.Equal("Iris", vm.PatientName);
            Assert.Equal("Asthma", vm.Disease);
            Assert.Equal("Paid", vm.BillStatus);
        }

        // ─── Helper ───────────────────────────────────────────────────────────────

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, results, true);
            return results;
        }
    }
}
