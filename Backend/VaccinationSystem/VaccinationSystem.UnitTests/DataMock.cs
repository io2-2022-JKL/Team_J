using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VaccinationSystem.Models;

namespace VaccinationSystem.UnitTests
{
    public class DataMock
    {
        protected Mock<DbSet<T>> GetMock<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }

        protected IQueryable<Patient> GetPatientsData()
        {
            var data = new List<Patient>
            {
                new Patient{Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                    PESEL = "00000000001",
                    FirstName = "Jan",
                    LastName = "Nowak",
                    DateOfBirth = DateTime.ParseExact("01-01-2000", "dd-MM-yyyy", null),
                    Mail = "jan.nowak@mail.com",
                    Password = "12345",
                    PhoneNumber = "+4812345679",
                    //Vaccinations = new List<Appointment>(),
                    //Certificates = new List<Certificate>(),
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
                    //Vaccinations = new List<Appointment>(),
                    //Certificates = new List<Certificate>(),
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
                    //Vaccinations = new List<Appointment>(),
                    //Certificates = new List<Certificate>(),
                    Active = true
                },
                new Patient{Id = Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9b"),
                    PESEL = "00000000003",
                    FirstName = "Artur",
                    LastName = "Michalak",
                    DateOfBirth = DateTime.ParseExact("01-01-1970", "dd-MM-yyyy", null),
                    Mail = "artur.michalak@mail.com",
                    Password = "qwerty",
                    PhoneNumber = "+48987654322",
                    //Vaccinations = new List<Appointment>(),
                    //Certificates = new List<Certificate>(),
                    Active = true
                },
                new Patient{Id = Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9c"),
                    PESEL = "00000000004",
                    FirstName = "Magdalena",
                    LastName = "Michalak",
                    DateOfBirth = DateTime.ParseExact("01-01-1975", "dd-MM-yyyy", null),
                    Mail = "magdalena.michalak@mail.com",
                    Password = "qwerty",
                    PhoneNumber = "+48987654320",
                    //Vaccinations = new List<Appointment>(),
                    //Certificates = new List<Certificate>(),
                    Active = true
                },
                new Patient{Id = Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e8a"),
                    PESEL = "00000000005",
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    DateOfBirth = DateTime.ParseExact("01-01-1975", "dd-MM-yyyy", null),
                    Mail = "jan.kowalski@mail.com",
                    Password = "qwerty",
                    PhoneNumber = "+48987654318",
                    //Vaccinations = new List<Appointment>(),
                    //Certificates = new List<Certificate>(),
                    Active = true
                }
            }.AsQueryable();
            //foreach (var patient in data)
            //{
            //    patient.Vaccinations = GetAppointmentsData().Where(a => a.PatientId == patient.Id).ToList();
            //}
            return data;
        }

        protected IQueryable<Doctor> GetDoctorsData()
        {
            var data = new List<Doctor>
            {
                new Doctor
                {
                    Id = Guid.Parse("e0d50915-5548-4993-dddd-edddab4e1df1"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0),
                    //Vaccinations = new List<Appointment>(),
                    PatientId = GetPatientsData().ElementAt(0).Id,
                    PatientAccount = GetPatientsData().ElementAt(0),
                    Active = true
                },
                new Doctor
                {
                    Id = Guid.Parse("e0d50915-5548-4993-dddd-edddab4e1df2"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1),
                    //Vaccinations = new List<Appointment>(),
                    PatientId = GetPatientsData().ElementAt(1).Id,
                    PatientAccount = GetPatientsData().ElementAt(1),
                    Active = false
                },
                new Doctor
                {
                    Id = Guid.Parse("e0d50915-5548-4993-dddd-edddab4e1df3"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1),
                    //Vaccinations = new List<Appointment>(),
                    PatientId = GetPatientsData().ElementAt(2).Id,
                    PatientAccount = GetPatientsData().ElementAt(2),
                    Active = true
                },
                new Doctor
                {
                    Id = Guid.Parse("e0d50915-5548-4993-dddd-edddab4e1df4"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2),
                    //Vaccinations = new List<Appointment>(),
                    PatientId = GetPatientsData().ElementAt(3).Id,
                    PatientAccount = GetPatientsData().ElementAt(3),
                    Active = false
                },
                new Doctor
                {
                    Id = Guid.Parse("e0d50915-5548-4993-dddd-edddab4e1df5"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2),
                    //Vaccinations = new List<Appointment>(),
                    PatientId = GetPatientsData().ElementAt(5).Id,
                    PatientAccount = GetPatientsData().ElementAt(5),
                    Active = true
                }
            }.AsQueryable();
            //foreach (var doc in data)
            //{
            //    doc.Vaccinations = GetAppointmentsData().Where(d => d.TimeSlot.DoctorId == doc.Id).ToList();
            //}
            return data;
        }

