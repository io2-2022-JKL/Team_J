namespace VaccinationSystem.DTO.PatientDTOs
{
    public class TimeSlotFilterResponseDTO
    {
        public string day { get; set; }
        public string vaccinationCenterId { get; set; }
        public string vaccinationCenterName { get; set; }
        public string vaccinationCenterCity { get; set; }
        public string vaccinationCenterStreet { get; set; }
    }
}
