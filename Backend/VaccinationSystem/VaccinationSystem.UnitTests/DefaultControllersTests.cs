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
    public class DefaultControllersTests : DataMock
    {
        [Theory]
        [InlineData("00000000004", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000004", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000004", "Adam-Micha'ł1234567890", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000004", "Adam Michał", "Testowy-'1234567890", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000004", "Adam Michał", "Testowy", "test@mail.pl", "12-12-1909", "testPassword", "+22 123456789")]
        [InlineData("00000000004", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "1", "+22 123456789")]
        [InlineData("00000000004", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "123456789")]
        [InlineData("00000000004", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "233 123456789")]
        [InlineData("00000000004", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "123456789012345")]
        [InlineData("00000000004", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+23456789012345")]
        public void RegisterUserEmptyTest(string pesel, string names, string surname, string mail, string dateOfBirth, string password, string phoneNumber)
        {
            // Arrange

            var patientsData = new List<Patient>();
            var patientsMockSet = GetMock(patientsData.AsQueryable());
            patientsMockSet.Setup(c => c.Add(It.IsAny<Patient>())).Callback(delegate (Patient p) {
                patientsData.Add(p);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientsMockSet.Object);

            var controller = new DefaultController(mockContext.Object);

            var request = new RegisterRequestDTO()
            {
                pesel = pesel,
                firstName = names,
                lastName = surname,
                mail = mail,
                dateOfBirth = dateOfBirth,
                password = password,
                phoneNumber = phoneNumber
            };

            // Act

            var result = controller.RegisterUser(request);

            // Assert

            Assert.IsType<OkResult>(result);
            Assert.Single(patientsData);
            var patient = patientsData[patientsData.Count() - 1];
            Assert.NotNull(patient);
            Assert.Equal(pesel, patient.PESEL);
            Assert.Equal(names, patient.FirstName);
            Assert.Equal(surname, patient.LastName);
            Assert.Equal(mail, patient.Mail);
            Assert.Equal(dateOfBirth, patient.DateOfBirth.ToString("dd-MM-yyyy"));
            Assert.Equal(password, patient.Password);
            Assert.Equal(phoneNumber.Replace(" ", ""), patient.PhoneNumber);
            Assert.True(patient.Active);
        }

        [Theory]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam-Micha'ł1234567890", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam Michał", "Testowy-'1234567890", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam Michał", "Testowy", "test@mail.pl", "12-12-1909", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "1", "+22 123456789")]
        [InlineData("00000000005", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "123456789")]
        [InlineData("00000000005", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "233 123456789")]
        [InlineData("00000000005", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "123456789012345")]
        [InlineData("00000000005", "Adam Michał", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+23456789012345")]
        public void RegisterUserTest(string pesel, string names, string surname, string mail, string dateOfBirth, string password, string phoneNumber)
        {
            // Arrange

            var patientsData = GetPatientsData().ToList();
            var patientsMockSet = GetMock(patientsData.AsQueryable());
            patientsMockSet.Setup(c => c.Add(It.IsAny<Patient>())).Callback(delegate (Patient p) {
                patientsData.Add(p);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientsMockSet.Object);

            var controller = new DefaultController(mockContext.Object);

            var request = new RegisterRequestDTO()
            {
                pesel = pesel,
                firstName = names,
                lastName = surname,
                mail = mail,
                dateOfBirth = dateOfBirth,
                password = password,
                phoneNumber = phoneNumber
            };

            // Act

            var result = controller.RegisterUser(request);

            // Assert

            Assert.IsType<OkResult>(result);
            Assert.Equal(GetPatientsData().Count() + 1, patientsData.Count());
            var patient = patientsData[patientsData.Count() - 1];
            Assert.NotNull(patient);
            Assert.Equal(pesel, patient.PESEL);
            Assert.Equal(names, patient.FirstName);
            Assert.Equal(surname, patient.LastName);
            Assert.Equal(mail, patient.Mail);
            Assert.Equal(dateOfBirth, patient.DateOfBirth.ToString("dd-MM-yyyy"));
            Assert.Equal(password, patient.Password);
            Assert.Equal(phoneNumber.Replace(" ", ""), patient.PhoneNumber);
            Assert.True(patient.Active);
        }

        [Theory]
        [InlineData("00000000004", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("0000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("000000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData(null, "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam!@#$%^&*()_+=[{]};:\"\\|<>,./", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", null, "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy!@#$%^&*()_+=[{]};:\"\\|<>,./", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "", "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", null, "test@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "@mail.pl", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "", "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", null, "07-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "32-07-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-13-2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07.07.2001", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "", "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", null, "testPassword", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "", "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", null, "+22 123456789")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "+22 1234567890123")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "1234567890123456")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "wrong_phone_number_format")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", "")]
        [InlineData("00000000005", "Adam", "Testowy", "test@mail.pl", "07-07-2001", "testPassword", null)]
        public void RegisterWrongUserTest(string pesel, string names, string surname, string mail, string dateOfBirth, string password, string phoneNumber)
        {
            // Arrange

            var patientsData = GetPatientsData().ToList();
            var patientsMockSet = GetMock(patientsData.AsQueryable());
            patientsMockSet.Setup(c => c.Add(It.IsAny<Patient>())).Callback(delegate (Patient p) {
                patientsData.Add(p);
            });

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientsMockSet.Object);

            var controller = new DefaultController(mockContext.Object);

            var request = new RegisterRequestDTO()
            {
                pesel = pesel,
                firstName = names,
                lastName = surname,
                mail = mail,
                dateOfBirth = dateOfBirth,
                password = password,
                phoneNumber = phoneNumber
            };

            // Act

            var result = controller.RegisterUser(request);

            // Assert

            Assert.IsType<BadRequestResult>(result);
            Assert.Equal(GetPatientsData().Count(), patientsData.Count());
        }

        [Theory]
        [InlineData("admin@systemszczepien.org.pl", "password", "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee", "admin")]
        [InlineData("jan.nowak@mail.com", "12345", "e0d50915-5548-4993-dddd-edddab4e1df1", "doctor")]
        [InlineData("agnieszka.kowalska@mail.com", "qwerty", "e0d50915-5548-4993-dddd-edddab4e1df3", "doctor")]
        [InlineData("artur.michalak@mail.com", "qwerty", "81a130d2-502f-4cf1-a376-63edeb000e9b", "patient")]
        [InlineData("magdalena.michalak@mail.com", "qwerty", "81a130d2-502f-4cf1-a376-63edeb000e9c", "patient")]
        public void SignInUserTest(string mail, string password, string expectedId, string expectedAccountType)
        {
            // Arrange

            var patientsData = GetPatientsData().ToList();
            var patientsMockSet = GetMock(patientsData.AsQueryable());
            patientsMockSet.Setup(c => c.Add(It.IsAny<Patient>())).Callback(delegate (Patient p) {
                patientsData.Add(p);
            });
            var doctorsData = GetDoctorsData().ToList();
            var doctorsMockSet = GetMock(doctorsData.AsQueryable());
            var adminsData = GetAdminsData().ToList();
            var adminsMockSet = GetMock(adminsData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientsMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorsMockSet.Object);
            mockContext.Setup(c => c.Admins).Returns(adminsMockSet.Object);

            var controller = new DefaultController(mockContext.Object);

            var request = new SigninRequestDTO()
            {
                mail = mail,
                password = password
            };

            // Act

            var result = controller.SignInUser(request);

            // Assert

            Assert.IsType<OkObjectResult>(result.Result);

            var response = result.Result as OkObjectResult;
            Assert.IsType<SigninResponseDTO>(response.Value);

            var account = response.Value as SigninResponseDTO;
            Assert.NotNull(account);
            Assert.Equal(expectedId, account.userID);
            Assert.Equal(expectedAccountType, account.userType);
        }

        [Theory]
        [InlineData("aadmin@systemszczepien.org.pl", "password")]
        [InlineData("test", "password")]
        [InlineData("", "password")]
        [InlineData(null, "password")]
        [InlineData("admin@systemszczepien.org.pl", "ppassword")]
        [InlineData("admin@systemszczepien.org.pl", "")]
        [InlineData("admin@systemszczepien.org.pl", null)]
        [InlineData("adam.nowak@mail.com", "haslo")]
        public void SignInWrongUserTest(string mail, string password)
        {
            // Arrange

            var patientsData = GetPatientsData().ToList();
            var patientsMockSet = GetMock(patientsData.AsQueryable());
            patientsMockSet.Setup(c => c.Add(It.IsAny<Patient>())).Callback(delegate (Patient p) {
                patientsData.Add(p);
            });
            var doctorsData = GetDoctorsData().ToList();
            var doctorsMockSet = GetMock(doctorsData.AsQueryable());
            var adminsData = GetAdminsData().ToList();
            var adminsMockSet = GetMock(adminsData.AsQueryable());

            var mockContext = new Mock<VaccinationSystemDbContext>();
            mockContext.Setup(c => c.Patients).Returns(patientsMockSet.Object);
            mockContext.Setup(c => c.Doctors).Returns(doctorsMockSet.Object);
            mockContext.Setup(c => c.Admins).Returns(adminsMockSet.Object);

            var controller = new DefaultController(mockContext.Object);

            var request = new SigninRequestDTO()
            {
                mail = mail,
                password = password
            };

            // Act

            var result = controller.SignInUser(request);

            // Assert

            Assert.IsType<BadRequestResult>(result.Result);
        }
    }
}