        protected IQueryable<VaccinationCenter> GetVaccinationCentersData()
        {
            var data = new List<VaccinationCenter>
            {
                new VaccinationCenter
                {
                    Id = Guid.Parse("e0d50915-5548-4993-9204-edddab4e1dff"),
                    Name = "Centrum1",
                    City = "Warszawa",
                    Address = "ul. zakaźna 1",
                    Active = true
                },
                new VaccinationCenter
                {
                    Id = Guid.Parse("e1d50915-5548-4993-9204-edddab4e1dff"),
                    Name = "Centrum2",
                    City = "Kraków",
                    Address = "ul. krótka 13",
                    Active = true
                },
                new VaccinationCenter
                {
                    Id = Guid.Parse("e2d50915-5548-4993-9204-edddab4e1dff"),
                    Name = "Centrum3",
                    City = "Gdańsk",
                    Address = "ul. morska 7",
                    Active = false
                },
            }.AsQueryable();
            return data;
        }

        protected IQueryable<VaccinesInVaccinationCenter> GetVaccinesInVaccinationCentersData()
        {
            var data = new List<VaccinesInVaccinationCenter>
            {
                new VaccinesInVaccinationCenter
                {
                    Id = Guid.Parse("a67f4d2e-6c16-4035-9ceb-c1f14fb07269"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0),
                    VaccineId = GetVaccinesData().ElementAt(0).Id,
                    Vaccine = GetVaccinesData().ElementAt(0)
                },
                new VaccinesInVaccinationCenter
                {
                    Id = Guid.Parse("b67f4d2e-6c16-4035-9ceb-c1f14fb07269"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0),
                    VaccineId = GetVaccinesData().ElementAt(1).Id,
                    Vaccine = GetVaccinesData().ElementAt(1)
                },
                new VaccinesInVaccinationCenter
                {
                    Id = Guid.Parse("c67f4d2e-6c16-4035-9ceb-c1f14fb07269"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0),
                    VaccineId = GetVaccinesData().ElementAt(3).Id,
                    Vaccine = GetVaccinesData().ElementAt(3)
                },
                new VaccinesInVaccinationCenter
                {
                    Id = Guid.Parse("d67f4d2e-6c16-4035-9ceb-c1f14fb07269"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1),
                    VaccineId = GetVaccinesData().ElementAt(1).Id,
                    Vaccine = GetVaccinesData().ElementAt(1)
                },
                new VaccinesInVaccinationCenter
                {
                    Id = Guid.Parse("e67f4d2e-6c16-4035-9ceb-c1f14fb07269"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1),
                    VaccineId = GetVaccinesData().ElementAt(2).Id,
                    Vaccine = GetVaccinesData().ElementAt(2)
                },
                new VaccinesInVaccinationCenter
                {
                    Id = Guid.Parse("f67f4d2e-6c16-4035-9ceb-c1f14fb07269"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2),
                    VaccineId = GetVaccinesData().ElementAt(0).Id,
                    Vaccine = GetVaccinesData().ElementAt(0)
                },
                new VaccinesInVaccinationCenter
                {
                    Id = Guid.Parse("f77f4d2e-6c16-4035-9ceb-c1f14fb07269"),
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2),
                    VaccineId = GetVaccinesData().ElementAt(3).Id,
                    Vaccine = GetVaccinesData().ElementAt(3)
                },
            }.AsQueryable();
            return data;
        }

        protected IQueryable<Vaccine> GetVaccinesData()
        {
            var data = new List<Vaccine>
            {
                new Vaccine
                {
                    Id = Guid.Parse("e0d50915-5548-4993-aaaa-edddab4e1df1"),
                    Company = "A",
                    Name = "vaccineA",
                    NumberOfDoses = 2,
                    MinDaysBetweenDoses = 30,
                    MaxDaysBetweenDoses = 60,
                    Virus = Virus.Koronawirus,
                    MinPatientAge = 7,
                    MaxPatientAge = 99,
                    Active = true
                },
                new Vaccine
                {
                    Id = Guid.Parse("e0d50915-5548-4993-aaaa-edddab4e1df2"),
                    Company = "B",
                    Name = "vaccineB",
                    NumberOfDoses = 1,
                    MinDaysBetweenDoses = 0,
                    MaxDaysBetweenDoses = -1,
                    Virus = Virus.Koronawirus,
                    MinPatientAge = 0,
                    MaxPatientAge = -1,
                    Active = true
                },
                new Vaccine
                {
                    Id = Guid.Parse("e0d50915-5548-4993-aaaa-edddab4e1df3"),
                    Company = "C",
                    Name = "vaccineC",
                    NumberOfDoses = 3,
                    MinDaysBetweenDoses = 100,
                    MaxDaysBetweenDoses = -1,
                    Virus = Virus.Koronawirus,
                    MinPatientAge = 0,
                    MaxPatientAge = 70,
                    Active = true
                },
                new Vaccine
                {
                    Id = Guid.Parse("e0d50915-5548-4993-aaaa-edddab4e1df4"),
                    Company = "D",
                    Name = "vaccineD",
                    NumberOfDoses = 2,
                    MinDaysBetweenDoses = 10,
                    MaxDaysBetweenDoses = 40,
                    Virus = Virus.Koronawirus,
                    MinPatientAge = 3,
                    MaxPatientAge = 25,
                    Active = false
                }
            }.AsQueryable();
            return data;
        }

        protected IQueryable<OpeningHours> GetOpeningHoursData()
        {
            const string format = "hh\\:mm";
            var data = new List<OpeningHours>
            {
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb1-edddab4e1df1"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Monday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb1-edddab4e1df2"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Tuesday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb1-edddab4e1df3"),
                    From = TimeSpan.ParseExact("09:30", format, null),
                    To = TimeSpan.ParseExact("17:30", format, null),
                    WeekDay = WeekDay.Wednesday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb1-edddab4e1df4"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Thursday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb1-edddab4e1df5"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("20:00", format, null),
                    WeekDay = WeekDay.Friday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb1-edddab4e1df6"),
                    From = TimeSpan.ParseExact("12:00", format, null),
                    To = TimeSpan.ParseExact("16:00", format, null),
                    WeekDay = WeekDay.Saturday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb1-edddab4e1df7"),
                    From = TimeSpan.ParseExact("00:00", format, null),
                    To = TimeSpan.ParseExact("00:00",  format, null),
                    WeekDay = WeekDay.Sunday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(0).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(0)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb2-edddab4e1df1"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Monday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb2-edddab4e1df2"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Tuesday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb2-edddab4e1df3"),
                    From = TimeSpan.ParseExact("00:00", format, null),
                    To = TimeSpan.ParseExact("00:00", format, null),
                    WeekDay = WeekDay.Wednesday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb2-edddab4e1df4"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Thursday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb2-edddab4e1df5"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("20:00", format, null),
                    WeekDay = WeekDay.Friday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb2-edddab4e1df6"),
                    From = TimeSpan.ParseExact("12:00", format, null),
                    To = TimeSpan.ParseExact("16:00", format, null),
                    WeekDay = WeekDay.Saturday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb2-edddab4e1df7"),
                    From = TimeSpan.ParseExact("00:00", format, null),
                    To = TimeSpan.ParseExact("00:00", format, null),
                    WeekDay = WeekDay.Sunday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(1).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(1)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb3-edddab4e1df1"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Monday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb3-edddab4e1df2"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Tuesday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb3-edddab4e1df3"),
                    From = TimeSpan.ParseExact("09:30", format, null),
                    To = TimeSpan.ParseExact("17:30", format, null),
                    WeekDay = WeekDay.Wednesday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb3-edddab4e1df4"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("18:00", format, null),
                    WeekDay = WeekDay.Thursday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb3-edddab4e1df5"),
                    From = TimeSpan.ParseExact("10:00", format, null),
                    To = TimeSpan.ParseExact("20:00", format, null),
                    WeekDay = WeekDay.Friday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb3-edddab4e1df6"),
                    From = TimeSpan.ParseExact("12:00", format, null),
                    To = TimeSpan.ParseExact("16:00", format, null),
                    WeekDay = WeekDay.Saturday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2)
                },
                new OpeningHours
                {
                    Id = Guid.Parse("e0d50915-5548-4993-bbb3-edddab4e1df7"),
                    From = TimeSpan.ParseExact("00:00", format, null),
                    To = TimeSpan.ParseExact("00:00", format, null),
                    WeekDay = WeekDay.Sunday,
                    VaccinationCenterId = GetVaccinationCentersData().ElementAt(2).Id,
                    VaccinationCenter = GetVaccinationCentersData().ElementAt(2)
                }
            }.AsQueryable();
            return data;
        }

        protected IQueryable<TimeSlot> GetTimeSlotsData()
        {
            string format = "dd-MM-yyyy HH\\:mm";
            var data = new List<TimeSlot>
            {
                new TimeSlot // 0
                {
                    Id = Guid.Parse("a0780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 10:00", format, null),
                    To = DateTime.ParseExact("01-03-2022 10:15", format, null),
                    DoctorId = GetDoctorsData().ElementAt(0).Id,
                    Doctor = GetDoctorsData().ElementAt(0),
                    IsFree = true,
                    Active = true
                },
                new TimeSlot // 1
                {
                    Id = Guid.Parse("a1780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 10:15", format, null),
                    To = DateTime.ParseExact("01-03-2022 10:30", format, null),
                    DoctorId = GetDoctorsData().ElementAt(0).Id,
                    Doctor = GetDoctorsData().ElementAt(0),
                    IsFree = false,
                    Active = true
                },
                new TimeSlot // 2
                {
                    Id = Guid.Parse("a2780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 10:30", format, null),
                    To = DateTime.ParseExact("01-03-2022 10:45", format, null),
                    DoctorId = GetDoctorsData().ElementAt(0).Id,
                    Doctor = GetDoctorsData().ElementAt(0),
                    IsFree = false,
                    Active = false
                },
                new TimeSlot // 3
                {
                    Id = Guid.Parse("a3780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 10:45", format, null),
                    To = DateTime.ParseExact("01-03-2022 11:00", format, null),
                    DoctorId = GetDoctorsData().ElementAt(0).Id,
                    Doctor = GetDoctorsData().ElementAt(0),
                    IsFree = true,
                    Active = false
                },
                new TimeSlot // 4
                {
                    Id = Guid.Parse("b0780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 10:00", format, null),
                    To = DateTime.ParseExact("01-03-2022 10:15", format, null),
                    DoctorId = GetDoctorsData().ElementAt(2).Id,
                    Doctor = GetDoctorsData().ElementAt(2),
                    IsFree = true,
                    Active = true
                },
                new TimeSlot // 5
                {
                    Id = Guid.Parse("b1780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 10:15", format, null),
                    To = DateTime.ParseExact("01-03-2022 10:30", format, null),
                    DoctorId = GetDoctorsData().ElementAt(2).Id,
                    Doctor = GetDoctorsData().ElementAt(2),
                    IsFree = false,
                    Active = true
                },
                new TimeSlot // 6
                {
                    Id = Guid.Parse("b2780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 10:30", format, null),
                    To = DateTime.ParseExact("01-03-2022 10:45", format, null),
                    DoctorId = GetDoctorsData().ElementAt(2).Id,
                    Doctor = GetDoctorsData().ElementAt(2),
                    IsFree = false,
                    Active = false
                },
                new TimeSlot // 7
                {
                    Id = Guid.Parse("b3780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 10:45", format, null),
                    To = DateTime.ParseExact("01-03-2022 11:00", format, null),
                    DoctorId = GetDoctorsData().ElementAt(2).Id,
                    Doctor = GetDoctorsData().ElementAt(2),
                    IsFree = true,
                    Active = false
                },
                new TimeSlot // 8
                {
                    Id = Guid.Parse("b4781125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-01-2022 10:45", format, null),
                    To = DateTime.ParseExact("01-01-2022 11:00", format, null),
                    DoctorId = GetDoctorsData().ElementAt(2).Id,
                    Doctor = GetDoctorsData().ElementAt(2),
                    IsFree = false,
                    Active = false
                },
                new TimeSlot // 9
                {
                    Id = Guid.Parse("a4780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 11:00", format, null),
                    To = DateTime.ParseExact("01-03-2022 11:15", format, null),
                    DoctorId = GetDoctorsData().ElementAt(0).Id,
                    Doctor = GetDoctorsData().ElementAt(0),
                    IsFree = false,
                    Active = true
                },
                new TimeSlot // 10
                {
                    Id = Guid.Parse("a5780125-a945-4e20-b2ab-02bcf0ce8f3b"),
                    From = DateTime.ParseExact("01-03-2022 11:15", format, null),
                    To = DateTime.ParseExact("01-03-2022 11:30", format, null),
                    DoctorId = GetDoctorsData().ElementAt(0).Id,
                    Doctor = GetDoctorsData().ElementAt(0),
                    IsFree = false,
                    Active = true
                },
            }.AsQueryable();
            return data;
        }

        protected IQueryable<Appointment> GetAppointmentsData()
        {
            var data = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.Parse("baa06325-e151-4cd6-a829-254c0314faad"),
                    WhichDose = 1,
                    TimeSlotId = GetTimeSlotsData().ElementAt(1).Id,
                    TimeSlot = GetTimeSlotsData().ElementAt(1),
                    PatientId = GetPatientsData().ElementAt(2).Id,
                    Patient = GetPatientsData().ElementAt(2),
                    VaccineId = GetVaccinesData().ElementAt(0).Id,
                    Vaccine = GetVaccinesData().ElementAt(0),
                    State = AppointmentState.Planned,
                    VaccineBatchNumber = null
                },
                new Appointment
                {
                    Id = Guid.Parse("baa16325-e151-4cd6-a829-254c0314faad"),
                    WhichDose = 2,
                    TimeSlotId = GetTimeSlotsData().ElementAt(5).Id,
                    TimeSlot = GetTimeSlotsData().ElementAt(5),
                    PatientId = GetPatientsData().ElementAt(4).Id,
                    Patient = GetPatientsData().ElementAt(4),
                    VaccineId = GetVaccinesData().ElementAt(2).Id,
                    Vaccine = GetVaccinesData().ElementAt(2),
                    State = AppointmentState.Planned,
                    VaccineBatchNumber = null
                },
                new Appointment
                {
                    Id = Guid.Parse("baa26325-e151-4cd6-a829-254c0314faad"),
                    WhichDose = 1,
                    TimeSlotId = GetTimeSlotsData().ElementAt(8).Id,
                    TimeSlot = GetTimeSlotsData().ElementAt(8),
                    PatientId = GetPatientsData().ElementAt(4).Id,
                    Patient = GetPatientsData().ElementAt(4),
                    VaccineId = GetVaccinesData().ElementAt(2).Id,
                    Vaccine = GetVaccinesData().ElementAt(2),
                    State = AppointmentState.Finished,
                    VaccineBatchNumber = "012345"
                },
                new Appointment
                {
                    Id = Guid.Parse("baa36325-e151-4cd6-a829-254c0314faad"),
                    WhichDose = 1,
                    TimeSlotId = GetTimeSlotsData().ElementAt(2).Id,
                    TimeSlot = GetTimeSlotsData().ElementAt(2),
                    PatientId = GetPatientsData().ElementAt(1).Id,
                    Patient = GetPatientsData().ElementAt(1),
                    VaccineId = GetVaccinesData().ElementAt(1).Id,
                    Vaccine = GetVaccinesData().ElementAt(1),
                    State = AppointmentState.Cancelled,
                    VaccineBatchNumber = null
                },
                new Appointment
                {
                    Id = Guid.Parse("baa46325-e151-4cd6-a829-254c0314faad"),
                    WhichDose = 1,
                    TimeSlotId = GetTimeSlotsData().ElementAt(9).Id,
                    TimeSlot = GetTimeSlotsData().ElementAt(9),
                    PatientId = GetPatientsData().ElementAt(1).Id,
                    Patient = GetPatientsData().ElementAt(1),
                    VaccineId = GetVaccinesData().ElementAt(0).Id,
                    Vaccine = GetVaccinesData().ElementAt(0),
                    State = AppointmentState.Finished,
                    VaccineBatchNumber = "KORW-KR"
                },
                new Appointment
                {
                    Id = Guid.Parse("baa46325-e151-4cd6-a829-254c0314faad"),
                    WhichDose = 1,
                    TimeSlotId = GetTimeSlotsData().ElementAt(10).Id,
                    TimeSlot = GetTimeSlotsData().ElementAt(10),
                    PatientId = GetPatientsData().ElementAt(5).Id,
                    Patient = GetPatientsData().ElementAt(5),
                    VaccineId = GetVaccinesData().ElementAt(0).Id,
                    Vaccine = GetVaccinesData().ElementAt(0),
                    State = AppointmentState.Cancelled,
                    VaccineBatchNumber = null
                },
            }.AsQueryable();
            return data;
        }

        protected IQueryable<Admin> GetAdminsData()
        {
            var data = new List<Admin>()
            {
                new Admin() {
                    Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                    PESEL = "00000000000",
                    FirstName = "Super",
                    LastName = "Admin",
                    Mail = "admin@systemszczepien.org.pl",
                    DateOfBirth = DateTime.ParseExact("17-04-2022", "dd-MM-yyyy", null),
                    Password = "password",
                    PhoneNumber = "+48 888 777 666"
                }
            }.AsQueryable();
            return data;
        }

        protected IQueryable<Certificate> GetCertificatesData()
        {
            var data = new List<Certificate>()
            {
                new Certificate()
                {
                    Id = Guid.Parse("aaaa1111-bb22-cc33-dd44-eeeeee555555"),
                    Url = "https://vaccinationsystem.blob.core.windows.net/certificates/Janusz_Mikke/327ac066-cd35-4be4-aa10-3cb2bba248eb.pdf",
                    PatientId = GetPatientsData().ElementAt(1).Id,
                    Patient = GetPatientsData().ElementAt(1),
                    VaccineId = GetVaccinesData().ElementAt(1).Id,
                    Vaccine = GetVaccinesData().ElementAt(1),
                },
                new Certificate()
                {
                    Id = Guid.Parse("aaaa1112-bb22-cc33-dd44-eeeeee555555"),
                    Url = "https://vaccinationsystem.blob.core.windows.net/certificates/Janusz_Mikke/327ac066-cd35-4be4-aa10-3cb2bba248eb.pdf",
                    PatientId = GetPatientsData().ElementAt(3).Id,
                    Patient = GetPatientsData().ElementAt(3),
                    VaccineId = GetVaccinesData().ElementAt(2).Id,
                    Vaccine = GetVaccinesData().ElementAt(2),
                },
            }.AsQueryable();
            return data;
        }
    }
}
