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
    public class PatientControllersTests: DataMock
    {
        [Theory]
        [InlineData("Warszawa", "01-03-2022 10:00", "01-03-2022 11:00", "Koronawirus", 1)]
        [InlineData("Kraków", "01-03-2022 10:00", "01-03-2022 11:00", "Koronawirus", 1)]
        public void FilterTimeSlotsTest(string city, string DateFrom, string dateTo, string Virus, int timeSlotCount)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());
            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());
            var vaccinationCenterData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCenterData.AsQueryable());
            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData().ToList();
            var vaccinesInVaccinationCentersMockSet = GetMock(vaccinesInVaccinationCentersData.AsQueryable());
            var openingHoursData = GetOpeningHoursData().ToList();
            var openingHoursMockSet = GetMock(openingHoursData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCentersMockSet.Object);
            mockContext.Setup(c => c.OpeningHours).Returns(openingHoursMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.FilterTimeSlots(city, DateFrom, dateTo, Virus);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<TimeSlotFilterResponseDTO>>(list.Value);

            var timeSlots = list.Value as List<TimeSlotFilterResponseDTO>;
            Assert.Equal(timeSlotCount, timeSlots.Count());
        }
        [Theory]
        [InlineData("Warszawa", "01-03-2022 12:00", "01-03-2022 13:00", "Koronawirus")]
        [InlineData("Warszawa", "01-03-2022 10:00", "01-03-2022 11:00", "KoronawirusFake")]
        [InlineData("Niewłaściwe Miasto", "01-03-2022 10:00", "01-03-2022 10:00", "Koronawirus")]
        [InlineData("Kraków", "01-03-2022 10:00", "01-03-2022 10:00", "Koronawirus")]
        public void FilterTimeSlotsEmptyTest(string city, string DateFrom, string dateTo, string Virus)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());
            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());
            var vaccinationCenterData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCenterData.AsQueryable());
            var vaccinesInVaccinationCentersData = GetVaccinesInVaccinationCentersData().ToList();
            var vaccinesInVaccinationCentersMockSet = GetMock(vaccinesInVaccinationCentersData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCentersMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.FilterTimeSlots(city, DateFrom, dateTo, Virus);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9a")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c")]
        public void GetIncomingVisitsTest(string patientId)
        {
            // Arrange
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinationCenterData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCenterData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetIncomingVisits(patientId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<FutureAppointmentDTO>>(list.Value);

            var appointments = list.Value as List<FutureAppointmentDTO>;
            Assert.Single(appointments);
        }
        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9f")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void GetIncomingVisitsEmptyTest(string patientId)
        {
            // Arrange
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinationCenterData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCenterData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetIncomingVisits(patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("baa06325-e151-4cd6-a829-254c0314faad", "81a130d2-502f-4cf1-a376-63edeb000e9a")]
        [InlineData("baa16325-e151-4cd6-a829-254c0314faad", "81a130d2-502f-4cf1-a376-63edeb000e9c")]
        public void CancelVisitTest(string appointmentId, string patientId)
        {
            // Arrange
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.CancelVisit(appointmentId, patientId);

            // Assert
            Assert.IsType<OkResult>(result);
            Appointment appointmentAfter = mockContext.Object.Appointments.Where(app => app.Id.ToString() == appointmentId).FirstOrDefault();
            Assert.True(appointmentAfter != null && appointmentAfter.State == AppointmentState.Cancelled);
            
        }
        [Theory]
        [InlineData("baa06325-e151-4cd6-a829-254c0314faad", "81a130d2-502f-4cf1-a376-63edeb000e9c")] // patient is not the owner of appointment
        [InlineData("baa26325-e151-4cd6-a829-254c0314faad", "81a130d2-502f-4cf1-a376-63edeb000e9c")]
        [InlineData("baa36325-e151-4cd6-a829-254c0314faad", "81a130d2-502f-4cf1-a376-63edeb000e9f")]
        [InlineData("00000000-0000-0000-0000-000000000000", "81a130d2-502f-4cf1-a376-63edeb000e9c")]
        public void CancelVisitInvalidTest(string appointmentId, string patientId)
        {
            // Arrange
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            Appointment appointmentBefore = mockContext.Object.Appointments.Where(app => app.Id.ToString() == appointmentId).FirstOrDefault();

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.CancelVisit(appointmentId, patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            Appointment appointmentAfter = mockContext.Object.Appointments.Where(app => app.Id.ToString() == appointmentId).FirstOrDefault();
            if (appointmentBefore != null && appointmentAfter != null) Assert.True(appointmentBefore.State == appointmentAfter.State);
        }
        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9f")]
        public void GetFormerVisitsTest(string patientId)
        {
            // Arrange
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinationCenterData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCenterData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetFormerVisits(patientId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<FormerAppointmentDTO>>(list.Value);

            var appointments = list.Value as List<FormerAppointmentDTO>;
            Assert.Single(appointments);
        }
        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9a")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void GetFormerVisitsEmptyTest(string patientId)
        {
            // Arrange
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinationCenterData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCenterData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetFormerVisits(patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9f")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b")]
        public void GetCertificatesTest(string patientId)
        {
            // Arrange
            var certificateData = GetCertificatesData().ToList();
            var certificateMockSet = GetMock(certificateData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Certificates).Returns(certificateMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetCertificates(patientId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<BasicCertificateInfoDTO>>(list.Value);

            var certificates = list.Value as List<BasicCertificateInfoDTO>;
            Assert.Single(certificates);
        }
        [Theory]
        [InlineData("12333324-502f-4cf1-a376-63edeb000e9a")]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void GetCertificatesEmptyTest(string patientId)
        {
            // Arrange
            var certificateData = GetCertificatesData().ToList();
            var certificateMockSet = GetMock(certificateData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Certificates).Returns(certificateMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetCertificates(patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
