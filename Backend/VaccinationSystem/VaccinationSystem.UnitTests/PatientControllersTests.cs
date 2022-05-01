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

namespace VaccinationSystem.UnitTests
{
    public class PatientControllersTests: DataMock
    {
        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9a")]
        public void GetPatientData(string patientId)
        {
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetPatientInfo(patientId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var info = result.Result as OkObjectResult;
            Assert.IsType<PatientInfoResponseDTO>(info.Value);

            var patientInfo = info.Value as PatientInfoResponseDTO;
            Assert.True(patientInfo.PESEL == "00000000002");
        }
        [Theory]
        [InlineData("01234567-0123-0123-0123-0123456789ab")]
        public void GetPatientEmptyData(string patientId)
        {
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetPatientInfo(patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("ZlyGuid")]
        [InlineData(null)]
        public void GetPatientBadFormatData(string patientId)
        {
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.GetPatientInfo(patientId);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Theory]
        [InlineData("Warszawa", "01-03-2022", "01-03-2022", "Koronawirus", 1)]
        [InlineData("Kraków", "01-03-2022", "01-03-2022", "Koronawirus", 1)]
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
        [InlineData("Warszawa", "01-03-2022", "01-03-2022", "Koronawirus")]
        [InlineData("Warszawa", "01-03-2022", "01-03-2022", "KoronawirusFake")]
        [InlineData("Niewłaściwe Miasto", "01-03-2022", "01-03-2022", "Koronawirus")]
        [InlineData("Kraków", "01-03-2022", "01-03-2022", "Koronawirus")]
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
            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentsMockSet = GetMock(appointmentsData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCentersMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentsMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.FilterTimeSlots(city, DateFrom, dateTo, Virus);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData(null, "01-03-2022", "01-03-2022", "Koronawirus")]
        [InlineData("Warszawa", null, "01-03-2022", "Koronawirus")]
        [InlineData("Warszawa", "01-03-2022", null, "Koronawirus")]
        [InlineData("Warszawa", "01-03-2022", "01-03-2022", null)]
        [InlineData("Warszawa", "01-03-2022 10:00", "01-03-2022", "Koronawirus")]
        [InlineData("Warszawa", "01-03-2022", "01-03-2022 11:00", "Koronawirus")]
        public void FilterTimeSlotsBadFormatTest(string city, string DateFrom, string dateTo, string Virus)
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
            var appointmentsData = GetAppointmentsData().ToList();
            var appointmentsMockSet = GetMock(appointmentsData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);
            mockContext.Setup(c => c.VaccinesInVaccinationCenter).Returns(vaccinesInVaccinationCentersMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentsMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.FilterTimeSlots(city, DateFrom, dateTo, Virus);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }
        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "e0d50915-5548-4993-aaaa-edddab4e1df1", 1)]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "b0780125-a945-4e20-b2ab-02bcf0ce8f3b", "e0d50915-5548-4993-aaaa-edddab4e1df3", 2)]
        public void BookVisitTest(string patientId, string timeSlotId, string vaccineId, int expectedDose)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap)
            {
                appointmentData.Add(ap);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.BookVisit(patientId, timeSlotId, vaccineId);

            // Assert
            Assert.IsType<OkResult>(result);
            var changedTimeSlot = mockContext.Object.TimeSlots.Where(ts => ts.Id.ToString() == timeSlotId && ts.Active == true).SingleOrDefault();
            Assert.True(changedTimeSlot.IsFree == false);
            var newAppointment = mockContext.Object.Appointments.Where(ap => ap.PatientId.ToString() == patientId && ap.TimeSlotId.ToString() == timeSlotId
            && ap.VaccineId.ToString() == vaccineId && ap.State == AppointmentState.Planned).SingleOrDefault();
            Assert.Equal(expectedDose, newAppointment.WhichDose);
        }
        [Theory]
        [InlineData("kfdkffffff", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "e0d50915-5548-4993-aaaa-edddab4e1df1")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "uhnfuhfew", "e0d50915-5548-4993-aaaa-edddab4e1df3")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "afsdnjkfasdunfsda")]
        [InlineData(null, "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "e0d50915-5548-4993-aaaa-edddab4e1df3")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", null, "e0d50915-5548-4993-aaaa-edddab4e1df3")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", null)]
        public void BookVisitBadFormatTest(string patientId, string timeSlotId, string vaccineId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.BookVisit(patientId, timeSlotId, vaccineId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "b0780125-a945-4e20-b2ab-02bcf0ce8f3b", "e0d50915-5548-4993-aaaa-edddab4e1df2")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "b1780125-a945-4e20-b2ab-02bcf0ce8f3b", "e0d50915-5548-4993-aaaa-edddab4e1df3")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", "b2780125-a945-4e20-b2ab-02bcf0ce8f3b", "e0d50915-5548-4993-aaaa-edddab4e1df3")]
        public void BookVisitEmptyTest(string patientId, string timeSlotId, string vaccineId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());
            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());
            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);

            var controller = new PatientController(mockContext.Object);

            // Act
            var result = controller.BookVisit(patientId, timeSlotId, vaccineId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
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
        [InlineData("BadFormatBruh")]
        [InlineData(null)]
        public void GetIncomingVisitsBadFormatTest(string patientId)
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
            Assert.IsType<BadRequestResult>(result.Result);
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
            Appointment appointmentAfter = mockContext.Object.Appointments.Where(app => app.Id.ToString() == appointmentId).SingleOrDefault();
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
        [InlineData("BadFormat", "81a130d2-502f-4cf1-a376-63edeb000e9c")] // patient is not the owner of appointment
        [InlineData("baa26325-e151-4cd6-a829-254c0314faad", "BadFormat")]
        [InlineData(null, "81a130d2-502f-4cf1-a376-63edeb000e9f")]
        [InlineData("00000000-0000-0000-0000-000000000000", null)]
        public void CancelVisitBadFormatTest(string appointmentId, string patientId)
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
            Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9c", 1)]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9f", 2)]
        public void GetFormerVisitsTest(string patientId, int expectedVisitCount)
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
            Assert.Equal(expectedVisitCount, appointments.Count());
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
        [InlineData("BadRequest")]
        [InlineData(null)]
        public void GetFormerVisitsBadFormatTest(string patientId)
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
            Assert.IsType<BadRequestResult>(result.Result);
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
        [Theory]
        [InlineData("Bfrrrrrt")]
        [InlineData(null)]
        public void GetCertificatesBadFormatTest(string patientId)
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
            Assert.IsType<BadRequestResult>(result.Result);
        }
    }
}
