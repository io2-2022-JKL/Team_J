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
    public class DoctorControllersTests: DataMock
    {
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
        public void CreateTimeSlotsTest(string doctorId, string from, string to, int timeSlotDurationInMinutes, int expectedadditionalTimeSlots)
        {
            // Arrange
            var timeSlotData = GetTimeSlotsData().ToList();
            var timeSlotMockSet = GetMock(timeSlotData.AsQueryable());
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot ts) {
                timeSlotData.Add(ts);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new DoctorController(mockContext.Object);
            CreateNewVisitsRequestDTO requestBody = new CreateNewVisitsRequestDTO()
            {
                from = from,
                to = to,
                timeSlotDurationInMinutes = timeSlotDurationInMinutes,
            };

            int timeSlotsBefore = mockContext.Object.TimeSlots.Where(ts => ts.DoctorId.ToString() == doctorId && ts.Active == true).Count();

            // Act
            var result = controller.CreateTimeSlots(doctorId, requestBody);

            // Assert
            Assert.IsType<OkResult>(result);

            int timeSlotsAfter = mockContext.Object.TimeSlots.Where(ts => ts.DoctorId.ToString() == doctorId && ts.Active == true).Count();

            Assert.Equal(expectedadditionalTimeSlots, timeSlotsAfter - timeSlotsBefore);
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
            timeSlotMockSet.Setup(c => c.Add(It.IsAny<TimeSlot>())).Callback(delegate (TimeSlot ts) {
                timeSlotData.Add(ts);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

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
    }
}
