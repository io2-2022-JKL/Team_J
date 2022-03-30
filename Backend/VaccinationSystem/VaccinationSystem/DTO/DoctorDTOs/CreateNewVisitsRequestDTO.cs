namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class CreateNewVisitsRequestDTO
    {
        public string from { get; set; }
        public string to { get; set; }
        public int timeSlotDurationInMinutes { get; set; }
    }
}
