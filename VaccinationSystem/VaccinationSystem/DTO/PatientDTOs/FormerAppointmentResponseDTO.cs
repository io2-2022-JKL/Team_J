namespace VaccinationSystem.DTO.PatientDTOs
{
    public class FormerAppointmentResponseDTO
    {
        public string vaccineType { get; set; }
        public string vaccineBatch { get; set; }
        public int whichVaccineDose { get; set; }
        public string visitId { get; set; }
        public string windowBegin { get; set; }
        public string windowEnd { get; set; }
        public string vaccinationCenterName { get; set; }
        public string vaccinationCenterCity { get; set; }
        public string vaccinationCenterStreet { get; set; }
    }
}
