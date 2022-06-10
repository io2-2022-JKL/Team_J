using Xunit;
using Moq;
using VaccinationSystem.Models;
using System.Collections.Generic;
using System.Linq;
using VaccinationSystem.Config;
using VaccinationSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using VaccinationSystem.DTO;
using VaccinationSystem.DTO.DoctorDTOs;
using System;
using VaccinationSystem.DTO.Errors;

namespace VaccinationSystem.UnitTests
{
    public class DoctorControllersTests : DataMock
    {
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "00000000-0000-0000-0000-000000000000")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "81a130d2-502f-4cf1-a376-63edeb000e9a")]
        public void GetDoctorPatientIdTest(string doctorId, string expectedPatientId)
        {
            // Arrange
            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var vaccinationCenterData = GetVaccinationCentersData().ToList();
            var vaccinationCenterMockSet = GetMock(vaccinationCenterData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.VaccinationCenters).Returns(vaccinationCenterMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetDoctorInfo(doctorId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var patientId = result.Result as OkObjectResult;
            Assert.IsType<GetDoctorInfoResponse>(patientId.Value);

            var acquiredPatientId = patientId.Value as GetDoctorInfoResponse;
            Assert.True(acquiredPatientId.patientAccountId == expectedPatientId);
        }
        [Theory]
        [InlineData("e0d50915-fa3e-fa3e-fa3e-edddab4e1df2")]
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

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetDoctorInfo(doctorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("null")]
        [InlineData(null)]
        public void GetDoctorPatientIdBadFormatTest(string doctorId)
        {
            // Arrange
            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetDoctorInfo(doctorId);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3")]
        public void GetExistingTimeSlotsTest(string doctorId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

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
        public void GetExistingTimeSlotsEmptyTest(string doctorId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetExistingTimeSlots(doctorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("e0dhf715-5548-4993-dddd-edifub4e1394")]
        [InlineData("BadFormat")]
        [InlineData("")]
        [InlineData(null)]
        public void GetExistingTimeSlotsBadFormatTest(string doctorId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetExistingTimeSlots(doctorId);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
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

            var controller = new DoctorController(mockContext.Object, null, null);
            CreateNewVisitsRequestDTO requestBody = new CreateNewVisitsRequestDTO()
            {
                windowBegin = from,
                windowEnd = to,
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
        [InlineData("BadFormat", "01-03-2022 09:55", "01-03-2022 10:10", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022", "01-03-2022 10:10", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "BadFormat", "01-03-2022 10:10", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 09:55", "01-03-2022", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 09:55", "BadFormat", 15)]
        [InlineData(null, "01-03-2022 09:55", "01-03-2022 10:10", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", null, "01-03-2022 10:10", 15)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "01-03-2022 09:55", null, 15)]
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

            var controller = new DoctorController(mockContext.Object, null, null);
            CreateNewVisitsRequestDTO requestBody = new CreateNewVisitsRequestDTO()
            {
                windowBegin = from,
                windowEnd = to,
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            string doctorId = "e0d50915-5548-4993-dddd-edddab4e1df1";
            List<TimeSlotToDeleteDTO> timeSlotIds = new List<TimeSlotToDeleteDTO>();
            timeSlotIds.Add(new TimeSlotToDeleteDTO()
            {
                id = "a0780125-a945-4e20-b2ab-02bcf0ce8f3b",
            });
            timeSlotIds.Add(new TimeSlotToDeleteDTO()
            {
                id = "a1780125-a945-4e20-b2ab-02bcf0ce8f3b",
            });
            timeSlotIds.Add(new TimeSlotToDeleteDTO()
            {
                id = "a3780125-a945-4e20-b2ab-02bcf0ce8f3b",
            });

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
        public void DeleteTimeSlotEmptyTest(string doctorId, string timeSlotId)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            List<TimeSlotToDeleteDTO> list = new List<TimeSlotToDeleteDTO>();
            list.Add(new TimeSlotToDeleteDTO()
            {
                id = timeSlotId,
            });

            // Act
            var result = controller.DeleteTimeSlot(doctorId, list);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Theory]
        [InlineData("BadFormat", "a2784345-a945-4e20-b2ab-02bcf0ce8f3b")]
        [InlineData(null, "a2784345-a945-4e20-b2ab-02bcf0ce8f3b")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", null)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "BadFormat")]
        [InlineData(null, null)]
        public void DeleteTimeSlotBadFormatTest(string doctorId, string timeSlotId)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            List<TimeSlotToDeleteDTO> list = new List<TimeSlotToDeleteDTO>();
            list.Add(new TimeSlotToDeleteDTO()
            {
                id = timeSlotId,
            });

            // Act
            var result = controller.DeleteTimeSlot(doctorId, list);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:00", "01-03-2022 10:10")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 09:00", "01-03-2022 10:15")]
        public void EditTimeSlotTest(string doctorId, string timeSlotId, string timeFrom, string timeTo)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            ModifyTimeSlotRequestDTO req = new ModifyTimeSlotRequestDTO()
            {
                timeFrom = timeFrom,
                timeTo = timeTo,
            };

            // Act
            var result = controller.tryModifyAppointment(doctorId, timeSlotId, req);

            // Assert
            Assert.True(result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a1780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:15", "01-03-2022 10:25", "baa06325-e151-4cd6-a829-254c0314faad")]
        public void EditTimeSlotCancelAppointmentTest(string doctorId, string timeSlotId, string timeFrom, string timeTo, string appointmentId)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            ModifyTimeSlotRequestDTO req = new ModifyTimeSlotRequestDTO()
            {
                timeFrom = timeFrom,
                timeTo = timeTo,
            };

            var appointmentBefore = mockContext.Object.Appointments.Where(ap => ap.Id == Guid.Parse(appointmentId)).SingleOrDefault();
            Assert.True(appointmentBefore.State == AppointmentState.Planned);

            // Act
            var result = controller.tryModifyAppointment(doctorId, timeSlotId, req);

            // Assert
            Assert.True(result);

            var appointmentAfter= mockContext.Object.Appointments.Where(ap => ap.Id == Guid.Parse(appointmentId)).SingleOrDefault();
            Assert.True(appointmentAfter.State == AppointmentState.Cancelled);
        }
        [Theory]
        [InlineData("badFormat", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:00", "01-03-2022 10:10")]
        [InlineData(null, "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:00", "01-03-2022 10:10")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "badFormat", "01-03-2022 10:00", "01-03-2022 10:10")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", null, "01-03-2022 10:00", "01-03-2022 10:10")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "10:00 01-03-2022", "01-03-2022 10:10")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", null, "01-03-2022 10:10")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:00", "10:10 01-03-2022")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:00", null)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:10", "01-03-2022 10:00")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:00", "01-03-2022 10:30")]
        public void EditTimeSlotBadRequestTest(string doctorId, string timeSlotId, string timeFrom, string timeTo)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            ModifyTimeSlotRequestDTO req = new ModifyTimeSlotRequestDTO()
            {
                timeFrom = timeFrom,
                timeTo = timeTo,
            };

            // Act
            try
            {
                var result = controller.tryModifyAppointment(doctorId, timeSlotId, req);
                Assert.True(1 == 0);
            }
            catch (BadRequestException)
            {
                Assert.True(1 == 1);
            }
            

            // Assert
            
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "a0780125-a945-4e20-b2ab-02bcf0ce8f3b", "01-03-2022 10:00", "01-03-2022 10:10")]
        public void EditTimeSlotFalseResponse(string doctorId, string timeSlotId, string timeFrom, string timeTo)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            ModifyTimeSlotRequestDTO req = new ModifyTimeSlotRequestDTO()
            {
                timeFrom = timeFrom,
                timeTo = timeTo,
            };

            // Act
            var result = controller.tryModifyAppointment(doctorId, timeSlotId, req);

            // Assert
            Assert.False(result);
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            // Act
            var result = controller.GetFormerAppointments(doctorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Theory]
        [InlineData("BadFormat")]
        [InlineData(null)]
        public void GetFormerAppointmentsBadRequestTest(string doctorId)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            // Act
            var result = controller.GetFormerAppointments(doctorId);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", 2)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", 1)]
        public void GetIncomingAppointmentsTest(string doctorId, int expectedNumber)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetIncomingAppointments(doctorId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var list = result.Result as OkObjectResult;

            Assert.IsType<List<DoctorIncomingAppointmentDTO>>(list.Value);

            var appointments = list.Value as List<DoctorIncomingAppointmentDTO>;
            Assert.Equal(appointments.Count, expectedNumber);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df2")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df4")]
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            // Act
            var result = controller.GetIncomingAppointments(doctorId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Theory]
        [InlineData("BadFormat")]
        [InlineData(null)]
        public void GetIncomingAppointmentsBadFormatTest(string doctorId)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);
            // Act
            var result = controller.GetIncomingAppointments(doctorId);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa06325-e151-4cd6-a829-254c0314faad", "vaccineA")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "baa16325-e151-4cd6-a829-254c0314faad", "vaccineC")]
        public void GetIncomingAppointmentTest(string doctorId, string appointmentId, string expectedVaccineName)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetIncomingAppointment(doctorId, appointmentId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var obj = result.Result as OkObjectResult;

            Assert.IsType<DoctorMarkedAppointmentResponseDTO>(obj.Value);

            var appointment = obj.Value as DoctorMarkedAppointmentResponseDTO;
            Assert.Equal(expectedVaccineName, appointment.vaccineName);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa16325-e151-4cd6-a829-254c0314faad")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1de0", "baa16325-e151-4cd6-a829-254c0314faad")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa16325-e151-4cd6-a829-254c0314eeee")]
        public void GetIncomingAppointmentEmptyTest(string doctorId, string appointmentId)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetIncomingAppointment(doctorId, appointmentId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Theory]
        [InlineData("BadFormat", "baa06325-e151-4cd6-a829-254c0314faad")]
        [InlineData(null, "baa06325-e151-4cd6-a829-254c0314faad")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "BadFormat")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", null)]
        public void GetIncomingAppointmentBadRequestTest(string doctorId, string appointmentId)
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

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccineMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.GetIncomingAppointment(doctorId, appointmentId);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa06325-e151-4cd6-a829-254c0314faad", "nice-batch-123", false)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "baa16325-e151-4cd6-a829-254c0314faad", "good-batch-321", false)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa66325-e151-4cd6-a829-254c0314faad", "pretty-batch-1", true)]
        public void ConfirmVaccinationTest(string doctorId, string appointmentId, string batchId, bool expectedCanCertify)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            //var appointmentBefore = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();

            // Act
            var result = controller.ConfirmVaccination(doctorId, appointmentId, batchId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var obj = result.Result as OkObjectResult;

            Assert.IsType<DoctorConfirmVaccinationResponseDTO>(obj.Value);

            var canCertify = obj.Value as DoctorConfirmVaccinationResponseDTO;
            Assert.Equal(expectedCanCertify, canCertify.canCertify);

            var appointmentNow = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();
            //Assert.True(appointmentBefore.State == AppointmentState.Planned);
            //Assert.True(appointmentBefore.CertifyState == CertifyState.NotLast);
            //Assert.True(appointmentBefore.VaccineBatchNumber == null);

            Assert.True(appointmentNow.State == AppointmentState.Finished);
            if(expectedCanCertify == false) Assert.True(appointmentNow.CertifyState == CertifyState.NotLast);
            else Assert.True(appointmentNow.CertifyState == CertifyState.LastNotCertified);
            Assert.True(appointmentNow.VaccineBatchNumber == batchId);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa16325-e151-4cd6-a829-254c0314faad", "nice-batch-123", true)]
        [InlineData("baa16325-e151-4cd6-a829-254c0314faad", "baa16325-e151-4cd6-a829-254c0314faad", "good-batch-321", true)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "e0d50915-5548-4993-dddd-edddab4e1df1", "nice-batch-123", false)]
        public void ConfirmVaccinationEmptyTest(string doctorId, string appointmentId, string batchId, bool validAppointmentId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);        

            // Act
            var result = controller.ConfirmVaccination(doctorId, appointmentId, batchId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);

            var appointmentBefore = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();
            if (validAppointmentId == true)
            {
                Assert.True(appointmentBefore.State == AppointmentState.Planned);
                Assert.True(appointmentBefore.CertifyState == CertifyState.NotLast);
                Assert.True(appointmentBefore.VaccineBatchNumber == null);
            }
            else Assert.True(appointmentBefore == null);
            
        }
        [Theory]
        [InlineData("BadFormat", "baa06325-e151-4cd6-a829-254c0314faad", "nice-batch-123", true)]
        [InlineData(null, "baa06325-e151-4cd6-a829-254c0314faad", "nice-batch-123", true)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "BadFormat", "nice-batch-123", false)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", null, "nice-batch-123", false)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa06325-e151-4cd6-a829-254c0314faad", null, true)]
        public void ConfirmVaccinationBadRequestTest(string doctorId, string appointmentId, string batchId, bool validAppointmentId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.ConfirmVaccination(doctorId, appointmentId, batchId);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);

            var appointmentBefore = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();
            if (validAppointmentId == true)
            {
                Assert.True(appointmentBefore.State == AppointmentState.Planned);
                Assert.True(appointmentBefore.CertifyState == CertifyState.NotLast);
                Assert.True(appointmentBefore.VaccineBatchNumber == null);
            }
            else Assert.True(appointmentBefore == null);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa06325-e151-4cd6-a829-254c0314faad")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df3", "baa16325-e151-4cd6-a829-254c0314faad")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa66325-e151-4cd6-a829-254c0314faad")]
        public void VaccinationDidNotHappenTest(string doctorId, string appointmentId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            //var appointmentBefore = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();

            // Act
            var result = controller.VaccinationDidNotHappen(doctorId, appointmentId);

            // Assert
            Assert.IsType<OkResult>(result);

            var appointmentNow = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();

            Assert.True(appointmentNow.State == AppointmentState.Cancelled);
            Assert.True(appointmentNow.CertifyState == CertifyState.NotLast);
        }
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa16325-e151-4cd6-a829-254c0314faad", true)]
        [InlineData("baa16325-e151-4cd6-a829-254c0314faad", "baa16325-e151-4cd6-a829-254c0314faad", true)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "e0d50915-5548-4993-dddd-edddab4e1df1", false)]
        public void VaccinationDidNotHappenEmptyTest(string doctorId, string appointmentId, bool validAppointmentId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.VaccinationDidNotHappen(doctorId, appointmentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            if(validAppointmentId == true)
            {
                var appointmentNow = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();

                Assert.True(appointmentNow.State == AppointmentState.Planned);
                Assert.True(appointmentNow.CertifyState == CertifyState.NotLast);
            }

        }
        [Theory]
        [InlineData("BadFormat", "baa06325-e151-4cd6-a829-254c0314faad", true)]
        [InlineData(null, "baa06325-e151-4cd6-a829-254c0314faad", true)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "BadFormat", false)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", null, false)]
        public void VaccinationDidNotHappenBadRequestTest(string doctorId, string appointmentId, bool validAppointmentId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            // Act
            var result = controller.VaccinationDidNotHappen(doctorId, appointmentId);

            // Assert
            Assert.IsType<BadRequestResult>(result);

            if (validAppointmentId == true)
            {
                var appointmentNow = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();

                Assert.True(appointmentNow.State == AppointmentState.Planned);
                Assert.True(appointmentNow.CertifyState == CertifyState.NotLast);
            }
        }
        /*[Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa66325-e151-4cd6-a829-254c0314faad")]
        public void CertifyTest(string doctorId, string appointmentId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var certificatesData = GetCertificatesData().ToList();
            var certificatesMockSet = GetMock(certificatesData.AsQueryable());
            certificatesMockSet.Setup(c => c.Add(It.IsAny<Certificate>())).Callback(delegate (Certificate cer) {
                certificatesData.Add(cer);
            });

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Certificates).Returns(certificatesMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            //var appointmentBefore = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();
            var certificateNumberBefore = certificatesData.Count();

            // Act
            controller.ConfirmVaccination(doctorId, appointmentId, "batch");
            var result = controller.Certify(doctorId, appointmentId);

            // Assert
            Assert.IsType<OkResult>(result);

            var appointmentNow = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();

            Assert.True(appointmentNow.State == AppointmentState.Finished);
            Assert.True(appointmentNow.CertifyState == CertifyState.Certified);

            var certificateNumberAfter = certificatesData.Count();
            Assert.Equal(certificateNumberAfter, certificateNumberBefore + 1);
        }*/
        [Theory]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "baa16325-e151-4cd6-a829-254c0314faad", true)]
        [InlineData("baa16325-e151-4cd6-a829-254c0314faad", "baa16325-e151-4cd6-a829-254c0314faad", true)]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "e0d50915-5548-4993-dddd-edddab4e1df1", false)]
        public void CertifyEmptyTest(string doctorId, string appointmentId, bool validAppointmentId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var certificatesData = GetCertificatesData().ToList();
            var certificatesMockSet = GetMock(certificatesData.AsQueryable());
            certificatesMockSet.Setup(c => c.Add(It.IsAny<Certificate>())).Callback(delegate (Certificate cer) {
                certificatesData.Add(cer);
            });

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Certificates).Returns(certificatesMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            //var appointmentBefore = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();
            var certificateNumberBefore = certificatesData.Count();

            // Act
            controller.ConfirmVaccination(doctorId, appointmentId, "batch");
            var result = controller.Certify(doctorId, appointmentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            if (validAppointmentId == true)
            {
                var appointmentNow = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();

                Assert.True(appointmentNow.State == AppointmentState.Planned);
                Assert.True(appointmentNow.CertifyState == CertifyState.NotLast);

                var certificateNumberAfter = certificatesData.Count();
                Assert.Equal(certificateNumberAfter, certificateNumberBefore);
            }

        }
        [Theory]
        [InlineData("BadFormat", "baa06325-e151-4cd6-a829-254c0314faad")]
        [InlineData(null, "baa06325-e151-4cd6-a829-254c0314faad")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", "BadFormat")]
        [InlineData("e0d50915-5548-4993-dddd-edddab4e1df1", null)]
        public void CertifyBadRequestTest(string doctorId, string appointmentId)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());

            var appointmentData = GetAppointmentsData().ToList();
            var appointmentMockSet = GetMock(appointmentData.AsQueryable());
            appointmentMockSet.Setup(c => c.Add(It.IsAny<Appointment>())).Callback(delegate (Appointment ap) {
                appointmentData.Add(ap);
            });

            var doctorData = GetDoctorsData().ToList();
            var doctorMockSet = GetMock(doctorData.AsQueryable());

            var vaccinesData = GetVaccinesData().ToList();
            var vaccinesMockSet = GetMock(vaccinesData.AsQueryable());

            var certificatesData = GetCertificatesData().ToList();
            var certificatesMockSet = GetMock(certificatesData.AsQueryable());
            certificatesMockSet.Setup(c => c.Add(It.IsAny<Certificate>())).Callback(delegate (Certificate cer) {
                certificatesData.Add(cer);
            });

            var patientData = GetPatientsData().ToList();
            var patientMockSet = GetMock(patientData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);
            mockContext.Setup(c => c.Appointments).Returns(appointmentMockSet.Object);
            mockContext.Setup(c => c.Vaccines).Returns(vaccinesMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.Certificates).Returns(certificatesMockSet.Object);
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);

            var controller = new DoctorController(mockContext.Object, null, null);

            //var appointmentBefore = appointmentData.Where(ap => ap.Id.ToString() == appointmentId).SingleOrDefault();
            var certificateNumberBefore = certificatesData.Count();

            // Act
            controller.ConfirmVaccination(doctorId, appointmentId, "batch");
            var result = controller.Certify(doctorId, appointmentId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            var certificateNumberAfter = certificatesData.Count();
            Assert.Equal(certificateNumberAfter, certificateNumberBefore);

        }
    }
}
