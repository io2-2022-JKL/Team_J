namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class CreateNewVisitsRequestDTO
    {
        public string windowBegin { get; set; }
        public string windowEnd { get; set; }
        public int timeSlotDurationInMinutes { get; set; }
    }
}
