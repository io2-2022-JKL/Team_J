using System;
using Xunit;
using Moq;
using VaccinationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VaccinationSystem.Config;
using VaccinationSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.PatientDTOs;
using VaccinationSystem.DTO.AdminDTOs;

namespace VaccinationSystem.UnitTests
{
    public class AdminControllersTests : DataMock
    {
        [Fact]
        public void GetPatientsTest()
        {
            // Arrange

            var patientsData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientsData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.GetPatients();

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<PatientDTO>>(list.Value);

            var patients = list.Value as List<PatientDTO>;
            Assert.Equal(patientsData.Count(), patients.Count);

            for (int i = 0; i < patientsData.Count(); i++)
            {
                var patient = patientsData[i];
                var patientDTO = patients[i];
                Assert.Equal(patient.Id.ToString(), patientDTO.id);
                Assert.Equal(patient.PESEL, patientDTO.PESEL);
                Assert.Equal(patient.FirstName, patientDTO.firstName);
                Assert.Equal(patient.LastName, patientDTO.lastName);
                Assert.Equal(patient.DateOfBirth.ToString("dd-MM-yyyy"), patientDTO.dateOfBirth);
                Assert.Equal(patient.Mail, patientDTO.mail);
                Assert.Equal(patient.PhoneNumber, patientDTO.phoneNumber);
                Assert.Equal(patient.Active, patientDTO.active);
            }

        }

