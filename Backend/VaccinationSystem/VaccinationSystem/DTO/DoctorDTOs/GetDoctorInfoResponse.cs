namespace VaccinationSystem.DTO.DoctorDTOs
{
    public class GetDoctorInfoResponse
    {
        public string vaccinationCenterId { get; set; }
        public string vaccinationCenterName { get; set; }
        public string vaccinationCenterCity { get; set; }
        public string vaccinationCenterStreet { get; set; }
        public string patientId { get; set; }
    }
}
