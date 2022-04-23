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
using VaccinationSystem.DTO.DoctorDTOs;

namespace VaccinationSystem.UnitTests
{
    public class DoctorControllersTests : DataMock
    {
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "00000000-0000-0000-0000-000000000000")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df2", "81a130d2-502f-4cf1-a376-63edeb000e9f")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "81a130d2-502f-4cf1-a376-63edeb000e9a")]
        public void GetDoctorPatientIdTest(string doctorId, string expectedPatientId)
        {
            // Arrange
            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object);

            // Act
            var result = controller.GetDoctorPatientId(doctorId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var patientId = result.Result as OkObjectResult;
            Assert.IsType<GetDoctorPatientIdResponse>(patientId.Value);

            var acquiredPatientId = patientId.Value as GetDoctorPatientIdResponse;
            Assert.True(acquiredPatientId.patientId == expectedPatientId);
        }
        [Theory]
        [InlineData("e0d50915-fa3e-fa3e-fa3e-edddab4e1df2")]
        [InlineData("null")]
        [InlineData(null)]
        public void GetDoctorPatientIdEmptyTest(string doctorId)
        {
            // Arrange
            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object);

            // Act
            var result = controller.GetDoctorPatientId(doctorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3")]
        public void GetExistingTimeSlotsTest(string doctorId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new DoctorController(mockContext.Object);

            // Act
            var result = controller.GetExistingTimeSlots(doctorId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<ExistingTimeSlotDTO>>(list.Value);

            var timeSlots = list.Value as List<ExistingTimeSlotDTO>;
            int count = timeSlotData.Where(ts => ts.DoctorId.ToString() == doctorId && ts.Active == true).Count();
            Assert.Equal(count, timeSlots.Count());
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df2")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df4")]
        [InlineData("e0dhf715-5548-4993-dddd-edifub4e1394")]
        public void GetExistingTimeSlotsEmptyTest(string doctorId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new DoctorController(mockContext.Object);

            // Act
            var result = controller.GetExistingTimeSlots(doctorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 10:00", "01-03-2022 11:00", 15, 2)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df5", "01-03-2022 10:00", "01-03-2022 11:00", 15, 4)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "01-03-2022 11:00", "01-03-2022 12:00", 10, 6)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "01-03-2022 11:00", "01-03-2022 12:00", 11, 5)]
        public void CreateTimeSlotsTest(string doctorId, string from, string to, int timeSlotDurationInMinutes, int expectedAdditionalTimeSlots)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot ts) {
                timeSlotData.Add(ts);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object);
            CreateNewVisitsRequestDTO requestBody = new CreateNewVisitsRequestDTO()
            {
                from = from,
                to = to,
                timeSlotDurationInMinutes = timeSlotDurationInMinutes,
            };

            //var shiet = mockContext.Object.TimeSlots.Where(ts => ts.Active == true && ts.DoctorId.ToString() == doctorId);
            int timeSlotsBefore = mockContext.Object.TimeSlots.Where(ts => ts.Active == true && ts.DoctorId.ToString() == doctorId).Count();

            // Act
            var result = controller.CreateTimeSlots(doctorId, requestBody);

            // Assert
            Assert.IsType<OkResult>(result);

            //shiet = mockContext.Object.TimeSlots.Where(ts => ts.Active == true && ts.DoctorId.ToString() == doctorId);
            int timeSlotsAfter = mockContext.Object.TimeSlots.Where(ts => ts.Active == true && ts.DoctorId.ToString() == doctorId).Count();

            Assert.Equal(expectedAdditionalTimeSlots, timeSlotsAfter - timeSlotsBefore);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 10:00", "01-03-2022 10:10", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 10:00", "01-03-2022 10:00", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 11:00", "01-03-2022 12:00", 0)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 10:10", "01-03-2022 10:20", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 10:00", "01-03-2022 10:15", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 10:02", "01-03-2022 10:12", 10)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 09:55", "01-03-2022 10:20", 25)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 10:05", "01-03-2022 10:20", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 09:55", "01-03-2022 10:10", 15)]
        public void CreateTimeSlotsEmptyTest(string doctorId, string from, string to, int timeSlotDurationInMinutes)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object);
            CreateNewVisitsRequestDTO requestBody = new CreateNewVisitsRequestDTO()
            {
                from = from,
                to = to,
                timeSlotDurationInMinutes = timeSlotDurationInMinutes,
            };

            // Act
            var result = controller.CreateTimeSlots(doctorId, requestBody);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public void DeleteTimeSlotTest()
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot ts) {
                timeSlotData.Add(ts);
            });

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);

            var controller = new DoctorController(mockContext.Object);

            string doctorId = "e0d50915-5548-4993-dddd-edddab4e1df1";
            List<string> timeSlotIds = new List<string>();
            timeSlotIds.Add("a0780125-a945-4e20-b2ab-02bcf0ce8f3b");
            timeSlotIds.Add("a1780125-a945-4e20-b2ab-02bcf0ce8f3b");
            timeSlotIds.Add("a3780125-a945-4e20-b2ab-02bcf0ce8f3b");

            // Act
            var result = controller.DeleteTimeSlot(doctorId, timeSlotIds);

            // Assert
            Assert.IsType<OkResult>(result);

            var checkedTimeSlot = mockContext.Object.TimeSlots.Where(ts => ts.Id.ToString() == "a0780125-a945-4e20-b2ab-02bcf0ce8f3b").SingleOrDefault();
            Assert.True(checkedTimeSlot.Active == false);
            checkedTimeSlot = mockContext.Object.TimeSlots.Where(ts => ts.Id.ToString() == "a1780125-a945-4e20-b2ab-02bcf0ce8f3b").SingleOrDefault();
            Assert.True(checkedTimeSlot.Active == false);
            checkedTimeSlot = mockContext.Object.TimeSlots.Where(ts => ts.Id.ToString() == "a3780125-a945-4e20-b2ab-02bcf0ce8f3b").SingleOrDefault();
            Assert.True(checkedTimeSlot.Active == false);
            var checkedAppointment = mockContext.Object.Appointments.Where(ap => ap.Id.ToString() == "baa06325-e151-4cd6-a829-254c0314faad").SingleOrDefault();
            Assert.True(checkedAppointment.State == AppointmentState.Cancelled);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a2780125-a945-4e20-b2ab-02bcf0ce8f3b")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df5", "a2780125-a945-4e20-b2ab-02bcf0ce8f3b")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a2784345-a945-4e20-b2ab-02bcf0ce8f3b")]
        [InlineData(null, "a2784345-a945-4e20-b2ab-02bcf0ce8f3b")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", null)]
        [InlineData(null, null)]
        public void DeleteTimeSlotWrongTest(string doctorId, string timeSlotId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot ts) {
                timeSlotData.Add(ts);
            });

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);

            var controller = new DoctorController(mockContext.Object);
            List<string> list = new List<string>();
            list.Add(timeSlotId);

            // Act
            var result = controller.DeleteTimeSlot(doctorId, list);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public void GetFormerAppointmentsTest()
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object);

            string doctorId = "e0d50915-5548-4993-dddd-edddab4e1df1";

            // Act
            var result = controller.GetFormerAppointments(doctorId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var list = result.Result as OkObjectResult;

            Assert.IsType<List<DoctorFormerAppointmentDTO>>(list.Value);

            var appointments = list.Value as List<DoctorFormerAppointmentDTO>;
            Assert.Equal(2, appointments.Count());
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df5")]
        [InlineData(null)]
        public void GetFormerAppointmentsEmptyTest(string doctorId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object);
            // Act
            var result = controller.GetFormerAppointments(doctorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3")]
        public void GetIncomingAppointmentsTest(string doctorId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object);

            // Act
            var result = controller.GetIncomingAppointments(doctorId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var list = result.Result as OkObjectResult;

            Assert.IsType<List<DoctorIncomingAppointmentDTO>>(list.Value);

            var appointments = list.Value as List<DoctorIncomingAppointmentDTO>;
            Assert.Single(appointments);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df2")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df4")]
        [InlineData(null)]
        public void GetIncomingAppointmentsEmptyTest(string doctorId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccineMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object);
            // Act
            var result = controller.GetIncomingAppointments(doctorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