        [Fact]
        public void GetPatientsEmptyTest()
        {
            // Arrange

            var mockSet = GetMock(new List<Patient>().AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(mockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.GetPatients();

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<PatientDTO>>(list.Value);

            var patients = list.Value as List<PatientDTO>;
            Assert.Empty(patients);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", 0)]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b", 3)]
        public void DeletePatientTest(string patientId, int index)
        {
            // Arrange
            var patientsData = GetPatientsData();
            var patientMockSet = GetMock(patientsData);

            var doctorsData = GetDoctorsData();
            var doctorMockSet = GetMock(doctorsData);
            var timeSlotsData = GetTimeSlotsData();
            var timeSlotMockSet = GetMock(timeSlotsData);
            var appointmentsData = GetAppointmentsData();
            var appointmentMockSet = GetMock(appointmentsData);

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeletePatient(patientId);

            // Assert

            Assert.IsType<OkResult>(result);
            Assert.Equal(GetPatientsData().Count(), patientsData.ToList().Count);
            Assert.False(patientsData.ToList()[index].Active);
            Assert.Empty(doctorsData.Where(doc => doc.PatientId == Guid.Parse(patientId) && doc.Active == true));
            Assert.Empty(timeSlotsData.Where(ts => ts.Doctor.PatientId == Guid.Parse(patientId) && ts.Active == true));
            Assert.Empty(appointmentsData.Where(a => a.TimeSlot.Doctor.PatientId == Guid.Parse(patientId) && a.State == AppointmentState.Planned));

        }

        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9f")]
        [InlineData("wrong_guid")]
        [InlineData("81a130d2-502f-4cf1-aaaa-63edeb000e9b")]
        [InlineData("")]
        [InlineData(null)]
        public void DeleteWrongPatientTest(string patientId)
        {
            // Arrange

            var patientMockSet = GetMock(GetPatientsData());
            var doctorMockSet = GetMock(GetDoctorsData());
            var timeSlotMockSet = GetMock(new List<TimeSlot>().AsQueryable());
            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeletePatient(patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public void GetDoctorsTest()
        {
            // Arrange

            var doctorsDataList = GetDoctorsData().ToList();

            var patientMockSet = GetMock(GetPatientsData());
            var vaccinationCenterMockSet = GetMock(GetVaccinationCentersData());
            var doctorMockSet = GetMock(GetDoctorsData());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.GetDoctors();

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<GetDoctorsResponseDTO>>(list.Value);

            var doctors = list.Value as List<GetDoctorsResponseDTO>;
            Assert.Equal(doctorsDataList.Count(), doctors.Count);

            for (int i = 0; i < doctorsDataList.Count(); i++)
            {
                Assert.Equal(doctorsDataList[i].Id.ToString(), doctors[i].id);
                Assert.Equal(doctorsDataList[i].PatientAccount.PESEL, doctors[i].PESEL);
                Assert.Equal(doctorsDataList[i].PatientAccount.FirstName, doctors[i].firstName);
                Assert.Equal(doctorsDataList[i].PatientAccount.LastName, doctors[i].lastName);
                Assert.Equal(doctorsDataList[i].PatientAccount.DateOfBirth.ToString("dd-MM-yyyy"), doctors[i].dateOfBirth);
                Assert.Equal(doctorsDataList[i].PatientAccount.Mail, doctors[i].mail);
                Assert.Equal(doctorsDataList[i].PatientAccount.PhoneNumber, doctors[i].phoneNumber);
                Assert.Equal(doctorsDataList[i].Active, doctors[i].active);
                Assert.Equal(doctorsDataList[i].VaccinationCenterId.ToString(), doctors[i].vaccinationCenterId);
                Assert.Equal(doctorsDataList[i].VaccinationCenter.Name, doctors[i].name);
                Assert.Equal(doctorsDataList[i].VaccinationCenter.City, doctors[i].city);
                Assert.Equal(doctorsDataList[i].VaccinationCenter.Address, doctors[i].street);
            }

        }

        [Fact]
        public void GetDoctorsEmptyTest()
        {
            // Arrange

            var patientMockSet = GetMock(GetPatientsData());
            var vaccinationCenterMockSet = GetMock(GetVaccinationCentersData());
            var doctorMockSet = GetMock(new List<Doctor>().AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.GetDoctors();

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<GetDoctorsResponseDTO>>(list.Value);

            var doctors = list.Value as List<GetDoctorsResponseDTO>;
            Assert.Empty(doctors);

        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "e0d50915-5548-4993-9204-edddab4e1dff", 0)]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9a", "e1d50915-5548-4993-9204-edddab4e1dff", 2)]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b", "e1d50915-5548-4993-9204-edddab4e1dff", 3)]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "e0d50915-5548-4993-9204-edddab4e1dff", 4)]
        public void AddDoctorToEmptyListTest(string doctorId, string vaccinationCenterId, int index)
        {
            // Arrange
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(GetPatientsData());
            var vaccinationCenterMockSet = GetMock(GetVaccinationCentersData());
            var doctorData = new List<Doctor>();
            var doctorMockSet = GetMock(doctorData.AsQueryable());
            doctorMockSet.Setup(c => c.Add(It.IsAny<Doctor>())).Callback(delegate (Doctor doc) {
                doctorData.Add(doc);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var request = new AddDoctorRequestDTO { doctorId = doctorId, vaccinationCenterId = vaccinationCenterId };

            // Act

            var result = controller.AddDoctor(request);

            // Assert

            Assert.IsType<OkResult>(result);

            Assert.Single(doctorData);
            Assert.Equal(patientData[index].Id, doctorData[0].PatientId);
            Assert.Equal(Guid.Parse(doctorId), doctorData[0].PatientId);
            Assert.Equal(patientData[index].PESEL, doctorData[0].PatientAccount.PESEL);
            Assert.Equal(patientData[index].FirstName, doctorData[0].PatientAccount.FirstName);
            Assert.Equal(patientData[index].LastName, doctorData[0].PatientAccount.LastName);
            Assert.Equal(patientData[index].DateOfBirth, doctorData[0].PatientAccount.DateOfBirth);
            Assert.Equal(patientData[index].Mail, doctorData[0].PatientAccount.Mail);
            Assert.Equal(patientData[index].Password, doctorData[0].PatientAccount.Password);
            Assert.Equal(patientData[index].PhoneNumber, doctorData[0].PatientAccount.PhoneNumber);
            //Assert.Equal(patientData[index].Vaccinations.Count(), doctorData[0].PatientAccount.Vaccinations.Count());
            //Assert.Equal(patientData[index].Certificates.Count(), doctorData[0].PatientAccount.Certificates.Count());
            Assert.True(doctorData[0].PatientAccount.Active);
            Assert.True(doctorData[0].Active);
            Assert.Equal(Guid.Parse(vaccinationCenterId), doctorData[0].VaccinationCenterId);
            //Assert.Empty(doctorData[0].Vaccinations);
        }

        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b", "e1d50915-5548-4993-9204-edddab4e1dff", 3)]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "e0d50915-5548-4993-9204-edddab4e1dff", 4)]
        public void AddDoctorTest(string doctorId, string vaccinationCenterId, int index)
        {
            // Arrange
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(GetPatientsData());
            var vaccinationCenterMockSet = GetMock(GetVaccinationCentersData());
            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());
            doctorMockSet.Setup(c => c.Add(It.IsAny<Doctor>())).Callback(delegate (Doctor doc) {
                doctorData.Add(doc);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var request = new AddDoctorRequestDTO { doctorId = doctorId, vaccinationCenterId = vaccinationCenterId };

            // Act

            var result = controller.AddDoctor(request);

            // Assert

            Assert.IsType<OkResult>(result);

            var patient = patientData[index];
            var doctor = doctorData[doctorData.ToList().Count() - 1];
            Assert.NotNull(patient);
            Assert.NotNull(doctor);

            Assert.Equal(GetDoctorsData().Count() + 1, doctorData.Count());
            Assert.Equal(patient.Id, doctor.PatientId);
            Assert.Equal(Guid.Parse(doctorId), doctor.PatientId);
            Assert.Equal(patient.PESEL, doctor.PatientAccount.PESEL);
            Assert.Equal(patient.FirstName, doctor.PatientAccount.FirstName);
            Assert.Equal(patient.LastName, doctor.PatientAccount.LastName);
            Assert.Equal(patient.DateOfBirth, doctor.PatientAccount.DateOfBirth);
            Assert.Equal(patient.Mail, doctor.PatientAccount.Mail);
            Assert.Equal(patient.Password, doctor.PatientAccount.Password);
            Assert.Equal(patient.PhoneNumber, doctor.PatientAccount.PhoneNumber);
            //Assert.Equal(patientData[index].Vaccinations.Count(), doctorData[doctorIndex].PatientAccount.Vaccinations.Count());
            //Assert.Equal(patient.Certificates.Count(), doctor.PatientAccount.Certificates.Count());
            Assert.True(doctor.PatientAccount.Active);
            Assert.True(doctor.Active);
            Assert.Equal(Guid.Parse(vaccinationCenterId), doctor.VaccinationCenterId);
            //Assert.Empty(doctorData[doctorIndex].Vaccinations);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "e1d50915-5548-4993-9204-edddab4e1dff")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9f", "e0d50915-5548-4993-9204-edddab4e1dff")]
        [InlineData("00000000-0000-0000-0000-000000000001", "e0d50915-5548-4993-9204-edddab4e1dff")]
        [InlineData("wrong_doctor_guid", "e0d50915-5548-4993-9204-edddab4e1dff")]
        [InlineData("", "e0d50915-5548-4993-9204-edddab4e1dff")]
        [InlineData(null, "e0d50915-5548-4993-9204-edddab4e1dff")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b", "00000000-0000-0000-0000-000000000000")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b", "e2d50915-5548-4993-9204-edddab4e1dff")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b", "wrong_vaccination_center_guid")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b", "")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b", null)]
        public void AddWrongDoctorTest(string doctorId, string vaccinationCenterId)
        {
            // Arrange
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(GetPatientsData());
            var vaccinationCenterMockSet = GetMock(GetVaccinationCentersData());
            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(GetDoctorsData());
            doctorMockSet.Setup(c => c.Add(It.IsAny<Doctor>())).Callback(delegate (Doctor doc) {
                doctorData.Add(doc);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var request = new AddDoctorRequestDTO { doctorId = doctorId, vaccinationCenterId = vaccinationCenterId };

            // Act

            var result = controller.AddDoctor(request);

            // Assert

            Assert.IsType<NotFoundResult>(result);

            Assert.Equal(GetDoctorsData().Count(), doctorData.Count());

        }

        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", 0)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", 2)]
        public void DeleteDoctorTest(string doctorId, int index)
        {
            // Arrange

            var timeSlotsData = GetTimeSlotsData();
            var appointmentsData = GetAppointmentsData();
            var doctorsData = GetDoctorsData().ToList();

            var patientMockSet = GetMock(GetPatientsData());
            var appointmentMockSet = GetMock(appointmentsData);
            var doctorMockSet = GetMock(doctorsData.AsQueryable());
            var timeSlotMockSet = GetMock(timeSlotsData);

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeleteDoctor(doctorId);

            // Assert

            Assert.IsType<OkResult>(result);

            Assert.Equal(GetDoctorsData().Count(), doctorsData.Count());
            Assert.False(doctorsData[index].Active);
            Assert.Empty(timeSlotsData.Where(ts => ts.DoctorId == Guid.Parse(doctorId) && ts.Active == true));
            Assert.Empty(appointmentsData.Where(a => a.TimeSlot.DoctorId == Guid.Parse(doctorId) && a.State == AppointmentState.Planned));
        }

        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df2")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df4")]
        [InlineData("e0d50915-5548-4993-aaaa-edddab4e1df4")]
        [InlineData("wrong_doctor_guid")]
        [InlineData("")]
        [InlineData(null)]
        public void DeleteWrongDoctorTest(string doctorId)
        {
            // Arrange

            var patientMockSet = GetMock(GetPatientsData());
            var appintmentsData = GetAppointmentsData();
            var appointmentMockSet = GetMock(appintmentsData);
            var doctorsData = GetDoctorsData();
            var doctorMockSet = GetMock(doctorsData);
            var timeSlotsData = GetTimeSlotsData();
            var timeSlotMockSet = GetMock(timeSlotsData);

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeleteDoctor(doctorId);

            // Assert

            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(GetDoctorsData().Count(), doctorsData.ToList().Count);
            if (Guid.TryParse(doctorId, out Guid doctorGuid))
            {
                foreach (var appointment in appintmentsData.Where(a => a.TimeSlot.DoctorId == doctorGuid).ToList())
                {
                    var originalAppointment = GetAppointmentsData().SingleOrDefault(a => a.Id == appointment.Id);
                    Assert.NotNull(originalAppointment);
                    Assert.Equal(originalAppointment.State, appointment.State);
                    Assert.Equal(originalAppointment.TimeSlot.Active, appointment.TimeSlot.Active);
                }
                foreach (var timeSlot in timeSlotsData.Where(ts => ts.DoctorId == doctorGuid).ToList())
                {
                    var originalTimeSlot = GetTimeSlotsData().SingleOrDefault(ts => ts.Id == timeSlot.Id);
                    Assert.NotNull(originalTimeSlot);
                    Assert.Equal(originalTimeSlot.Active, timeSlot.Active);
                }
            }

        }

        [Fact]
        public void GetVaccinationCentersEmptyTest()
        {
            // Arrange

            var vaccinationCenterMockSet = GetMock(new List<VaccinationCenter>().AsQueryable());
            var vaccineMockSet = GetMock(new List<Vaccine>().AsQueryable());
            var openingHoursMockSet = GetMock(new List<OpeningHours>().AsQueryable());
            var vaccinesInVaccinationCenterMockSet = GetMock(new List<VaccinesInVaccinationCenter>().AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCenterMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.GetVaccinationCenters();

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<VaccinationCenterDTO>>(list.Value);

            var centers = list.Value as List<VaccinationCenterDTO>;
            Assert.Empty(centers);
        }

        [Fact]
        public void GetVaccinationCentersTest()
        {
            // Arrange

            var vaccinationCentersData = GetVaccinationCentersData().ToList();
            var vaccinesData = GetVaccinesData().ToList();
            var openingHoursData = GetOpeningHoursData().ToList();
            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCentersData.AsQueryable());
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            var openingHoursMockSet = GetMock(openingHoursData.AsQueryable());
            var vaccinesInVaccinationCenterMockSet = GetMock(vaccinesInVaccinationCentersData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCenterMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.GetVaccinationCenters();

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<VaccinationCenterDTO>>(list.Value);

            var centers = list.Value as List<VaccinationCenterDTO>;
            Assert.Equal(GetVaccinationCentersData().Count(), centers.Count);
            for (int i = 0; i < vaccinationCentersData.Count(); i++)
            {
                Assert.Equal(vaccinationCentersData.ToList()[i].Id.ToString(), centers[i].id);
                Assert.Equal(vaccinationCentersData.ToList()[i].Name, centers[i].name);
                Assert.Equal(vaccinationCentersData.ToList()[i].City, centers[i].city);
                Assert.Equal(vaccinationCentersData.ToList()[i].Address, centers[i].street);
                Assert.Equal(vaccinationCentersData.ToList()[i].Active, centers[i].active);
                Assert.Equal(vaccinesInVaccinationCentersData.Where(v => v.VaccinationCenterId == Guid.Parse(centers[i].id)).Count(), centers[i].vaccines.Count());
                foreach (var vaccineDTO in centers[i].vaccines)
                {
                    var vaccine = vaccinesInVaccinationCentersData.SingleOrDefault(v => v.VaccineId == Guid.Parse(vaccineDTO.id) && v.VaccinationCenterId == vaccinationCentersData.ToList()[i].Id);
                    Assert.NotNull(vaccine);
                    Assert.Equal(vaccine.VaccineId.ToString(), vaccineDTO.id);
                    Assert.Equal(vaccine.Vaccine.Name, vaccineDTO.name);
                    Assert.Equal(vaccine.Vaccine.Company, vaccineDTO.companyName);
                    Assert.Equal(vaccine.Vaccine.Virus.ToString(), vaccineDTO.virus);
                }
                Assert.Equal(openingHoursData.Where(oh => oh.VaccinationCenterId == Guid.Parse(centers[i].id)).Count(), centers[i].openingHoursDays.Count());
                Assert.Equal(7, centers[i].openingHoursDays.Count());
                for (int day = 0; day < 7; day++)
                {
                    Assert.Equal(openingHoursData.Single(oh => oh.VaccinationCenterId == Guid.Parse(centers[i].id) && (int)oh.WeekDay == day).From, TimeSpan.ParseExact(centers[i].openingHoursDays[day].From, "hh\\:mm", null));
                    Assert.Equal(openingHoursData.Single(oh => oh.VaccinationCenterId == Guid.Parse(centers[i].id) && (int)oh.WeekDay == day).To, TimeSpan.ParseExact(centers[i].openingHoursDays[day].To, "hh\\:mm", null));
                }
            }
        }

        [Theory]
        [InlineData("Centrum4", "Warszawa", "Al. Jerozolimskie 251", true)]
        [InlineData("Centrum5", "Poznañ", "ul. B. Prusa 10", false)]
        public void AddVaccinationCenterTest(string name, string city, string street, bool active)
        {
            // Arrange
            var vaccinationCentersData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCentersData.AsQueryable());
            vaccinationCenterMockSet.Setup(c => c.Add(It.IsAny<VaccinationCenter>())).Callback(delegate (VaccinationCenter vc) {
                vaccinationCentersData.Add(vc);
            });

            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData().ToList();
            var vaccinesInVaccinationCenterMockSet = GetMock(vaccinesInVaccinationCentersData.AsQueryable());
            vaccinesInVaccinationCenterMockSet.Setup(c => c.Add(It.IsAny<VaccinesInVaccinationCenter>())).Callback(delegate (VaccinesInVaccinationCenter vaccine) {
                vaccinesInVaccinationCentersData.Add(vaccine);
            });

            var vaccinesData = GetVaccinesData();
            var vaccineMockSet = GetMock(vaccinesData);

            var openingHoursData = GetOpeningHoursData().ToList();
            var openingHoursMockSet = GetMock(openingHoursData.AsQueryable());
            openingHoursMockSet.Setup(c => c.Add(It.IsAny<OpeningHours>())).Callback(delegate (OpeningHours oh) {
                openingHoursData.Add(oh);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var request = new AddVaccinationCenterRequestDTO {
                name = name,
                city = city,
                street = street,
                vaccineIds = new List<string> { vaccinesData.ToList()[0].Id.ToString(), vaccinesData.ToList()[2].Id.ToString() },
                openingHoursDays = new List<OpeningHoursDayDTO>
                {
                    new OpeningHoursDayDTO { From = "07:00", To = "18:00" },
                    new OpeningHoursDayDTO { From = "07:00", To = "18:00" },
                    new OpeningHoursDayDTO { From = "06:00", To = "15:00" },
                    new OpeningHoursDayDTO { From = "06:30", To = "13:45" },
                    new OpeningHoursDayDTO { From = "00:00", To = "00:00" },
                    new OpeningHoursDayDTO { From = "00:00", To = "00:00" },
                    new OpeningHoursDayDTO { From = "00:00", To = "00:00" }
                },
                active = active
            };

            // Act

            var result = controller.AddVaccinationCenter(request);

            // Assert

            Assert.IsType<OkResult>(result);

            int indexVc = vaccinationCentersData.Count() - 1;

            Assert.Equal(GetVaccinationCentersData().Count() + 1, vaccinationCentersData.Count());
            var vaccinationCenter = vaccinationCentersData[indexVc];
            Assert.Equal(name, vaccinationCenter.Name);
            Assert.Equal(city, vaccinationCenter.City);
            Assert.Equal(street, vaccinationCenter.Address);
            Assert.Equal(active, vaccinationCenter.Active);

            foreach (var vaccineId in request.vaccineIds)
            {
                Assert.Single(vaccinesInVaccinationCentersData.Where(v => v.VaccineId == Guid.Parse(vaccineId) && v.VaccinationCenterId == vaccinationCenter.Id));
            }
            Assert.Equal(request.openingHoursDays.Count(), openingHoursData.Where(oh => oh.VaccinationCenterId == vaccinationCenter.Id).Count());
            Assert.Equal(7, openingHoursData.Where(oh => oh.VaccinationCenterId == vaccinationCenter.Id).Count());
            for (int day = 0; day < 7; day++)
            {
                Assert.Equal(openingHoursData.Single(oh => oh.VaccinationCenterId == vaccinationCenter.Id && oh.WeekDay == (WeekDay)day).From, TimeSpan.ParseExact(request.openingHoursDays[day].From, "hh\\:mm", null));
                Assert.Equal(openingHoursData.Single(oh => oh.VaccinationCenterId == vaccinationCenter.Id && oh.WeekDay == (WeekDay)day).To, TimeSpan.ParseExact(request.openingHoursDays[day].To, "hh\\:mm", null));
            }
        }

        [Theory]
        [InlineData("Centrum4", "Warszawa", "Al. Jerozolimskie 251", true)]
        [InlineData("Centrum5", "Poznañ", "ul. B. Prusa 10", false)]
        public void AddWrongTimeSpanFormatVaccinationCenterTest(string name, string city, string street, bool active)
        {
            // Arrange
            var vaccinationCentersData = GetVaccinationCentersData();
            var vaccinationCenterMockSet = GetMock(vaccinationCentersData);
            vaccinationCenterMockSet.Setup(c => c.Add(It.IsAny<VaccinationCenter>())).Callback(delegate (VaccinationCenter vc) {
                vaccinationCentersData.ToList().Add(vc);
            });

            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData();
            var vaccinesInVaccinationCenterMockSet = GetMock(vaccinesInVaccinationCentersData);
            vaccinesInVaccinationCenterMockSet.Setup(c => c.Add(It.IsAny<VaccinesInVaccinationCenter>())).Callback(delegate (VaccinesInVaccinationCenter vaccine) {
                vaccinesInVaccinationCentersData.ToList().Add(vaccine);
            });

            var vaccinesData = GetVaccinesData();
            var vaccineMockSet = GetMock(vaccinesData);

            var openingHoursData = GetOpeningHoursData();
            var openingHoursMockSet = GetMock(openingHoursData);
            openingHoursMockSet.Setup(c => c.Add(It.IsAny<OpeningHours>())).Callback(delegate (OpeningHours oh) {
                openingHoursData.ToList().Add(oh);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var request = new AddVaccinationCenterRequestDTO
            {
                name = name,
                city = city,
                street = street,
                vaccineIds = new List<string> { vaccinesData.ToList()[0].Id.ToString(), vaccinesData.ToList()[2].Id.ToString() },
                openingHoursDays = new List<OpeningHoursDayDTO>
                {
                    new OpeningHoursDayDTO { From = "7:00", To = "18:00" },
                    new OpeningHoursDayDTO { From = "7:00", To = "18:00" },
                    new OpeningHoursDayDTO { From = "6:00", To = "15:00" },
                    new OpeningHoursDayDTO { From = "6:30", To = "13:45" },
                    new OpeningHoursDayDTO { From = "test", To = "00:00" },
                    new OpeningHoursDayDTO { From = "00:00", To = "1000" },
                    new OpeningHoursDayDTO { From = "00:00", To = "00:00" }
                },
                active = active
            };

            // Act

            var result = controller.AddVaccinationCenter(request);

            // Assert

            Assert.IsType<NotFoundResult>(result);

            Assert.Equal(GetVaccinationCentersData().Count(), vaccinationCentersData.Count());
            Assert.Equal(GetVaccinesInVaccinationCentersData().Count(), vaccinesInVaccinationCentersData.Count());
            Assert.Equal(GetOpeningHoursData().Count(), openingHoursData.Count());

        }

        [Theory]
        [InlineData("Centrum4", "Warszawa", "Al. Jerozolimskie 251", true, "e0d50915-5548-4993-dddd-edddab4e1df1")]
        [InlineData("Centrum5", "Poznañ", "ul. B. Prusa 10", false, "wrong_vaccine_guid")]
        [InlineData("Centrum5", "Poznañ", "ul. B. Prusa 10", false, "")]
        [InlineData("Centrum5", "Poznañ", "ul. B. Prusa 10", false, null)]
        public void AddWrongVaccineIdVaccinationCenterTest(string name, string city, string street, bool active, string id)
        {
            // Arrange
            var vaccinationCentersData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCentersData.AsQueryable());
            vaccinationCenterMockSet.Setup(c => c.Add(It.IsAny<VaccinationCenter>())).Callback(delegate (VaccinationCenter vc) {
                vaccinationCentersData.Add(vc);
            });

            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData().ToList();
            var vaccinesInVaccinationCenterMockSet = GetMock(vaccinesInVaccinationCentersData.AsQueryable());
            vaccinesInVaccinationCenterMockSet.Setup(c => c.Add(It.IsAny<VaccinesInVaccinationCenter>())).Callback(delegate (VaccinesInVaccinationCenter vaccine) {
                vaccinesInVaccinationCentersData.Add(vaccine);
            });

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var openingHoursData = GetOpeningHoursData().ToList();
            var openingHoursMockSet = GetMock(openingHoursData.AsQueryable());
            openingHoursMockSet.Setup(c => c.Add(It.IsAny<OpeningHours>())).Callback(delegate (OpeningHours oh) {
                openingHoursData.Add(oh);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var request = new AddVaccinationCenterRequestDTO
            {
                name = name,
                city = city,
                street = street,
                vaccineIds = new List<string> { id },
                openingHoursDays = new List<OpeningHoursDayDTO>
                {
                    new OpeningHoursDayDTO { From = "07:00", To = "18:00" },
                    new OpeningHoursDayDTO { From = "07:00", To = "18:00" },
                    new OpeningHoursDayDTO { From = "06:00", To = "15:00" },
                    new OpeningHoursDayDTO { From = "06:30", To = "13:45" },
                    new OpeningHoursDayDTO { From = "00:00", To = "00:00" },
                    new OpeningHoursDayDTO { From = "00:00", To = "00:00" },
                    new OpeningHoursDayDTO { From = "00:00", To = "00:00" }
                },
                active = active
            };

            // Act

            var result = controller.AddVaccinationCenter(request);

            // Assert

            Assert.IsType<NotFoundResult>(result);

            Assert.Equal(GetVaccinationCentersData().Count(), vaccinationCentersData.Count());
            Assert.Equal(GetVaccinesInVaccinationCentersData().Count(), vaccinesInVaccinationCentersData.Count());
            Assert.Equal(GetOpeningHoursData().Count(), openingHoursData.Count());
        }

        [Theory]
        [InlineData("Centrum4", "Warszawa", "Al. Jerozolimskie 251", true, 6)]
        [InlineData("Centrum5", "Poznañ", "ul. B. Prusa 10", false, 8)]
        [InlineData("Centrum5", "Poznañ", "ul. B. Prusa 10", false, 0)]
        public void AddWrongOpeningHoursCountVaccinationCenterTest(string name, string city, string street, bool active, int count)
        {
            // Arrange
            var vaccinationCentersData = GetVaccinationCentersData();
            var vaccinationCenterMockSet = GetMock(vaccinationCentersData);
            vaccinationCenterMockSet.Setup(c => c.Add(It.IsAny<VaccinationCenter>())).Callback(delegate (VaccinationCenter vc) {
                vaccinationCentersData.ToList().Add(vc);
            });

            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData();
            var vaccinesInVaccinationCenterMockSet = GetMock(vaccinesInVaccinationCentersData);
            vaccinesInVaccinationCenterMockSet.Setup(c => c.Add(It.IsAny<VaccinesInVaccinationCenter>())).Callback(delegate (VaccinesInVaccinationCenter vaccine) {
                vaccinesInVaccinationCentersData.ToList().Add(vaccine);
            });

            var vaccinesData = GetVaccinesData();
            var vaccineMockSet = GetMock(vaccinesData);

            var openingHoursData = GetOpeningHoursData();
            var openingHoursMockSet = GetMock(openingHoursData);
            openingHoursMockSet.Setup(c => c.Add(It.IsAny<OpeningHours>())).Callback(delegate (OpeningHours oh) {
                openingHoursData.ToList().Add(oh);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var openingHoursDays = new List<OpeningHoursDayDTO>();
            for (int i = 0; i < count; i++)
                openingHoursDays.Add(new OpeningHoursDayDTO() { From = "07:00", To = "18:00" });


            var request = new AddVaccinationCenterRequestDTO
            {
                name = name,
                city = city,
                street = street,
                vaccineIds = new List<string> { vaccinesData.ToList()[0].Id.ToString(), vaccinesData.ToList()[2].Id.ToString() },
                openingHoursDays = openingHoursDays,
                active = active
            };

            // Act

            var result = controller.AddVaccinationCenter(request);

            // Assert

            Assert.IsType<NotFoundResult>(result);

            Assert.Equal(GetVaccinationCentersData().Count(), vaccinationCentersData.Count());
            Assert.Equal(GetVaccinesInVaccinationCentersData().Count(), vaccinesInVaccinationCentersData.Count());
            Assert.Equal(GetOpeningHoursData().Count(), openingHoursData.Count());
        }

        [Theory]
        [InlineData("e0d50915-5548-4993-9204-edddab4e1dff", 0)]
        [InlineData("e1d50915-5548-4993-9204-edddab4e1dff", 1)]
        public void DeleteVaccinationCenterTest(string id, int index)
        {
            // Arrange
            var vaccinationCentersData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCentersData.AsQueryable());

            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData().ToList();
            var vaccinesInVaccinationCenterMockSet = GetMock(vaccinesInVaccinationCentersData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var openingHoursData = GetOpeningHoursData().ToList();
            var openingHoursMockSet = GetMock(openingHoursData.AsQueryable());

            var doctorsData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorsData.AsQueryable());

            var timeSlotsData = GetTimeSlotsData().ToList();
            var timeSlotsMockSet = GetMock(timeSlotsData.AsQueryable());

            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentsMockSet = GetMock(appointmentsData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotsMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentsMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeleteVaccinationCenter(id);

            // Assert

            Assert.IsType<OkResult>(result);

            Assert.Equal(GetVaccinationCentersData().Count(), vaccinationCentersData.Count());
            Assert.Equal(GetVaccinesInVaccinationCentersData().Count(), vaccinesInVaccinationCentersData.Count());
            Assert.Equal(GetOpeningHoursData().Count(), openingHoursData.Count());
            Assert.False(vaccinationCentersData[index].Active);

            Assert.Equal(7, openingHoursData.Where(oh => oh.VaccinationCenterId == vaccinationCentersData.ToList()[index].Id).Count());

            Assert.Empty(timeSlotsData.Where(ts => ts.Doctor.VaccinationCenterId == vaccinationCentersData[index].Id && ts.Active == true));
            Assert.Empty(appointmentsData.Where(a => a.TimeSlot.Doctor.VaccinationCenterId == vaccinationCentersData[index].Id && a.State == AppointmentState.Planned));
            Assert.Empty(doctorsData.Where(doc => doc.VaccinationCenterId == vaccinationCentersData[index].Id && doc.Active == true));
        }

        [Theory]
        [InlineData("e2d50915-5548-4993-9204-edddab4e1dff")]
        [InlineData("e2d50915-5548-aaaa-9204-edddab4e1dff")]
        [InlineData("wrong_guid_format")]
        [InlineData("")]
        [InlineData(null)]
        public void DeleteWrongVaccinationCenterTest(string id)
        {
            // Arrange
            var vaccinationCentersData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCentersData.AsQueryable());

            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData().ToList();
            var vaccinesInVaccinationCenterMockSet = GetMock(vaccinesInVaccinationCentersData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var openingHoursData = GetOpeningHoursData().ToList();
            var openingHoursMockSet = GetMock(openingHoursData.AsQueryable());

            var doctorsData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorsData.AsQueryable());

            var timeSlotsData = GetTimeSlotsData().ToList();
            var timeSlotsMockSet = GetMock(timeSlotsData.AsQueryable());

            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentsMockSet = GetMock(appointmentsData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotsMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentsMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeleteVaccinationCenter(id);

            // Assert

            Assert.IsType<NotFoundResult>(result);

            Assert.Equal(GetVaccinationCentersData().Count(), vaccinationCentersData.Count());
            Assert.Equal(GetVaccinesInVaccinationCentersData().Count(), vaccinesInVaccinationCentersData.Count());
            Assert.Equal(GetOpeningHoursData().Count(), openingHoursData.Count());

            if (Guid.TryParse(id, out Guid guid))
            {
                foreach (var doctor in doctorsData.Where(doc => doc.VaccinationCenterId == guid && doc.Active == true))
                {
                    foreach (var appointment in appointmentsData.Where(a => a.TimeSlot.DoctorId == doctor.Id).ToList())
                    {
                        var originalAppointment = GetAppointmentsData().SingleOrDefault(a => a.Id == appointment.Id);
                        Assert.NotNull(originalAppointment);
                        Assert.Equal(originalAppointment.State, appointment.State);
                        Assert.Equal(originalAppointment.TimeSlot.Active, appointment.TimeSlot.Active);
                    }
                    foreach (var timeSlot in timeSlotsData.Where(ts => ts.DoctorId == doctor.Id).ToList())
                    {
                        var originalTimeSlot = GetTimeSlotsData().SingleOrDefault(ts => ts.Id == timeSlot.Id);
                        Assert.NotNull(originalTimeSlot);
                        Assert.Equal(originalTimeSlot.Active, timeSlot.Active);
                    }
                }
            }

        }

        [Fact]
        public void GetVaccinesEmptyTest()
        {
            // Arrange

            var vaccinesData = new List<Vaccine>();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.GetVaccines();

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<VaccineDTO>>(list.Value);

            var vaccines = list.Value as List<VaccineDTO>;
            Assert.Empty(vaccines);
        }

        [Fact]
        public void GetVaccinesTest()
        {
            // Arrange

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.GetVaccines();

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<VaccineDTO>>(list.Value);

            var vaccines = list.Value as List<VaccineDTO>;

            Assert.Equal(vaccinesData.Count(), vaccines.Count());

            foreach (var vaccineDTO in vaccines)
            {
                var originalVaccine = vaccinesData.SingleOrDefault(v => v.Id == Guid.Parse(vaccineDTO.vaccineId));
                Assert.NotNull(originalVaccine);
                Assert.Equal(originalVaccine.Company, vaccineDTO.company);
                Assert.Equal(originalVaccine.Name, vaccineDTO.name);
                Assert.Equal(originalVaccine.NumberOfDoses, vaccineDTO.numberOfDoses);
                Assert.Equal(originalVaccine.MinDaysBetweenDoses, vaccineDTO.minDaysBetweenDoses);
                Assert.Equal(originalVaccine.MaxDaysBetweenDoses, vaccineDTO.maxDaysBetweenDoses);
                Assert.Equal(originalVaccine.Virus.ToString(), vaccineDTO.virus);
                Assert.Equal(originalVaccine.MinPatientAge, vaccineDTO.minPatientAge);
                Assert.Equal(originalVaccine.MaxPatientAge, vaccineDTO.maxPatientAge);
                Assert.Equal(originalVaccine.Active, vaccineDTO.active);
            }
        }

        [Theory]
        [InlineData(1, 1, 10, "Koronawirus", 1, 10)]
        [InlineData(1, 0, 10, "Koronawirus", 1, 10)]
        [InlineData(1, 1, 10, "Koronawirus", 0, 10)]
        [InlineData(10, 1, 10, "Koronawirus", 1, 10)]
        [InlineData(1, 10, 10, "Koronawirus", 1, 10)]
        [InlineData(1, 0, 0, "Koronawirus", 1, 10)]
        [InlineData(1, 1, 10, "Koronawirus", 10, 10)]
        [InlineData(1, 1, 10, "Koronawirus", 0, 0)]
        [InlineData(1, -1, 10, "Koronawirus", 1, 10)]
        [InlineData(1, -1, -10, "Koronawirus", 1, 10)]
        [InlineData(1, 1, -10, "Koronawirus", 1, 10)]
        [InlineData(1, 1, 10, "Koronawirus", -1, 10)]
        [InlineData(1, 1, 10, "Koronawirus", -1, -10)]
        [InlineData(1, 1, 10, "Koronawirus", 1, -10)]
        public void AddVaccineTest(int numberOfDoses, int minDaysBetweenDoses, int maxDaysBetweenDoses, string virus, int minPatientAge, int maxPatientAge)
        {
            // Arrange

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            vaccineMockSet.Setup(c => c.Add(It.IsAny<Vaccine>())).Callback(delegate (Vaccine v) {
                vaccinesData.Add(v);
            });
            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var vaccineDTO = new AddVaccineRequestDTO()
            {
                company = "CompanyX",
                name = "Xvaccine",
                numberOfDoses = numberOfDoses,
                minDaysBetweenDoses = minDaysBetweenDoses,
                maxDaysBetweenDoses = maxDaysBetweenDoses,
                minPatientAge = minPatientAge,
                maxPatientAge = maxPatientAge,
                virus = virus,
                active = true
            };

            // Act

            var result = controller.AddVaccine(vaccineDTO);

            // Assert

            Assert.IsType<OkResult>(result);

            Assert.Equal(GetVaccinesData().ToList().Count() + 1, vaccinesData.Count());

            var vaccine = vaccinesData[vaccinesData.Count() - 1];

            Assert.Equal(vaccineDTO.company, vaccine.Company);
            Assert.Equal(vaccineDTO.name, vaccine.Name);
            Assert.Equal(vaccineDTO.numberOfDoses, vaccine.NumberOfDoses);
            Assert.Equal(vaccineDTO.minDaysBetweenDoses, vaccine.MinDaysBetweenDoses);
            Assert.Equal(vaccineDTO.maxDaysBetweenDoses, vaccine.MaxDaysBetweenDoses);
            Assert.Equal(vaccineDTO.minPatientAge, vaccine.MinPatientAge);
            Assert.Equal(vaccineDTO.maxPatientAge, vaccine.MaxPatientAge);
            Assert.Equal(vaccineDTO.virus, vaccine.Virus.ToString());
            Assert.Equal(vaccineDTO.active, vaccine.Active);
        }

        [Theory]
        [InlineData(1, 1, 10, "Œwirus", 1, 10)]
        [InlineData(1, 1, 10, "", 1, 10)]
        [InlineData(1, 1, 10, null, 1, 10)]
        [InlineData(0, 1, 10, "Koronawirus", 1, 10)]
        [InlineData(-1, 1, 10, "Koronawirus", 1, 10)]
        [InlineData(1, 1, 10, "Koronawirus", 10, 1)]
        [InlineData(1, 10, 1, "Koronawirus", 1, 10)]
        public void AddWrongVaccineTest(int numberOfDoses, int minDaysBetweenDoses, int maxDaysBetweenDoses, string virus, int minPatientAge, int maxPatientAge)
        {
            // Arrange

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            vaccineMockSet.Setup(c => c.Add(It.IsAny<Vaccine>())).Callback(delegate (Vaccine v) {
                vaccinesData.Add(v);
            });
            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            var vaccineDTO = new AddVaccineRequestDTO()
            {
                company = "CompanyX",
                name = "Xvaccine",
                numberOfDoses = numberOfDoses,
                minDaysBetweenDoses = minDaysBetweenDoses,
                maxDaysBetweenDoses = maxDaysBetweenDoses,
                minPatientAge = minPatientAge,
                maxPatientAge = maxPatientAge,
                virus = virus,
                active = true
            };

            // Act

            var result = controller.AddVaccine(vaccineDTO);

            // Assert

            Assert.IsType<NotFoundResult>(result);

            Assert.Equal(GetVaccinesData().ToList().Count(), vaccinesData.Count());

            foreach (var vaccine in vaccinesData)
            {
                var originalVaccine = GetVaccinesData().SingleOrDefault(v => v.Id == vaccine.Id);
                Assert.NotNull(originalVaccine);

                Assert.Equal(originalVaccine.Company, vaccine.Company);
                Assert.Equal(originalVaccine.Name, vaccine.Name);
                Assert.Equal(originalVaccine.NumberOfDoses, vaccine.NumberOfDoses);
                Assert.Equal(originalVaccine.MinDaysBetweenDoses, vaccine.MinDaysBetweenDoses);
                Assert.Equal(originalVaccine.MaxDaysBetweenDoses, vaccine.MaxDaysBetweenDoses);
                Assert.Equal(originalVaccine.MinPatientAge, vaccine.MinPatientAge);
                Assert.Equal(originalVaccine.MaxPatientAge, vaccine.MaxPatientAge);
                Assert.Equal(originalVaccine.Virus, vaccine.Virus);
                Assert.Equal(originalVaccine.Active, vaccine.Active);
            }

        }

        [Theory]
        [InlineData("e0d50915-5548-4993-aaaa-edddab4e1df1", 0)]
        [InlineData("e0d50915-5548-4993-aaaa-edddab4e1df2", 1)]
        [InlineData("e0d50915-5548-4993-aaaa-edddab4e1df3", 2)]
        public void DeleteVaccineTest(string vaccineId, int index)
        {
            // Arrange

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            vaccineMockSet.Setup(c => c.Add(It.IsAny<Vaccine>())).Callback(delegate (Vaccine v) {
                vaccinesData.Add(v);
            });
            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentsData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment a)
            {
                appointmentsData.Add(a);
            });
            var timeSlotsData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotsData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot timeSlot)
            {
                timeSlotsData.Add(timeSlot);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeleteVaccine(vaccineId);

            // Assert

            Assert.IsType<OkResult>(result);

            Assert.Equal(GetVaccinesData().ToList().Count(), vaccinesData.Count());
            Assert.False(vaccinesData[index].Active);

            Assert.Empty(appointmentsData.Where(a => a.VaccineId == Guid.Parse(vaccineId) && a.State == AppointmentState.Planned));

        }

        [Theory]
        [InlineData("e0d50915-5548-4993-aaaa-edddab4e1df4")]
        [InlineData("e0d50915-5548-4993-1111-edddab4e1df4")]
        [InlineData("wrong_guid_format")]
        [InlineData("")]
        [InlineData(null)]
        public void DeleteWrongVaccineTest(string vaccineId)
        {
            // Arrange

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            vaccineMockSet.Setup(c => c.Add(It.IsAny<Vaccine>())).Callback(delegate (Vaccine v) {
                vaccinesData.Add(v);
            });
            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentsData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment a)
            {
                appointmentsData.Add(a);
            });
            var timeSlotsData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotsData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot timeSlot)
            {
                timeSlotsData.Add(timeSlot);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeleteVaccine(vaccineId);

            // Assert

            Assert.IsType<NotFoundResult>(result);

            Assert.Equal(GetVaccinesData().ToList().Count(), vaccinesData.Count());

            if (Guid.TryParse(vaccineId, out Guid guid))
            {
                foreach (var appointment in appointmentsData.Where(a => a.VaccineId == guid).ToList())
                {
                    var originalAppointment = GetAppointmentsData().SingleOrDefault(a => a.Id == appointment.Id);
                    Assert.NotNull(originalAppointment);
                    Assert.Equal(originalAppointment.State, appointment.State);
                    Assert.Equal(originalAppointment.TimeSlot.Active, appointment.TimeSlot.Active);
                }
            }

        }

        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df2")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df4")]
        public void GetTimeSlotsTest(string doctorId)
        {
            // Arrange

            var doctorsData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorsData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            vaccineMockSet.Setup(c => c.Add(It.IsAny<Vaccine>())).Callback(delegate (Vaccine v) {
                vaccinesData.Add(v);
            });
            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentsData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment a)
            {
                appointmentsData.Add(a);
            });
            var timeSlotsData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotsData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot timeSlot)
            {
                timeSlotsData.Add(timeSlot);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            string format = "dd-MM-yyyy HH\\:mm";

            // Act

            var result = controller.GetTimeSlots(doctorId);

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<TimeSlotDTO>>(list.Value);

            var timeSlots = list.Value as List<TimeSlotDTO>;
            Assert.Equal(timeSlotsData.Where(ts => ts.DoctorId == Guid.Parse(doctorId)).ToList().Count(), timeSlots.Count());

            foreach (var timeSlot in timeSlots)
            {
                var originalTimeSlot = timeSlotsData.SingleOrDefault(ts => ts.DoctorId == Guid.Parse(doctorId) && ts.Id == Guid.Parse(timeSlot.id));
                Assert.NotNull(originalTimeSlot);
                Assert.Equal(originalTimeSlot.From, DateTime.ParseExact(timeSlot.from, format, null));
                Assert.Equal(originalTimeSlot.To, DateTime.ParseExact(timeSlot.to, format, null));
                Assert.Equal(originalTimeSlot.IsFree, timeSlot.isFree);
                Assert.Equal(originalTimeSlot.Active, timeSlot.active);
            }
        }

