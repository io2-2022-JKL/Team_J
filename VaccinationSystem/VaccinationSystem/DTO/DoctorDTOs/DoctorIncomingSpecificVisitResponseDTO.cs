namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class DoctorIncomingSpecificVisitResponseDTO
    {
        public string vaccineType { get; set; }
        public string vaccineData { get; set; } // zmienić w dokumentacji
        public int whichVaccineDose { get; set; }
        public string visitId { get; set; }
        public string patientNamesAndSurname { get; set; }
        public string PESEL { get; set; }
        public string windowBegin { get; set; }
        public string windowEnd { get; set; }
        public string vaccinationCenterName { get; set; }
        public string vaccinationCenterCity { get; set; }
        public string vaccinationCenterStreet { get; set; }
    }
}
