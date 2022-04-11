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
    public class AdminControllersTests
    {
        [Fact]
        public void GetPatientsTest()
        {
            // Arrange

            var data = new List<Patient>
            {
                new Patient{Id = Guid.NewGuid(),
                    PESEL = "00000000001",
                    FirstName = "Jan",
                    LastName = "Nowak",
                    DateOfBirth = DateTime.ParseExact("01-01-2000", "dd-MM-yyyy", null),
                    Mail = "jan.nowak@mail.com",
                    Password = "12345",
                    PhoneNumber = "+4812345679",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = true
                },
                new Patient{Id = Guid.NewGuid(),
                    PESEL = "00000000002",
                    FirstName = "Adam",
                    LastName = "Nowak",
                    DateOfBirth = DateTime.ParseExact("01-01-1950", "dd-MM-yyyy", null),
                    Mail = "adam.nowak@mail.com",
                    Password = "haslo",
                    PhoneNumber = "+48234567890",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = false
                },
                new Patient{Id = Guid.NewGuid(),
                    PESEL = "00000000002",
                    FirstName = "Agnieszka",
                    LastName = "Kowalska",
                    DateOfBirth = DateTime.ParseExact("01-01-1990", "dd-MM-yyyy", null),
                    Mail = "agnieszka.kowalska@mail.com",
                    Password = "qwerty",
                    PhoneNumber = "+48987654321",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = true
                }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Patient>>();
            mockSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

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
            Assert.Equal(3, patients.Count);

        }

        [Fact]
        public void GetPatientsEmptyTest()
        {
            // Arrange

            var data = new List<Patient>().AsQueryable();

            var mockSet = new Mock<DbSet<Patient>>();
            mockSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

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
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9a", 2)]
        public void DeletePatientTest(string patientId, int index)
        {
            // Arrange

            var patientData = new List<Patient>
            {
                new Patient{Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    PESEL = "00000000001",
                    FirstName = "Jan",
                    LastName = "Nowak",
                    DateOfBirth = DateTime.ParseExact("01-01-2000", "dd-MM-yyyy", null),
                    Mail = "jan.nowak@mail.com",
                    Password = "12345",
                    PhoneNumber = "+4812345679",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = true
                },
                new Patient{Id = Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9f"),
                    PESEL = "00000000002",
                    FirstName = "Adam",
                    LastName = "Nowak",
                    DateOfBirth = DateTime.ParseExact("01-01-1950", "dd-MM-yyyy", null),
                    Mail = "adam.nowak@mail.com",
                    Password = "haslo",
                    PhoneNumber = "+48234567890",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = false
                },
                new Patient{Id = Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9a"),
                    PESEL = "00000000002",
                    FirstName = "Agnieszka",
                    LastName = "Kowalska",
                    DateOfBirth = DateTime.ParseExact("01-01-1990", "dd-MM-yyyy", null),
                    Mail = "agnieszka.kowalska@mail.com",
                    Password = "qwerty",
                    PhoneNumber = "+48987654321",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = true
                }
            }.AsQueryable();

            var doctorData = new List<Doctor>
            {
                new Doctor{Id = Guid.NewGuid(),
                VaccinationCenterId = Guid.NewGuid(),
                VaccinationCenter = new VaccinationCenter(),
                Vaccinations = new List<Appointment>(),
                PatientAccount = patientData.ElementAt(0),
                Active = true}
            }.AsQueryable();

            var timeSlotData = new List<TimeSlot>().AsQueryable(); // nie mo¿e byæ null

            var patientMockSet = new Mock<DbSet<Patient>>();
            patientMockSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(patientData.Provider);
            patientMockSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(patientData.Expression);
            patientMockSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(patientData.ElementType);
            patientMockSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(patientData.GetEnumerator());

            var doctorMockSet = new Mock<DbSet<Doctor>>();
            doctorMockSet.As<IQueryable<Doctor>>().Setup(m => m.Provider).Returns(doctorData.Provider);
            doctorMockSet.As<IQueryable<Doctor>>().Setup(m => m.Expression).Returns(doctorData.Expression);
            doctorMockSet.As<IQueryable<Doctor>>().Setup(m => m.ElementType).Returns(doctorData.ElementType);
            doctorMockSet.As<IQueryable<Doctor>>().Setup(m => m.GetEnumerator()).Returns(doctorData.GetEnumerator());

            var timeSlotMockSet = new Mock<DbSet<TimeSlot>>();
            timeSlotMockSet.As<IQueryable<TimeSlot>>().Setup(m => m.Provider).Returns(timeSlotData.Provider);
            timeSlotMockSet.As<IQueryable<TimeSlot>>().Setup(m => m.Expression).Returns(timeSlotData.Expression);
            timeSlotMockSet.As<IQueryable<TimeSlot>>().Setup(m => m.ElementType).Returns(timeSlotData.ElementType);
            timeSlotMockSet.As<IQueryable<TimeSlot>>().Setup(m => m.GetEnumerator()).Returns(timeSlotData.GetEnumerator());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorMockSet.Object);
            mockContext.Setup(c => c.TimeSlots).Returns(timeSlotMockSet.Object);

            var controller = new AdminController(mockContext.Object);

            // Act

            var result = controller.DeletePatient(patientId);
            var getPatientsResult = controller.GetPatients();

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.IsType<OkObjectResult>(getPatientsResult.Result);

            var list = getPatientsResult.Result as OkObjectResult;
            Assert.IsType<List<PatientDTO>>(list.Value);

            var patients = list.Value as List<PatientDTO>;
            Assert.Equal(3, patients.Count);
            Assert.False(patients[index].active);

        }

        [Theory]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9f")]
        [InlineData("wrong_guid")]
        [InlineData("81a130d2-502f-4cf1-a376-63edeb000e9b")]
        [InlineData("")]
        public void DeleteWrongPatientTest(string patientId)
        {
            // Arrange

            var patientData = new List<Patient>
            {
                new Patient{Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    PESEL = "00000000001",
                    FirstName = "Jan",
                    LastName = "Nowak",
                    DateOfBirth = DateTime.ParseExact("01-01-2000", "dd-MM-yyyy", null),
                    Mail = "jan.nowak@mail.com",
                    Password = "12345",
                    PhoneNumber = "+4812345679",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = true
                },
                new Patient{Id = Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9f"),
                    PESEL = "00000000002",
                    FirstName = "Adam",
                    LastName = "Nowak",
                    DateOfBirth = DateTime.ParseExact("01-01-1950", "dd-MM-yyyy", null),
                    Mail = "adam.nowak@mail.com",
                    Password = "haslo",
                    PhoneNumber = "+48234567890",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = false
                },
                new Patient{Id = Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9a"),
                    PESEL = "00000000002",
                    FirstName = "Agnieszka",
                    LastName = "Kowalska",
                    DateOfBirth = DateTime.ParseExact("01-01-1990", "dd-MM-yyyy", null),
                    Mail = "agnieszka.kowalska@mail.com",
                    Password = "qwerty",
                    PhoneNumber = "+48987654321",
                    Vaccinations = new List<Appointment>(),
                    Certificates = new List<Certificate>(),
                    Active = true
                }
            }.AsQueryable();

            var doctorData = new List<Doctor>
            {
                new Doctor{Id = Guid.NewGuid(),
                VaccinationCenterId = Guid.NewGuid(),
                VaccinationCenter = new VaccinationCenter(),
                Vaccinations = new List<Appointment>(),
                PatientAccount = patientData.ElementAt(0),
                Active = true}
            }.AsQueryable();

            var timeSlotData = new List<TimeSlot>().AsQueryable(); // nie mo¿e byæ null

            var patientMockSet = new Mock<DbSet<Patient>>();
            patientMockSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(patientData.Provider);
            patientMockSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(patientData.Expression);
            patientMockSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(patientData.ElementType);
            patientMockSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(patientData.GetEnumerator());

            var doctorMockSet = new Mock<DbSet<Doctor>>();
            doctorMockSet.As<IQueryable<Doctor>>().Setup(m => m.Provider).Returns(doctorData.Provider);
            doctorMockSet.As<IQueryable<Doctor>>().Setup(m => m.Expression).Returns(doctorData.Expression);
            doctorMockSet.As<IQueryable<Doctor>>().Setup(m => m.ElementType).Returns(doctorData.ElementType);
            doctorMockSet.As<IQueryable<Doctor>>().Setup(m => m.GetEnumerator()).Returns(doctorData.GetEnumerator());

            var timeSlotMockSet = new Mock<DbSet<TimeSlot>>();
            timeSlotMockSet.As<IQueryable<TimeSlot>>().Setup(m => m.Provider).Returns(timeSlotData.Provider);
            timeSlotMockSet.As<IQueryable<TimeSlot>>().Setup(m => m.Expression).Returns(timeSlotData.Expression);
            timeSlotMockSet.As<IQueryable<TimeSlot>>().Setup(m => m.ElementType).Returns(timeSlotData.ElementType);
            timeSlotMockSet.As<IQueryable<TimeSlot>>().Setup(m => m.GetEnumerator()).Returns(timeSlotData.GetEnumerator());

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

    }
}
