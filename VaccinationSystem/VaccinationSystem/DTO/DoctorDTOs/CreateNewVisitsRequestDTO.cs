namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class CreateNewVisitsRequestDTO
    {
        public string timeFrom { get; set; }
        public string timeTo { get; set; }
        public int windowDurationInMinutes { get; set; }
    }
}
