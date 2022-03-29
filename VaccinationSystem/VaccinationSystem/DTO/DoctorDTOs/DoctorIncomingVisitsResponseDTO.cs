namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class DoctorIncomingVisitsResponseDTO
    {
        public string vaccineType { get; set; }
        public int whichVaccineDose { get; set; }
        public string visitId { get; set; }
        public string patientNamesAndSurname { get; set; }
        public string windowBegin { get; set; }
        public string windowEnd { get; set; }
        public string vaccinationCenterName { get; set; }
        public string vaccinationCenterCity { get; set; }
        public string vaccinationCenterStreet { get; set; }
    }
}
