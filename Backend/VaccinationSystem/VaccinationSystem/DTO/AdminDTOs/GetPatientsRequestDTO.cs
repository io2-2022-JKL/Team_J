namespace VaccinationSystem.DTO.AdminDTOs
{
    public class GetPatientsRequestDTO
    {
        public string PESEL { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string mail { get; set; }
        public string dateOfBirth { get; set; }
        public string phoneNumber { get; set; }
    }
}
