namespace VaccinationSystem.DTO
{
    public class RegisterRequestDTO
    {
        public string PESEL { get; set; }
        public string names { get; set; }
        public string surname { get; set; }
        public string mail { get; set; }
        public string dateOfBirth { get; set; }
        public string password { get; set; }
        public string phoneNumber { get; set; }

    }
}