        [Theory]
        [InlineData("e0d50915-5548-4993-0000-edddab4e1df1")]
        [InlineData("wrong_guid_format")]
        [InlineData("")]
        [InlineData(null)]
        public void GetWrongDoctorTimeSlotsTest(string doctorId)
        {
            // Arrange

            var doctorsData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorsData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            vaccineMockSet.Setup(c => c.Add(It.IsAny<Vaccine>())).Callback(delegate (Vaccine v) {
                vaccinesData.Add(v);
            });
            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentsData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment a)
            {
                appointmentsData.Add(a);
            });
            var timeSlotsData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotsData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot timeSlot)
            {
                timeSlotsData.Add(timeSlot);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            //string format = "dd-MM-yyyy HH\\:mm";

            // Act

            var result = controller.GetTimeSlots(doctorId);

            // Assert

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Theory]
        [MemberData(nameof(TimeSlotIdList1))]
        public void DeleteTimeSlots(List<string> ids)
        {
            // Arrange

            var doctorsData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorsData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            vaccineMockSet.Setup(c => c.Add(It.IsAny<Vaccine>())).Callback(delegate (Vaccine v) {
                vaccinesData.Add(v);
            });
            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentsData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment a)
            {
                appointmentsData.Add(a);
            });
            var timeSlotsData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotsData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot timeSlot)
            {
                timeSlotsData.Add(timeSlot);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeleteTimeSlots(ids);

            // Assert

            Assert.IsType<OkResult>(result);

            Assert.Equal(GetTimeSlotsData().ToList().Count(), timeSlotsData.Count());

            bool anyDeleted = false;

            foreach (var id in ids)
            {
                if (Guid.TryParse(id, out Guid guid))
                {
                    var timeSlot = timeSlotsData.SingleOrDefault(ts => ts.Id == guid);
                    if(timeSlot != null)
                    {
                        Assert.False(timeSlot.Active);
                        if(GetTimeSlotsData().SingleOrDefault(ts => ts.Id == guid).Active != timeSlot.Active)
                            anyDeleted = true;
                    }
                    var appointment = appointmentsData.SingleOrDefault(a => a.TimeSlotId == guid);
                    if(appointment != null)
                    {
                        Assert.True(appointment.State != AppointmentState.Planned);
                    }

                }
            }
            Assert.True(anyDeleted);
        }

        public static IEnumerable<object[]> TimeSlotIdList1()
        {
            yield return new object[] { new List<string>() { "a0780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "a1780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "b0780125-a945-4e20-b2ab-02bcf0ce8f3b", "b1780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "b0780125-a945-4e20-b2ab-02bcf0ce8f3b", "b1780125-a945-4e20-b2ab-02bcf0ce8f3b", "b2780125-a945-4e20-b2ab-02bcf0ce8f3b", "b3780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "b2780125-a945-4e20-b2ab-02bcf0ce8f3b", "b3780125-a945-4e20-b2ab-02bcf0ce8f3b", "b0780125-a945-4e20-b2ab-02bcf0ce8f3b", "b1780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "a0780125-a945-4e20-aaaa-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "a0780125-a945-4e20-aaaa-02bcf0ce8f3b", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "wrong_guid_format", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "wrong_guid_format" } };
            yield return new object[] { new List<string>() { "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "" } };
            yield return new object[] { new List<string>() { "", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { null, "a0780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", null } };
        }

        [Theory]
        [MemberData(nameof(TimeSlotIdList2))]
        public void DeleteWrongTimeSlots(List<string> ids)
        {
            // Arrange

            var doctorsData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorsData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            vaccineMockSet.Setup(c => c.Add(It.IsAny<Vaccine>())).Callback(delegate (Vaccine v) {
                vaccinesData.Add(v);
            });
            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentsData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment a)
            {
                appointmentsData.Add(a);
            });
            var timeSlotsData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotsData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot timeSlot)
            {
                timeSlotsData.Add(timeSlot);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeleteTimeSlots(ids);

            // Assert

            Assert.IsType<NotFoundResult>(result);

            Assert.Equal(GetTimeSlotsData().ToList().Count(), timeSlotsData.Count());

            foreach (var id in ids)
            {
                if(Guid.TryParse(id, out Guid guid))
                {
                    var timeSlot = timeSlotsData.SingleOrDefault(ts => ts.Id == guid);
                    if(timeSlot != null)
                    {
                        Assert.Equal(GetTimeSlotsData().SingleOrDefault(ts => ts.Id == guid).Active, timeSlot.Active);
                    }

                    var appointment = appointmentsData.SingleOrDefault(a => a.TimeSlotId == guid);
                    if(appointment != null)
                    {
                        Assert.Equal(GetAppointmentsData().SingleOrDefault(a => a.Id == appointment.Id).State, appointment.State);
                    }
                }
            }
        }

        public static IEnumerable<object[]> TimeSlotIdList2()
        {
            yield return new object[] { new List<string>() { "a2780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "a2780125-a945-4e20-b2ab-02bcf0ce8f3b", "a3780125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "b2780125-a945-4e20-b2ab-02bcf0ce8f3b", "b3780125-a945-4e20-b2ab-02bcf0ce8f3b", "b4781125-a945-4e20-b2ab-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "b3780125-a945-4e20-aaaa-02bcf0ce8f3b" } };
            yield return new object[] { new List<string>() { "wrong_guid_format" } };
            yield return new object[] { new List<string>() { "" } };
            yield return new object[] { new List<string>() { null } };
        }


    }
}
